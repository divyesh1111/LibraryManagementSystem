using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests
{
    public class ApiAuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApiAuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
        }

        // Integration Test 17
        [Fact]
        public async Task Login_ShouldReturn200_AndToken_ForAdminUser()
        {
            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest
                {
                    Email = "admin@library.local",
                    Password = "Admin123!"
                });

            var body = await res.Content.ReadAsStringAsync();
            var token = await res.Content.ReadFromJsonAsync<TokenResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));
            Assert.Equal("Bearer", token.TokenType);
            Assert.True(token.ExpiresAtUtc > DateTime.UtcNow);
        }

        // Integration Test 18
        [Fact]
        public async Task Login_ShouldReturn200_AndToken_ForMemberUser()
        {
            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest
                {
                    Email = "member@library.local",
                    Password = "Member123!"
                });

            var token = await res.Content.ReadFromJsonAsync<TokenResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));
        }

        // Integration Test 19
        [Fact]
        public async Task Login_ShouldReturn401_ForWrongPassword()
        {
            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest
                {
                    Email = "admin@library.local",
                    Password = "WrongPassword!"
                });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        // Integration Test 20
        [Fact]
        public async Task Login_ShouldReturn401_ForNonExistentUser()
        {
            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest
                {
                    Email = "nobody@doesnotexist.com",
                    Password = "Whatever123!"
                });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        // Integration Test 21
        [Fact]
        public async Task Login_ShouldReturn400_WhenEmailIsEmpty()
        {
            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest
                {
                    Email = "",
                    Password = "Admin123!"
                });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        // Integration Test 22
        [Fact]
        public async Task Login_ShouldReturn200_ForLibrarianUser()
        {
            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest
                {
                    Email = "librarian@library.local",
                    Password = "Librarian123!"
                });

            var token = await res.Content.ReadFromJsonAsync<TokenResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));
        }
    }
}