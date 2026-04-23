using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests
{
    public class ApiCategoriesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApiCategoriesIntegrationTests(CustomWebApplicationFactory factory)
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

        // Integration Test 14
        [Fact]
        public async Task GetCategories_ShouldReturnSeededData()
        {
            // Arrange
            var token = await LoginAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.GetAsync("/api/v1/categories?page=1&pageSize=20");
            var body = await res.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("Fiction", body);
        }

        // Integration Test 15
        [Fact]
        public async Task CreateCategory_ShouldReturn201_WhenAdminCreates()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                name = $"TestCategory_{Guid.NewGuid():N}",
                description = "Integration test category",
                isActive = true,
                displayOrder = 99
            };

            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/categories", payload);

            // Assert
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        // Integration Test 16
        [Fact]
        public async Task CreateCategory_ShouldReturn403_WhenMemberTries()
        {
            // Arrange
            var token = await LoginAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                name = "Unauthorized Category",
                isActive = true
            };

            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/categories", payload);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }
}