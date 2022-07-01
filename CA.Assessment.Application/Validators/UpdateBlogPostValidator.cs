using CA.Assessment.Application.Requests;
using CA.Assessment.Model;
using FluentValidation;

namespace CA.Assessment.Application.Validators;

public sealed class UpdateBlogPostValidator : AbstractValidator<UpdateBlogPost>
{
    public UpdateBlogPostValidator()
    {
        RuleFor(ubp => ubp.Content).MaximumLength(BlogPost.MAX_CONTENT_SIZE)
            .WithErrorCode(ValidationErrorCodes.BLOG_POST_TOO_LONG);
    }
}
