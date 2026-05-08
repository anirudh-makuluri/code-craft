using System.Net;
using System.Text.Json;

namespace api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var payload = JsonSerializer.Serialize(new { error = "Internal server error" });
            await context.Response.WriteAsync(payload);
        }
    }
}
