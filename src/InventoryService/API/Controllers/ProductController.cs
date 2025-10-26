using System.Net.Mime;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace InventoryService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto, IFormFile? image, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var imageUrl = await SaveImageAsync(image, ct) ?? "https://placehold.co/600x400";

            var result = await _service.CreateProductAsync(dto, imageUrl);

            result.ImageUrl = ToAbsoluteUrl(result.ImageUrl);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId }, result);
        }

    
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var product = await _service.GetByIdAsync(id);
            if (product is null) return NotFound();

            product.ImageUrl = ToAbsoluteUrl(product.ImageUrl);
            return Ok(product);
        }

    
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            if (pageNumber <= 0) return BadRequest("Invalid page number.");
            if (pageSize <= 0 || pageSize > 100) return BadRequest("Invalid page size.");

            var products = (await _service.GetAllAsync(
                searchTerm: null,
                sortBy: null,
                ascending: true,
                pageNumber: pageNumber,
                pageSize: pageSize,
                minPrice: null,
                maxPrice: null,
                category: null,
                inStock: null
            )).ToList();

            for (int i = 0; i < products.Count; i++)
                products[i].ImageUrl = ToAbsoluteUrl(products[i].ImageUrl);

            return Ok(products);
        }

        
        [HttpPut("{id:guid}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var updated = await _service.UpdateProductAsync(id, dto);
            if (updated is null) return NotFound($"No product found with ID {id}.");

            updated.ImageUrl = ToAbsoluteUrl(updated.ImageUrl);
            return Ok(updated);
        }

        
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        
        [HttpPatch("restore/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var restored = await _service.RestoreAsync(id);
            return restored ? Ok() : NotFound();
        }

        private string? ToAbsoluteUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return url;

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return baseUrl + url;
        }

        private async Task<string?> SaveImageAsync(IFormFile? file, CancellationToken ct)
        {
            if (file is null || file.Length == 0) return null;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new InvalidOperationException("Invalid image type.");

            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relFolder = Path.Combine("images", "products");
            var absFolder = Path.Combine(webRoot, relFolder);
            Directory.CreateDirectory(absFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var absPath = Path.Combine(absFolder, fileName);

            await using var s = new FileStream(absPath, FileMode.Create);
            await file.CopyToAsync(s, ct);

            return "/" + Path.Combine(relFolder, fileName).Replace("\\", "/");
        }
    }
}
