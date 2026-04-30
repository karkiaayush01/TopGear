using TopGear.Application.CustomExceptions;

namespace TopGear.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger): IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    public async Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, "An error occurred");

        context.Response.ContentType = "application/json";

        int statusCode = ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;
        var response = new
        {
            Message = ex.Message ?? "",
            StatusCode = statusCode
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
