using Shared.DTOs;
using DeliveryService.Application.Services.Interfaces;
using AutoMapper;
using DeliveryService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;
using Shared.Enums;
using DeliveryService.GrpcGenerated;
using DeliveryService.Application.Clients;

namespace DeliveryService.Application.Services.Implementations
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _repo;
        private readonly IMapper _mapper;
        private readonly OrderGrpcClient _orderClient;
        private readonly int _DeliveriesToProcessPerDay = 20;

        public DeliveryService(IDeliveryRepository repo, IMapper mapper, OrderGrpcClient orderClient)
        {
            _repo = repo;
            _mapper = mapper;
            _orderClient = orderClient;
        }

        public async Task<DeliveryResponseDto> CreateDeliveryAsync(DeliveryCreateDto dto, Guid userId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            dto.UserId = userId;
            Delivery delivery = _mapper.Map<Delivery>(dto);
            delivery.IsActive = true;
            await _repo.AddAsync(delivery);
            await _repo.SaveChangesAsync();

            return _mapper.Map<DeliveryResponseDto>(delivery);
        }

        public async Task<CreateDeliveryResponse> CreateDeliveryGrpcAsync(Guid orderId, Guid userId)
        {
            var eta = await CalculateEtaAsync();
            DeliveryCreateDto dto = new DeliveryCreateDto
            {
                OrderId = orderId,
                UserId = userId,
                Status = DeliveryStatus.PENDING,
                Eta = eta
            };

            DeliveryResponseDto? delivery;
            try
            {
                delivery = await CreateDeliveryAsync(dto, userId);
            }
            catch (Exception)
            {
                return new CreateDeliveryResponse
                {
                    Success = false
                };
            }
            return new CreateDeliveryResponse
            {
                Success = delivery != null
            };
        }

        public async Task<DeliveryResponseDto?> GetByIdAsync(Guid id)
        {
            var delivery = await _repo.GetByIdAsync(id);
            return delivery == null ? null : _mapper.Map<DeliveryResponseDto>(delivery);
        }

        public async Task<IEnumerable<DeliveryResponseDto>> GetAllAsync(
            string? searchTerm,
            string? sortBy,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            DateTime? minEta = null,
            DateTime? maxEta = null,
            string? status = null,
            Guid? orderId = null,
            Guid? userId = null)
        {
            var deliveries = await _repo.SearchSortAndFilterAsync(
                searchTerm, sortBy, ascending, pageNumber, pageSize,
                minEta, maxEta, status, orderId, userId);

            return deliveries?.Select(d => _mapper.Map<DeliveryResponseDto>(d)) ?? Enumerable.Empty<DeliveryResponseDto>();
        }

        public async Task<DeliveryResponseDto?> UpdateDeliveryAsync(Guid id, DeliveryUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            _mapper.Map(dto, existing);

            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            return _mapper.Map<DeliveryResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deleted = await _repo.SoftDeleteAsync(id);
            if (deleted) await _repo.SaveChangesAsync();
            return deleted;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var restored = await _repo.RestoreAsync(id);
            if (restored) await _repo.SaveChangesAsync();
            return restored;
        }

        public async Task ProcessPendingDeliveriesAsync()
        {
            var pendingDeliveries = await _repo.GetNextPendingDeliveriesAsync(_DeliveriesToProcessPerDay);

            foreach (var delivery in pendingDeliveries)
            {
                delivery.MarkProcessing();
                await _orderClient.UpdateOrderStatusAsync(delivery.OrderId, OrderStatus.PROCESSING.ToString());
                _repo.Update(delivery);
            }

            await _repo.SaveChangesAsync();
        }

        public async Task ProcessDeliveriesToProcessAsync()
        {
            var deliveriesToProcess = await _repo.GetNextDeliveriesToProcessAsync(_DeliveriesToProcessPerDay);

            foreach (var delivery in deliveriesToProcess)
            {
                delivery.MarkOnRoute();
                await _orderClient.UpdateOrderStatusAsync(delivery.OrderId, OrderStatus.SHIPPED.ToString());
                _repo.Update(delivery);
            }

            await _repo.SaveChangesAsync();
        }

        public async Task ProcessOnRouteDeliveriesAsync()
        {
            var onRouteDeliveries = await _repo.GetNextOnRouteDeliveriesAsync(_DeliveriesToProcessPerDay);

            foreach (var delivery in onRouteDeliveries)
            {
                delivery.MarkDelivered();
                await _orderClient.UpdateOrderStatusAsync(delivery.OrderId, OrderStatus.COMPLETED.ToString());
                _repo.Update(delivery);
            }

            await _repo.SaveChangesAsync();
        }
        private async Task<DateTime> CalculateEtaAsync()
        {
            int dailyCapacity = _DeliveriesToProcessPerDay;
            var now = DateTime.Now;
            var cronHour = 8;

            var pendingCount = await _repo.GetPendingCountAsync();

            var processedTodayCount = await _repo.GetProcessedCountForDateAsync(DateTime.Today);

            int remainingSlotsToday = dailyCapacity - processedTodayCount;

            int daysToWait = 0;

            if (now.Hour < cronHour)
            {
                if (pendingCount < remainingSlotsToday)
                    daysToWait = 0;
                else
                    daysToWait = (pendingCount - remainingSlotsToday) / dailyCapacity + 
                                ((pendingCount - remainingSlotsToday) % dailyCapacity != 0 ? 1 : 0);
            }
            else
            {
                daysToWait = pendingCount / dailyCapacity + (pendingCount % dailyCapacity != 0 ? 1 : 0);
            }

            var eta = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, cronHour, 0, 0)
                            .AddDays(daysToWait);

            return eta;
        }


    }
}
