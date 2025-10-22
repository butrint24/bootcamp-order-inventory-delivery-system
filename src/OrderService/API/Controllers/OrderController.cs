using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Shared.DTOs.Order;
using System;
using System.Threading.Tasks;

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
            var order = await _service.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDto dto)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid order ID.");

            var updatedOrder = await _service.UpdateOrderAsync(id, dto);

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
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid order ID.");

            var deleted = await _service.DeleteOrderAsync(id);
            if (!deleted)
                return NotFound($"Order with ID {id} not found.");

            return NoContent();
        }
    }
}
