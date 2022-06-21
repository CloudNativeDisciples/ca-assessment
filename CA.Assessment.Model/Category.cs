namespace CA.Assessment.Model;

public sealed class Category
{
    public Guid Identity { get; }

    public string Name { get; }

    public Category(Guid identity, string name)
    {
        Identity = identity;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
