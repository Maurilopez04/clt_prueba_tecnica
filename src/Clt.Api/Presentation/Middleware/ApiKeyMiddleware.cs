using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Clt.Api.Presentation.Middleware;

public sealed class ApiKeyMiddleware(
    RequestDelegate next,
    IConfiguration configuration)
{
    private const string HeaderName = "X-API-KEY";
    private readonly byte[] expectedApiKey = Encoding.UTF8.GetBytes(
        configuration["ApiKey"]
        ?? throw new InvalidOperationException("ApiKey configuration is required."));

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out StringValues providedValues)
            || providedValues.Count != 1
            || !IsValid(providedValues[0]))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "A valid API key is required."
            });
            return;
        }

        await next(context);
    }

    private bool IsValid(string? providedApiKey)
    {
        if (string.IsNullOrEmpty(providedApiKey))
        {
            return false;
        }

        var providedBytes = Encoding.UTF8.GetBytes(providedApiKey);
        return CryptographicOperations.FixedTimeEquals(expectedApiKey, providedBytes);
    }
}
