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

        public async Task<IEnumerable<DeliveryResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var deliveries = await _repo.GetAllAsync(pageNumber, pageSize);
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
    }
}
