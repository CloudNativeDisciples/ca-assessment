namespace CA.Assessment.Application.Dtos;

public sealed class TagDetails
{
    public TagDetails(Guid identity, string name)
    {
        Identity = identity;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Guid Identity { get; }

    public string Name { get; }
}
