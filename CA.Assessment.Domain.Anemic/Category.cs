namespace CA.Assessment.Domain.Anemic;

public sealed class Category
{
    public Category(Guid identity, string name)
    {
        Identity = identity;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Guid Identity { get; }

    public string Name { get; }
}
