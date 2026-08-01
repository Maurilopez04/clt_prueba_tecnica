using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Clt.Api.Presentation.Middleware;

public sealed class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate next;
    private readonly byte[] expectedUsername;
    private readonly byte[] expectedPassword;

    public SwaggerBasicAuthMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        this.next = next;

        var username = configuration["Swagger:Username"];
        var password = configuration["Swagger:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Swagger credentials are required.");
        }

        expectedUsername = Encoding.UTF8.GetBytes(username);
        expectedPassword = Encoding.UTF8.GetBytes(password);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next(context);
            return;
        }

        if (!IsAuthorized(context.Request.Headers.Authorization))
        {
            context.Response.Headers.WWWAuthenticate = "Basic realm=\"Swagger\", charset=\"UTF-8\"";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }

    private bool IsAuthorized(string? authorization)
    {
        if (!AuthenticationHeaderValue.TryParse(authorization, out var header)
            || !header.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrEmpty(header.Parameter))
        {
            return false;
        }

        try
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter));
            var separatorIndex = credentials.IndexOf(':');

            if (separatorIndex < 0)
            {
                return false;
            }

            var username = Encoding.UTF8.GetBytes(credentials[..separatorIndex]);
            var password = Encoding.UTF8.GetBytes(credentials[(separatorIndex + 1)..]);

            return CryptographicOperations.FixedTimeEquals(expectedUsername, username)
                && CryptographicOperations.FixedTimeEquals(expectedPassword, password);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
