using CA.Assessment.Application.Dtos;
using FluentValidation;

namespace CA.Assessment.Application.Validations;

public class UpdateBlogPostValidator : AbstractValidator<UpdateBlogPost>
{
    public UpdateBlogPostValidator()
    {
        RuleFor(ubp => ubp.Content).MaximumLength(1024)
            .WithErrorCode(ValidationErrorCodes.BLOG_POST_TOO_LONG);
    }
}
