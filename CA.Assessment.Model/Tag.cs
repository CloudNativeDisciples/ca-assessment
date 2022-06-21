namespace CA.Assessment.Model;

public sealed class Tag
{
    public Guid Identity { get; }

    public string Name { get; }

    public Tag(Guid identity, string name)
    {
        Identity = identity;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
