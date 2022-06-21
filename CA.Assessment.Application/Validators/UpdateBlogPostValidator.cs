using CA.Assessment.Application.Requests;
using FluentValidation;

namespace CA.Assessment.Application.Validators;

public sealed class UpdateBlogPostValidator : AbstractValidator<UpdateBlogPost>
{
    public UpdateBlogPostValidator()
    {
        RuleFor(ubp => ubp.Content).MaximumLength(1024)
            .WithErrorCode(ValidationErrorCodes.BLOG_POST_TOO_LONG);
    }
}
