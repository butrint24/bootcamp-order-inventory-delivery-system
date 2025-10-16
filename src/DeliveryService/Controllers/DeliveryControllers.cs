using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private static readonly List<dynamic> Deliveries = new();

        //Create a new delivery
        [HttpPost]
        public IActionResult Create([FromBody] dynamic delivery)
        {
            delivery.Id = Deliveries.Count + 1;
            Deliveries.Add(delivery);
            return CreatedAtAction(nameof(Read), new { id = delivery.Id }, delivery);
        }

        //Read
        [HttpGet]
        public IActionResult Read()
        {
            return Ok(Deliveries);
        }
    }
}
