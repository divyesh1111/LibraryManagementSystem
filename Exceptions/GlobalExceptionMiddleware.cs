using LibraryManagementSystem.Exceptions;

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
                await WriteErrorPageAsync(context, 409, "Duplicate ISBN Error", 
                    $"A book with ISBN '{ex.ISBN}' already exists in our library system.", 
                    "warning", "bi-exclamation-triangle-fill");
            }
            catch (BookNotFoundException ex)
            {
                _logger.LogWarning(ex, "Book not found: {BookId}", ex.BookId);
                await WriteErrorPageAsync(context, 404, "Book Not Found", 
                    $"The book you're looking for (ID: {ex.BookId}) could not be found in our collection.", 
                    "danger", "bi-book-fill");
            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer not found: {CustomerId}", ex.CustomerId);
                await WriteErrorPageAsync(context, 404, "Customer Not Found", 
                    $"Customer with ID {ex.CustomerId} does not exist in our system.", 
                    "danger", "bi-person-x-fill");
            }
            catch (UnauthorizedLoanException ex)
            {
                _logger.LogWarning(ex, "Unauthorized loan attempt: {CustomerId} - {Reason}", ex.CustomerId, ex.Reason);
                await WriteErrorPageAsync(context, 403, "Loan Not Authorized", 
                    $"Unable to process book loan: {ex.Reason}", 
                    "warning", "bi-shield-exclamation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await WriteErrorPageAsync(context, 500, "System Error", 
                    "An unexpected error occurred while processing your request. Our team has been notified.", 
                    "danger", "bi-exclamation-octagon-fill");
            }
        }

        private static async Task WriteErrorPageAsync(HttpContext context, int statusCode, string title, string message, string alertType, string icon)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "text/html";

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