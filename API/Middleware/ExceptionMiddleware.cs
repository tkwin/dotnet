using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next,
    ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = environment.IsDevelopment() ?
                new ApiException(httpContext.Response.StatusCode, ex.Message, ex.StackTrace) : new ApiException(httpContext.Response.StatusCode, ex.Message, "Internal server error");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            await httpContext.Response.WriteAsync(json);
        }
    }
}
