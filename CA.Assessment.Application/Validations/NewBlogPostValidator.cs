using CA.Assessment.Application.Dtos;
using FluentValidation;

namespace CA.Assessment.Application.Validations;

public sealed class NewBlogPostValidator : AbstractValidator<NewBlogPost>
{
    public NewBlogPostValidator()
    {
        RuleFor(nbp => nbp.Content).NotEmpty().MaximumLength(1024)
            .WithErrorCode(ValidationErrorCodes.BLOG_POST_TOO_LONG);

        RuleFor(nbp => nbp.Author).NotEmpty().WithErrorCode(ValidationErrorCodes.NO_AUTHOR_SPECIFIED_ON_BLOG_POST);

        RuleFor(nbp => nbp.Title).NotEmpty().WithErrorCode(ValidationErrorCodes.NO_TITLE_SPECIFIED_ON_BLOG_POST);

        RuleFor(nbp => nbp.Category).NotEmpty().WithErrorCode(ValidationErrorCodes.NO_CATEGORY_SPECIFIED_ON_BLOG_POST);

        RuleFor(nbp => nbp.Tags).ForEach(t => t.NotEmpty().WithErrorCode(ValidationErrorCodes.BLOG_POST_TAG_IS_EMPTY))
            .WithErrorCode(ValidationErrorCodes.NO_TAGS_SPECIFIED_ON_BLOG_POST);
    }
}
