using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((ctx, config) =>
            {
                var tempDbPath = Path.Combine(Path.GetTempPath(), $"lms_test_{Guid.NewGuid():N}.db");

                var dict = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = $"Data Source={tempDbPath}",


                    ["Jwt:Issuer"] = "LibraryManagementSystem",
                    ["Jwt:Audience"] = "LibraryManagementSystem.ApiClients",
                    ["Jwt:Key"] = "CHANGE_ME_TO_A_32+_CHAR_RANDOM_SECRET_KEY",
                    ["Jwt:ExpiresMinutes"] = "120"
                };

                config.AddInMemoryCollection(dict);
            });
        }
    }
}