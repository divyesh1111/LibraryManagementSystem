using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Api.Security
{
    public interface IJwtTokenService
    {
        Task<TokenResponse> CreateTokenAsync(ApplicationUser user);
    }

    public record TokenResponse(string AccessToken, DateTime ExpiresAtUtc, string TokenType = "Bearer");
}