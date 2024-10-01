using System.Text.Json;
using Microsoft.AspNetCore.Http;
using OrderServiceQuery.Core.Resources.Common;

namespace OrderServiceQuery.Infrastructure.Middlewares
{
    public class UnhandledExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public UnhandledExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var errorResponse = new StandardResponse(false) {Message = "An unhandled exception has occurred"};
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }

    }

    
}
