using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(ReadOnly = true, Nullable = false, Title = "Blog Post Search Result")]
public sealed class BlogPostSummaryDto
{
    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post UUID")]
    public Guid Identity { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Title")]
    public string Title { get; }

    public BlogPostSummaryDto(Guid identity, string title)
    {
        Identity = identity;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }
}
