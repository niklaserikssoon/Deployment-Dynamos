using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace workoutapp_API.ExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request was canceled");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "Request was canceled."
                }));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "An error occurred while processing the HTTP request."
                }));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error");
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = "An error occurred while updating the database."
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware."
            }));
        }
    }
}
