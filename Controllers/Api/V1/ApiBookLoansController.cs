using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/loans")]
    [Authorize(Policy = "ManageLibrary", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class ApiBookLoansController : ControllerBase
    {
        private readonly IBookLoanService _svc;
        public ApiBookLoansController(IBookLoanService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<PagedResult<BookLoan>>> Get([FromQuery] string? searchTerm, [FromQuery] LoanStatus? status,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _svc.GetPagedAsync(searchTerm, status, page, pageSize));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookLoan>> GetById(int id)
        {
            var l = await _svc.GetByIdAsync(id);
            return l == null ? NotFound() : Ok(l);
        }

        [HttpPost]
        public async Task<ActionResult<BookLoan>> Create([FromBody] BookLoan loan)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var created = await _svc.CreateAsync(loan);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookLoan loan)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _svc.UpdateAsync(id, loan);
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

        [HttpPost("mark-overdue")]
        public async Task<ActionResult<object>> MarkOverdue()
        {
            var count = await _svc.MarkOverdueAsync();
            return Ok(new { marked = count });
        }

        [HttpPost("{id:int}/return")]
        public async Task<IActionResult> Return(int id, [FromQuery] DateTime? returnDateUtc, [FromQuery] string? notes)
        {
            var date = returnDateUtc ?? DateTime.UtcNow;
            var (ok, reason) = await _svc.ReturnAsync(id, date, notes);
            if (!ok && reason == "Not found.") return NotFound();
            if (!ok) return Conflict(new { message = reason });
            return NoContent();
        }
    }
}