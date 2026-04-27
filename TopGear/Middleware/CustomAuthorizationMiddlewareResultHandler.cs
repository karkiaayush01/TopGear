using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace TopGear.Middleware;

/*
 * This customer authorization middleware result handler is invoked when endpoints with [Authorize] fail during the authentication process itself.
 * Since the Authorize error only throw a 401 and 403 without any message, this handler catches that error and sends an appropriate JSON response.
 * This makes it easier for users to understand why the API failed.
 * Documentation Referred: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/customizingauthorizationmiddlewareresponse?view=aspnetcore-10.0
*/

public class CustomAuthorizationMiddlewareResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded)
        {
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(
                    "{\"message\": \"You aren't authorized to perform this operation\"}"
                );
                return;
            }

            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(
                    "{\"message\": \"You need to log in first\"}"
                );
                return;
            }
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
