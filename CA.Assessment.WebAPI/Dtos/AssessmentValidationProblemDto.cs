using FluentValidation.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(ReadOnly = true, Title = "Validation Problem")]
public sealed class AssessmentValidationProblem
{
    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Error Code")]
    public string ErrorCode { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Error Message")]
    public string ErrorMessage { get; }

    private AssessmentValidationProblem(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }

    public static AssessmentValidationProblem FromValidationFailure(ValidationFailure failure)
    {
        if (failure is null)
        {
            throw new ArgumentNullException(nameof(failure));
        }

        return new AssessmentValidationProblem(failure.ErrorCode, failure.ErrorMessage);
    }
}
