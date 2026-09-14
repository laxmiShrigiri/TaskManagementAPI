using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TaskManagementAPI.Exceptions
{
    public sealed class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger ) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occurred on path {Path}: {Message}",
                        httpContext.Request.Path, exception.Message);

            var (statusCode, title) = exception switch
            {
                AppException appEx => (appEx.StatusCode, appEx.GetType().Name.Replace("Exception", string.Empty)),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Not Found"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unathorized"),
                _=> (HttpStatusCode.InternalServerError,"Internal Server Error") 
            };
            httpContext.Response.StatusCode = (int)statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext 
            { 
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Title = title,
                    Status = (int)statusCode,
                    Detail = exception.Message,
                    Instance = httpContext.Request.Path
                },
                Exception = exception
            });
        }

    }
}
