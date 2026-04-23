using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class ApiAuthorsController : ControllerBase
    {
        private readonly IAuthorService _svc;
        public ApiAuthorsController(IAuthorService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<PagedResult<Author>>> Get([FromQuery] string? searchTerm, [FromQuery] string? sortBy,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _svc.GetPagedAsync(searchTerm, sortBy, page, pageSize));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var a = await _svc.GetByIdAsync(id);
            return a == null ? NotFound() : Ok(a);
        }

        [HttpPost]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Author>> Create([FromBody] Author author)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = await _svc.CreateAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(int id, [FromBody] Author author)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _svc.UpdateAsync(id, author);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, reason) = await _svc.DeleteAsync(id);
            if (!ok && reason == "Not found.") return NotFound();
            if (!ok) return Conflict(new { message = reason });
            return NoContent();
        }
    }
}