using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
namespace BankingSystem.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(exception,
            "Unhandled exception = {Message}",exception.Message);

            var response = new
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Message = "Unknow error Please try  again later"
            };

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/jason";

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                cancellationToken
            );

            return true;
        }
    }
}