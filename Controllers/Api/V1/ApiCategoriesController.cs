using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/categories")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class ApiCategoriesController : ControllerBase
    {
        private readonly ICategoryService _svc;
        public ApiCategoriesController(ICategoryService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<PagedResult<Category>>> Get([FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(await _svc.GetPagedAsync(searchTerm, page, pageSize));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var c = await _svc.GetByIdAsync(id);
            return c == null ? NotFound() : Ok(c);
        }

        [HttpPost]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = await _svc.CreateAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _svc.UpdateAsync(id, category);
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