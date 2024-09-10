using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.API.ExceptionHandle
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var statusCode = context.Exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,

                ValidationException => StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(new
            {
                error = context.Exception.Message,
                statusCode = statusCode,
                stackTrace = context.Exception.StackTrace
            })
            {
                StatusCode = statusCode
            };
        }

    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}
