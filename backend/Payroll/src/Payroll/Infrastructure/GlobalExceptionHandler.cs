using Microsoft.AspNetCore.Diagnostics;

namespace Payroll.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger; // TODO: Add Serilog?

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
        _logger.LogError(
            exception,
            "Unhandled exception at {Method} {Path}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier
        );

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var problem = new
        {
            title = "An unexpected error occurred",
            status = 500,
            traceId = httpContext.TraceIdentifier, // safe to expose
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
