using Microsoft.AspNetCore.Http;

namespace AdminApi.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate _next, ILogger<ExceptionHandlingMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");

            if (context.Response.HasStarted) throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");

            if (context.Response.HasStarted) throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message + " An unexpected error occurred."
            });
        }
    }
}