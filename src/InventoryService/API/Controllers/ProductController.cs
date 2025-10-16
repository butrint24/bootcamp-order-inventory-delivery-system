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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid product ID.");

            if (product == null)
                return BadRequest("Please provide product data to update.");


            product.ProductId = id;

            var updatedProduct = await _service.UpdateAsync(product);

            if (updatedProduct == null)
                return NotFound($"No product found with ID {id}.");

            return Ok(updatedProduct);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid product ID.");

            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Product {id} not found." });

            return NoContent(); 
        }

    }
}
