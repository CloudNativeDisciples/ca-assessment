using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(ReadOnly = true, Title = "Blog Post Details")]
public sealed class BlogPostDetailsDto
{
    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post UUID")]
    public Guid Identity { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Author")]
    public string Author { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Content")]
    public string Content { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Title")]
    public string Title { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Iamge")]
    public Guid Image { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Category")]
    public CategoryDetailsDto Category { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Blog Post Tags")]
    public IEnumerable<TagDetailsDto> Tags { get; }

    public BlogPostDetailsDto(
        Guid identity,
        string author,
        string content,
        string title,
        Guid image,
        CategoryDetailsDto category,
        IEnumerable<TagDetailsDto> tags)
    {
        Identity = identity;
        Image = image;

        Author = author ?? throw new ArgumentNullException(nameof(author));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Tags = tags ?? throw new ArgumentNullException(nameof(tags));
    }
}
