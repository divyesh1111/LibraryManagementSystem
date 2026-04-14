using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Api.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LibraryManagementSystem.IntegrationTests
{
    public class ReviewsWorkflowTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ReviewsWorkflowTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
        }

        private async Task<string> LoginTokenAsync(string email, string password)
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
            return token!.AccessToken;
        }

        [Fact]
        public async Task Review_Workflow_ShouldSucceed()
        {
            // Admin token
            var adminToken = await LoginTokenAsync("admin@library.local", "Admin123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            // Create customer with member email (needed for review rule)
            var createCustomerPayload = new
            {
                firstName = "Member",
                lastName = "User",
                email = "member@library.local",
                isActiveMember = true
            };

            var createCustomerRes = await _client.PostAsJsonAsync("/api/v1/customers", createCustomerPayload);

            var createCustomerBody = await createCustomerRes.Content.ReadAsStringAsync();
            Assert.True(createCustomerRes.IsSuccessStatusCode, $"Status={createCustomerRes.StatusCode}, Body={createCustomerBody}");

            var createdCustomer = await createCustomerRes.Content.ReadFromJsonAsync<CustomerResponse>();
            Assert.NotNull(createdCustomer);

            // Create loan for bookId=1
            var loanPayload = new
            {
                bookId = 1,
                customerId = createdCustomer!.id,
                loanDate = DateTime.UtcNow,
                dueDate = DateTime.UtcNow.AddDays(7)
            };

            var createLoanRes = await _client.PostAsJsonAsync("/api/v1/loans", loanPayload);
            var createLoanBody = await createLoanRes.Content.ReadAsStringAsync();
            Assert.True(createLoanRes.IsSuccessStatusCode, $"Status={createLoanRes.StatusCode}, Body={createLoanBody}");

            // Member creates review
            var memberToken = await LoginTokenAsync("member@library.local", "Member123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", memberToken);

            var reviewPayload = new
            {
                bookId = 1,
                customerId = createdCustomer.id,
                rating = 5,
                title = "Great book",
                content = "This book was excellent. Highly recommended!"
            };

            var createReviewRes = await _client.PostAsJsonAsync("/api/v1/reviews", reviewPayload);
            var createReviewBody = await createReviewRes.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, createReviewRes.StatusCode);

            var createdReview = await createReviewRes.Content.ReadFromJsonAsync<ReviewResponse>();
            Assert.NotNull(createdReview);
            Assert.False(createdReview!.isApproved);

            // Librarian approves
            var librarianToken = await LoginTokenAsync("librarian@library.local", "Librarian123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", librarianToken);

            var approveRes = await _client.PostAsync($"/api/v1/reviews/{createdReview.id}/approve?approved=true", null);
            Assert.Equal(HttpStatusCode.NoContent, approveRes.StatusCode);
        }

        private class CustomerResponse
        {
            public int id { get; set; }
            public string email { get; set; } = "";
        }

        private class ReviewResponse
        {
            public int id { get; set; }
            public bool isApproved { get; set; }
        }
    }
}