using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/reviews")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class ApiReviewsController : ControllerBase
    {
        private readonly IReviewService _svc;
        public ApiReviewsController(IReviewService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<PagedResult<Review>>> Get([FromQuery] int? bookId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var includeUnapproved = User.IsInRole("Admin") || User.IsInRole("Librarian");
            return Ok(await _svc.GetPagedAsync(bookId, includeUnapproved, page, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Review>> GetById(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            return r == null ? NotFound() : Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized(new { message = "Email claim not found in token." });

            var (ok, reason, created) = await _svc.CreateAsync(review, email);
            if (!ok) return Conflict(new { message = reason });

            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(int id, [FromBody] Review review)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _svc.UpdateAsync(id, review);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, reason) = await _svc.DeleteAsync(id);
            if (!ok && reason == "Not found.") return NotFound();
            if (!ok) return Conflict(new { message = reason });
            return NoContent();
        }

        [HttpPost("{id:int}/approve")]
        [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Approve(int id, [FromQuery] bool approved = true)
        {
            var ok = await _svc.SetApprovalAsync(id, approved);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("{id:int}/helpful")]
        public async Task<IActionResult> Helpful(int id)
        {
            var ok = await _svc.IncrementHelpfulAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}