using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(WriteOnly = true, Title = "Update Blog Post Data")]
public class UpdateBlogPostDto
{
    [SwaggerSchema("Title of the Blog Post", Nullable = true, WriteOnly = true)]
    public string? Title { get; set; }

    [MaxLength(1024)]
    [SwaggerSchema("Content of the Blog Post", Nullable = true, WriteOnly = true)]
    public string? Content { get; set; }

    [SwaggerSchema("Author of the Blog Post", Nullable = true, WriteOnly = true)]
    public string? Author { get; set; }

    [SwaggerSchema("Category of the Blog Post", Nullable = true, WriteOnly = true)]
    public string? Category { get; set; }

    [SwaggerSchema("Tags of the Blog Post", Nullable = true, WriteOnly = true)]
    public IEnumerable<string>? Tags { get; set; }
}
