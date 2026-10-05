using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GridPulse.Infrastructure.Middlewares;

public class ExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred while processing the request.");
            await HandleException(ex, httpContext);
        }
    }

    private async Task HandleException(Exception ex, HttpContext httpContext)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = ex.Message
        };
        var jsonResponse = JsonSerializer.Serialize(response);
        await httpContext.Response.WriteAsync(jsonResponse);
    }
}
