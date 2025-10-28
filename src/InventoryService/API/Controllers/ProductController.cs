using Shared.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Shared.DTOs;
=======
using System;
using System.Threading.Tasks;
>>>>>>> 9c4d298 (Add image upload support for Product)

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

<<<<<<< HEAD
        
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
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

<<<<<<< HEAD
    
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid product ID.");

            var product = await _service.GetByIdAsync(id);
            if (product is null) return NotFound();

            return Ok(product);
        }

<<<<<<< HEAD
    
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
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

<<<<<<< HEAD
        
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
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

<<<<<<< HEAD
        
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
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

<<<<<<< HEAD
        
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
        [HttpPatch("restore/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid product ID.");

            var restored = await _service.RestoreAsync(id);
            return restored ? Ok() : NotFound();
        }
<<<<<<< HEAD

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
=======
>>>>>>> 9c4d298 (Add image upload support for Product)
    }
}
