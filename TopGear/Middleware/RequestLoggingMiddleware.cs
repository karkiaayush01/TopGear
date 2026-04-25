namespace TopGear.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

        await next(context);

        logger.LogInformation("Finished handling request {Path}", context.Request.Path);
    }
}
