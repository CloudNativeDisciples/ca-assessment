using System.Net;
using CA.Assessment.Domain.Anemic.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CA.Assessment.WebAPI.Filters;

public sealed class DomainExceptionFilters : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var maybeResult = context.Exception switch
        {
            BlogPostImageNotFoundException => new NotFoundResult(),
            BlogPostNotFoundException => new NotFoundResult(),
            UnauthorizedBlogPostDeletionException => new UnauthorizedResult(),
            ForbiddenBlogPostDeletionException => new ForbidResult(),
            ValidationException validationEx => GenerateValidationResult(validationEx),
            _ => null
        };

        if (maybeResult is null)
        {
            return;
        }

        context.Result = maybeResult;
    }

    private IActionResult GenerateValidationResult(ValidationException validationEx)
    {
        if (validationEx is null) throw new ArgumentNullException(nameof(validationEx));

        var errorCodes = validationEx.Errors
            .Select(v => new { Code = v.ErrorCode, Property = v.PropertyName })
            .ToList();

        var jsonResult = new JsonResult(errorCodes) { StatusCode = (int) HttpStatusCode.BadRequest };

        return jsonResult;
    }
}
