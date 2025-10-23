using Application.DTOs;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryService.Application.Services.Interfaces
{
    public interface IDeliveryService
    {
        Task<DeliveryResponseDto> CreateDeliveryAsync(DeliveryCreateDto dto);
        Task<DeliveryResponseDto?> UpdateDeliveryAsync(Guid id, DeliveryUpdateDto dto);
        Task<DeliveryResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<DeliveryResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
    }
}
