namespace CA.Assessment.Application.Dtos;

public sealed class CategoryDetails
{
    public CategoryDetails(Guid identity, string name)
    {
        Identity = identity;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Guid Identity { get; }

    public string Name { get; }
}
