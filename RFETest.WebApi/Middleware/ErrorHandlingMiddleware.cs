using Microsoft.AspNetCore.Mvc;
using RFETest.Core.Values;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;

namespace RFETest.WebApi.Middleware
{
    /// <summary>
    /// Error handling middleware - catches exceptions, and converts them to a response with ProblemDetails in body
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problemDetails = GetProblemDetails(exception);

            context.Response.StatusCode = problemDetails.Status ?? throw new InvalidOperationException();

            await SerializeResponse(problemDetails, context.Request, context.Response);
        }

        protected virtual async Task SerializeResponse(ProblemDetails problemDetails, HttpRequest request, HttpResponse response)
        {
            response.ContentType = "application/json";
            await response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(problemDetails));
        }

        protected virtual ProblemDetails GetProblemDetails(Exception exception)
        {
            switch (exception)
            {
                case ComparisonIDIncompleteException e:
                    return new ProblemDetails
                    {
                        Type = "Value(s) missing",
                        Status = (int)HttpStatusCode.NotFound,
                        Detail = e.Message
                    };

                case ValidationException e:
                    var validationErrors = e.ValidationResult.MemberNames.ToDictionary(x => x, x => new List<string> { e.ValidationResult.ErrorMessage ?? $"Error on member {x}" }.ToArray());

                    return new ValidationProblemDetails(validationErrors)
                    {
                        Type = "Validation failed",
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = e.ValidationResult.ErrorMessage
                    };

                default:
                    return new ProblemDetails
                    {
                        Type = "Server error",
                        Status = (int)HttpStatusCode.InternalServerError,
                    };
            }
        }
    }
}
