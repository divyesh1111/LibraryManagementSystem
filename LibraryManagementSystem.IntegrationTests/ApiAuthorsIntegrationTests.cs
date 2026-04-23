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

        [Fact]
        public async Task GetAuthors_ShouldReturn200_WhenAuthenticated()
        {
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.GetAsync("/api/v1/authors?page=1&pageSize=5");
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("items", body);
            Assert.Contains("totalCount", body);
        }

        [Fact]
        public async Task GetAuthors_ShouldReturn401_WhenNotAuthenticated()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var res = await _client.GetAsync("/api/v1/authors");

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task CreateAuthor_ShouldReturn201_WhenAdminCreates()
        {
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

            var res = await _client.PostAsJsonAsync("/api/v1/authors", payload);
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            Assert.Contains("new", body.ToLower());
        }

        [Fact]
        public async Task CreateAuthor_ShouldReturn403_WhenMemberTries()
        {
            var token = await LoginAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                firstName = "Hacker",
                lastName = "Author",
                isActive = true
            };

            var res = await _client.PostAsJsonAsync("/api/v1/authors", payload);

            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Fact]
        public async Task GetAuthorById_ShouldReturn404_WhenNotExists()
        {
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.GetAsync("/api/v1/authors/99999");

            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetAuthorById_ShouldReturn200_WhenExists()
        {
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.GetAsync("/api/v1/authors/1");
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("firstName", body);
        }
    }
}