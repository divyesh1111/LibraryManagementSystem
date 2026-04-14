using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests
{
    public class ApiSmokeTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ApiSmokeTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
        }

        private async Task<string> LoginAndGetTokenAsync(string email, string password)
        {
            var res = await _client.PostAsJsonAsync("/api/v1/auth/login", new ApiLoginRequest
            {
                Email = email,
                Password = password
            });

            if (res.StatusCode == HttpStatusCode.TemporaryRedirect || res.StatusCode == HttpStatusCode.PermanentRedirect)
            {
                var location = res.Headers.Location?.ToString() ?? "(no location)";
                throw new Exception($"Unexpected redirect during login. Status={res.StatusCode}, Location={location}");
            }

            res.EnsureSuccessStatusCode();

            var token = await res.Content.ReadFromJsonAsync<TokenResponse>();
            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));

            return token.AccessToken;
        }

        [Fact]
        public async Task Books_Get_ShouldReturn200_WhenAuthenticated()
        {
            var token = await LoginAndGetTokenAsync("member@library.local", "Member123!");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var res = await _client.GetAsync("/api/v1/books?page=1&pageSize=5");

            var body = await res.Content.ReadAsStringAsync();
            Assert.True(res.IsSuccessStatusCode, $"Status={res.StatusCode}, Body={body}");
            Assert.Contains("items", body);
        }

        [Fact]
        public async Task Books_Post_ShouldReturn403_ForMember()
        {
            var token = await LoginAndGetTokenAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                title = "New Book",
                isbn = "999-TEST-ISBN",
                authorId = 1,
                availableCopies = 1,
                totalCopies = 1
            };

            var res = await _client.PostAsJsonAsync("/api/v1/books", payload);
            var body = await res.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }
    }
}