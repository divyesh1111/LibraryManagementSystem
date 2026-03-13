using LibraryManagementSystem.Data;
using LibraryManagementSystem.Middleware;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o => {
    o.User.RequireUniqueEmail = true;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireDigit = true;
    o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = false;
    o.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<LibraryDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(o => {
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/AccessDenied";
    o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
});

builder.Services.AddAuthentication()
    .AddGoogle("Google", o => {
        o.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        o.SaveTokens = true;
    })
    .AddOAuth("GitHub", o => {
        o.ClientId = builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
        o.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
        o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        o.TokenEndpoint = "https://github.com/login/oauth/access_token";
        o.UserInformationEndpoint = "https://api.github.com/user";
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email");
        o.SaveTokens = true;
        o.Events = new OAuthEvents {
            OnCreatingTicket = async context => {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                if(payload.RootElement.TryGetProperty("id", out var id)) 
                    context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
                if(payload.RootElement.TryGetProperty("login", out var login)) 
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Name, login.GetString() ?? ""));
                if(payload.RootElement.TryGetProperty("email", out var email) && email.ValueKind == JsonValueKind.String) 
                    context.Identity?.AddClaim(new Claim(ClaimTypes.Email, email.GetString() ?? ""));
            }
        };
    });

builder.Services.AddAuthorization(o => {
    o.AddPolicy("ManageLibrary", p => p.RequireRole("Admin", "Librarian"));
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Global Exception Handling Middleware (MUST be first)
app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
    await IdentitySeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
    await DbSeeder.SeedAsync(db);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();