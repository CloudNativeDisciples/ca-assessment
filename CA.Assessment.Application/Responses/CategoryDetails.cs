namespace CA.Assessment.Application.Responses;

public sealed class CategoryDetails
{
    public Guid Identity { get; }

    public string Name { get; }

    public CategoryDetails(Guid identity, string name)
    {
        Identity = identity;

        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
