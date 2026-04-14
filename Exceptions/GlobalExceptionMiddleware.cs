using LibraryManagementSystem.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LibraryManagementSystem.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DuplicateIsbnException ex)
            {
                _logger.LogWarning(ex, "Duplicate ISBN detected: {ISBN}", ex.ISBN);
                await WriteErrorAsync(context, 409, "Duplicate ISBN Error",
                    $"A book with ISBN '{ex.ISBN}' already exists.", ex);
            }
            catch (BookNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book not found: {BookId}", ex.BookId);
                await WriteErrorAsync(context, 404, "Book Not Found",
                    $"Book ID {ex.BookId} not found.", ex);
            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer not found: {CustomerId}", ex.CustomerId);
                await WriteErrorAsync(context, 404, "Customer Not Found",
                    $"Customer with ID {ex.CustomerId} not found.", ex);
            }
            catch (UnauthorizedLoanException ex)
            {
                _logger.LogWarning(ex, "Unauthorized loan attempt: {CustomerId} - {Reason}", ex.CustomerId, ex.Reason);
                await WriteErrorAsync(context, 403, "Loan Not Authorized", ex.Reason, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await WriteErrorAsync(context, 500, "System Error",
                    "An unexpected error occurred while processing your request.", ex);
            }
        }

        private static bool IsApiRequest(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
                return true;

            var accept = context.Request.Headers.Accept.ToString();
            return accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, string title, string message, Exception ex)
        {
            if (IsApiRequest(context))
            {
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = message,
                    Instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
                return;
            }

            // HTML response (your existing style)
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "text/html";

            var alertType = statusCode >= 500 ? "danger" : statusCode == 404 ? "danger" : "warning";
            var icon = statusCode == 404 ? "bi-search" : "bi-exclamation-triangle-fill";

            var html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='utf-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
  <title>{title} - Library Management System</title>
  <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
  <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css'>
</head>
<body class='bg-light'>
  <div class='container'>
    <div class='row justify-content-center align-items-center' style='min-height: 100vh;'>
      <div class='col-md-6'>
        <div class='card shadow-lg border-0 rounded-4'>
          <div class='card-body p-5 text-center'>
            <i class='bi {icon} text-{alertType}' style='font-size: 5rem;'></i>
            <h1 class='display-4 fw-bold mt-4 mb-3'>{title}</h1>
            <p class='lead text-muted mb-4'>{message}</p>
            <div class='alert alert-{alertType} d-inline-block mb-4' role='alert'>
              <strong>Error Code:</strong> {statusCode}
            </div>
            <div class='d-grid gap-2'>
              <a href='/' class='btn btn-primary btn-lg'>
                <i class='bi bi-house-door me-2'></i>Return to Home
              </a>
              <button onclick='history.back()' class='btn btn-outline-secondary btn-lg'>
                <i class='bi bi-arrow-left me-2'></i>Go Back
              </button>
            </div>
          </div>
        </div>
        <div class='text-center mt-3'>
          <small class='text-muted'>
            <i class='bi bi-clock me-1'></i>Occurred at {DateTime.Now:yyyy-MM-dd HH:mm:ss}
          </small>
        </div>
      </div>
    </div>
  </div>
</body>
</html>";

            await context.Response.WriteAsync(html);
        }
    }
}