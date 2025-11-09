using Shared.DTOs;
using DeliveryService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DeliveryService.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _service;

        public DeliveryController(IDeliveryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryCreateDto dto)
        {
            if (!TryGetUserId(out var userId))
                return Forbid("Missing or invalid X-User-Id header.");

            var result = await _service.CreateDeliveryAsync(dto, userId);
            return Created("", result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid delivery ID.");

            var delivery = await _service.GetByIdAsync(id);
            if (delivery == null)
                return NotFound();

            return Ok(delivery);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortBy,
            [FromQuery] bool ascending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? minEta = null,
            [FromQuery] DateTime? maxEta = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? orderId = null,
            [FromQuery] Guid? userId = null)
        {
            if (pageNumber <= 0)
                return BadRequest("Invalid page number.");

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest("Invalid page size.");

            var deliveries = await _service.GetAllAsync(searchTerm, sortBy, ascending, pageNumber, pageSize, minEta, maxEta, status, orderId, userId);
            return Ok(deliveries);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DeliveryUpdateDto dto)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid delivery ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedDelivery = await _service.UpdateDeliveryAsync(id, dto);
            if (updatedDelivery == null)
                return NotFound($"No delivery found with ID {id}.");

            return Ok(updatedDelivery);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid delivery ID.");

            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("restore/{id:guid}")]
        public async Task<IActionResult> Restore(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid delivery ID.");

            var restored = await _service.RestoreAsync(id);
            return restored ? Ok() : NotFound();
        }
        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            if (!Request.Headers.TryGetValue("X-User-Id", out var header))
                return false;

            return Guid.TryParse(header, out userId);
        }
    }
}
