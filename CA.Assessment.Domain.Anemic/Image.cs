namespace CA.Assessment.Domain.Anemic;

public sealed class Image
{
    public Guid Identity { get; }

    public string Mime { get; }

    public string Name { get; }

    public Image(Guid identity, string mime, string name)
    {
        Identity = identity;

        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}