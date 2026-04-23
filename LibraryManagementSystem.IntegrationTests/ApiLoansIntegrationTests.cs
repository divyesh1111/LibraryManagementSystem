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

        [Fact]
        public async Task GetLoans_ShouldReturn200_ForAdmin()
        {
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.GetAsync("/api/v1/loans?page=1&pageSize=5");
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("items", body);
        }

        [Fact]
        public async Task GetLoans_ShouldReturn403_ForMember()
        {
            var token = await LoginAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.GetAsync("/api/v1/loans?page=1&pageSize=5");

            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Fact]
        public async Task GetLoans_ShouldReturn200_ForLibrarian()
        {
            var token = await LoginAsync("librarian@library.local", "Librarian123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.GetAsync("/api/v1/loans?page=1&pageSize=5");

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Fact]
        public async Task GetLoans_ShouldReturn401_WhenNoToken()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var res = await _client.GetAsync("/api/v1/loans");

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task MarkOverdue_ShouldReturn200_ForAdmin()
        {
            var token = await LoginAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await _client.PostAsync("/api/v1/loans/mark-overdue", null);
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("marked", body);
        }
    }
}