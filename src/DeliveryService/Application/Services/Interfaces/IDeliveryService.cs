using Shared.DTOs;
using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DeliveryService.Application.Services.Interfaces
{
    public interface IDeliveryService
    {
        Task<DeliveryResponseDto> CreateDeliveryAsync(DeliveryCreateDto dto);
        Task<DeliveryResponseDto?> UpdateDeliveryAsync(Guid id, DeliveryUpdateDto dto);
        Task<DeliveryResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<DeliveryResponseDto>> GetAllAsync(
            string? searchTerm,
            string? sortBy,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10,
            DateTime? minEta = null,
            DateTime? maxEta = null,
            string? status = null,
            Guid? orderId = null,
            Guid? userId = null);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
        Task ProcessPendingDeliveriesAsync();
    }
}
