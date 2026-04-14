using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/branches")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class ApiLibraryBranchesController : ControllerBase
    {
        private readonly ILibraryBranchService _svc;
        public ApiLibraryBranchesController(ILibraryBranchService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<PagedResult<LibraryBranch>>> Get([FromQuery] string? searchTerm, [FromQuery] bool? activeOnly, [FromQuery] string? sortBy,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _svc.GetPagedAsync(searchTerm, activeOnly, sortBy, page, pageSize));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibraryBranch>> GetById(int id)
        {
            var b = await _svc.GetByIdAsync(id);
            return b == null ? NotFound() : Ok(b);
        }

        [HttpPost]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<LibraryBranch>> Create([FromBody] LibraryBranch branch)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = await _svc.CreateAsync(branch);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(int id, [FromBody] LibraryBranch branch)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _svc.UpdateAsync(id, branch);
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