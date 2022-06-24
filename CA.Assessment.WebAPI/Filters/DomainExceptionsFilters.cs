using CA.Assessment.Model.Exceptions;
using CA.Assessment.WebAPI.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CA.Assessment.WebAPI.Filters;

public sealed class DomainExceptionFilters : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

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

    private static IActionResult GenerateValidationResult(ValidationException validationEx)
    {
        if (validationEx is null)
        {
            throw new ArgumentNullException(nameof(validationEx));
        }

        var errorCodes = validationEx.Errors
            .Select(AssessmentValidationProblem.FromValidationFailure)
            .ToList();

        var jsonResult = new JsonResult(errorCodes) { StatusCode = StatusCodes.Status422UnprocessableEntity };

        return jsonResult;
    }
}
