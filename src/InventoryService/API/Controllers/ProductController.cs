using Shared.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var result = await _service.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortBy,
            [FromQuery] bool ascending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? category = null,
            [FromQuery] bool? inStock = null)
        {
            if (pageNumber <= 0) return BadRequest("Invalid page number.");
            if (pageSize <= 0 || pageSize > 100) return BadRequest("Invalid page size.");

            var products = await _service.GetAllAsync(searchTerm, sortBy, ascending, pageNumber, pageSize, minPrice, maxPrice, category, inStock);
            return Ok(products);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var updatedProduct = await _service.UpdateProductAsync(id, dto);
            if (updatedProduct == null) return NotFound($"No product found with ID {id}.");

            return Ok(updatedProduct);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPatch("restore/{id:guid}")]
        public async Task<IActionResult> Restore(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid product ID.");

            var restored = await _service.RestoreAsync(id);
            return restored ? Ok() : NotFound();
        }
    }
}
