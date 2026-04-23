using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests
{
    public class ApiAuthorsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApiAuthorsIntegrationTests(CustomWebApplicationFactory factory)
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

        // Integration Test 1
        [Fact]
        public async Task GetAuthors_ShouldReturn200_WhenAuthenticated()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.GetAsync("/api/v1/authors?page=1&pageSize=5");
            var body = await res.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("items", body);
            Assert.Contains("totalCount", body);
        }

        // Integration Test 2
        [Fact]
        public async Task GetAuthors_ShouldReturn401_WhenNotAuthenticated()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var res = await _client.GetAsync("/api/v1/authors");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        // Integration Test 3
        [Fact]
        public async Task CreateAuthor_ShouldReturn201_WhenAdminCreates()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                firstName = "New",
                lastName = "Author",
                nationality = "Indian",
                isActive = true
            };

            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/authors", payload);
            var body = await res.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            Assert.Contains("new", body.ToLower());
        }

        // Integration Test 4
        [Fact]
        public async Task CreateAuthor_ShouldReturn403_WhenMemberTries()
        {
            // Arrange
            var token = await LoginAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                firstName = "Hacker",
                lastName = "Author",
                isActive = true
            };

            // Act
            var res = await _client.PostAsJsonAsync("/api/v1/authors", payload);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        // Integration Test 5
        [Fact]
        public async Task GetAuthorById_ShouldReturn404_WhenNotExists()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var res = await _client.GetAsync("/api/v1/authors/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        // Integration Test 6
        [Fact]
        public async Task GetAuthorById_ShouldReturn200_WhenExists()
        {
            // Arrange
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act (seeded author with Id=1 from DbSeeder)
            var res = await _client.GetAsync("/api/v1/authors/1");
            var body = await res.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("firstName", body);
        }
    }
}