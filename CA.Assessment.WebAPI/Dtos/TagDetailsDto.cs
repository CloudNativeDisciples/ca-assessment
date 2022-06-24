using Swashbuckle.AspNetCore.Annotations;

namespace CA.Assessment.WebAPI.Dtos;

[SwaggerSchema(ReadOnly = true, Title = "Tag")]
public sealed class TagDetailsDto
{
    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Tag UUID")]
    public Guid Identity { get; }

    [SwaggerSchema(ReadOnly = true, Nullable = false, Description = "Tag Name")]
    public string Name { get; }

    public TagDetailsDto(Guid identity, string name)
    {
        Identity = identity;

        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
