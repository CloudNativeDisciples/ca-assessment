using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(WriteOnly = true, Nullable = false, Title = "Blog Post Search Filters")]
public sealed class SearchBlogPostFiltersDto
{
    [SwaggerSchema(WriteOnly = true, Nullable = true, Description = "Category to match")]
    public string? Category { get; set; }

    [SwaggerSchema(WriteOnly = true, Nullable = true, Description = "Tags to match")]
    public IEnumerable<string>? Tags { get; set; }

    [SwaggerSchema(WriteOnly = true, Nullable = true, Description = "Title to match")]
    public string? Title { get; set; }
}
