using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities;
using System;
using System.Threading.Tasks;

namespace InventoryService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            var result = await _service.CreateAsync(product);
            return Created("", result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid product ID.");

            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0)
                return BadRequest("Invalid page number.");

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest("Invalid page size.");

            var products = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(products);
        }
    }
}
