using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Shared.DTOs.Order;
using System;
using System.Threading.Tasks;
using Shared.Attributes;
using Shared.Enums;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
        {
            if (!TryGetUserId(out var userId))
                return Forbid("Missing or invalid X-User-Id header.");

            var order = await _service.CreateOrderAsync(dto, userId);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [HttpPut("{id:guid}")]
        [AuthorizeRoleAttribute(RoleType.Admin)]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDto dto)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid order ID.");

            if (!TryGetUserId(out var userId))
                return Forbid("Missing or invalid X-User-Id header.");

            var updatedOrder = await _service.UpdateOrderAsync(id, dto, userId);

            if (updatedOrder == null)
                return NotFound($"Order with ID {id} not found.");

            return Ok(updatedOrder);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid order ID.");

            var order = await _service.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var orders = await _service.GetAllOrdersAsync(pageNumber, pageSize);
            return Ok(orders);
        }

        [HttpDelete("{id:guid}")]
        [AuthorizeRoleAttribute(RoleType.Admin)]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid order ID.");

            if (!TryGetUserId(out var userId))
                return Forbid("Missing or invalid X-User-Id header.");

            var deleted = await _service.DeleteOrderAsync(id, userId);
            if (!deleted)
                return NotFound($"Order with ID {id} not found.");

            return NoContent();
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
