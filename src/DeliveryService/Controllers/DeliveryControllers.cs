using Microsoft.AspNetCore.Mvc;
using DeliveryService.Models;
using DeliveryService.Data;

namespace DeliveryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public DeliveryController(DeliveryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new delivery and saves it to the database.
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Read), new { id = delivery.Id }, delivery);
        }

        /// <summary>
        /// Retrieves all deliveries from the database.
        /// </summary>
        [HttpGet]
        public IActionResult Read()
        {
            var deliveries = _context.Deliveries.ToList();
            return Ok(deliveries);
        }
    }
}
