using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BuildingBlocks.Exceptions;


namespace BuildingBlocks.Exceptions.Handler
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred while processing the request. Request path: {RequestPath}, HTTP method: {HttpMethod}", httpContext.Request.Path, httpContext.Request.Method);

            var (statusCode, message) = exception switch
            {
                BaseDomainException domainException => (domainException.StatusCode, domainException.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var response = new { error = message };
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken);

            return true;
        }
    }
}