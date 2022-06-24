using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(ReadOnly = true, Title = "Category")]
public sealed class CategoryDetailsDto
{
    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Category UUID")]
    public Guid Identity { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Category Name")]
    public string Name { get; }

    public CategoryDetailsDto(Guid identity, string name)
    {
        Identity = identity;

        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
