using Shared.DTOs;
using DeliveryService.Application.Services.Interfaces;
using AutoMapper;
using DeliveryService.Infrastructure.Repositories.Interfaces;
using Shared.Entities;

namespace DeliveryService.Application.Services.Implementations
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _repo;
        private readonly IMapper _mapper;
        private readonly int _DeliveriesToProcessPerDay = 20;

        public DeliveryService(IDeliveryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<DeliveryResponseDto> CreateDeliveryAsync(DeliveryCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            Delivery delivery = _mapper.Map<Delivery>(dto);
            delivery.IsActive = true;
            await _repo.AddAsync(delivery);
            await _repo.SaveChangesAsync();

            return _mapper.Map<DeliveryResponseDto>(delivery);
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
                //update order status here
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
                //also here
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
                //and here
                _repo.Update(delivery);
            }

            await _repo.SaveChangesAsync();
        }
    }
}
