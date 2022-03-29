namespace CA.Assessment.Domain.Anemic;

public sealed class Image
{
    public Image(Guid identity, byte[] content, string mime, string name)
    {
        Identity = identity;
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Guid Identity { get; }

    public byte[] Content { get; }

    public string Mime { get; }

    public string Name { get; }
}
