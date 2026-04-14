using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers.Api.V1
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class ApiAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _tokenService;

        public ApiAuthController(UserManager<ApplicationUser> userManager, IJwtTokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] ApiLoginRequest request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return Unauthorized(new { message = "Invalid credentials." });

            var ok = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!ok) return Unauthorized(new { message = "Invalid credentials." });

            var token = await _tokenService.CreateTokenAsync(user);
            return Ok(token);
        }
    }
}