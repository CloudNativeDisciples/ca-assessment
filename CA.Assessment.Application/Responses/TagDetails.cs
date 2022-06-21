namespace CA.Assessment.Application.Responses;

public sealed class TagDetails
{
    public Guid Identity { get; }

    public string Name { get; }

    public TagDetails(Guid identity, string name)
    {
        Identity = identity;

        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
