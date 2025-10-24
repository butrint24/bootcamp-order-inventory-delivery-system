using Application.Services.Interfaces;
using Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Shared.Attributes;
using Shared.Enums;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [AuthorizeRoleAttribute(RoleType.Admin)]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            // if (!ModelState.IsValid)
            //     return BadRequest("Invalid user data.");

            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user ID.");

            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound($"No user found with ID {id}.");

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0)
                return BadRequest("Invalid page number.");

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest("Invalid page size.");

            var users = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(users);
        }

        [HttpPut("{id:guid}")]
        [AuthorizeRoleAttribute(RoleType.Admin)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user ID.");

            // if (!ModelState.IsValid)
            //     return BadRequest("Invalid user data.");

            var result = await _service.UpdateAsync(id, dto);
            if (result == null)
                return NotFound($"No user found with ID {id}.");

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [AuthorizeRoleAttribute(RoleType.Admin)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user ID.");

            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound($"User with ID {id} not found.");
        }

        [HttpPatch("restore/{id:guid}")]
        [AuthorizeRoleAttribute(RoleType.Admin)]
        public async Task<IActionResult> Activate(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user ID.");

            var success = await _service.RestoreAsync(id);
            return success ? NoContent() : NotFound($"User with ID {id} not found.");
        }
    }
}
