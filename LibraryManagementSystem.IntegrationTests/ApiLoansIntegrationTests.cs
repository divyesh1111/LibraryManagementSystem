using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests
{
    public class ApiLoansIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApiLoansIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
        }

        private async Task<string> LoginAsync(string email, string password)
        {
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new ApiLoginRequest { Email = email, Password = password });
            res.EnsureSuccessStatusCode();
            var token = await res.Content.ReadFromJsonAsync<TokenResponse>();
            return token!.AccessToken;
        }

        // Integration Test 23
        [Fact]
        public async Task GetLoans_ShouldReturn200_ForAdmin()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.GetAsync("/api/v1/loans?page=1&pageSize=5");
            var body = await res.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("items", body);
        }

        // Integration Test 24
        [Fact]
        public async Task GetLoans_ShouldReturn403_ForMember()
        {
            // Arrange
            var token = await LoginAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.GetAsync("/api/v1/loans?page=1&pageSize=5");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        // Integration Test 25
        [Fact]
        public async Task GetLoans_ShouldReturn200_ForLibrarian()
        {
            // Arrange
            var token = await LoginAsync("librarian@library.local", "Librarian123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.GetAsync("/api/v1/loans?page=1&pageSize=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        // Integration Test 26
        [Fact]
        public async Task GetLoans_ShouldReturn401_WhenNoToken()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var res = await _client.GetAsync("/api/v1/loans");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        // Integration Test 27
        [Fact]
        public async Task MarkOverdue_ShouldReturn200_ForAdmin()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.PostAsync("/api/v1/loans/mark-overdue", null);
            var body = await res.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("marked", body);
        }
    }
}