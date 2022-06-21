namespace CA.Assessment.Model;

public sealed class BlogPostImage
{
    public Guid Identity { get; }

    public string Mime { get; }

    public string Name { get; }

    public BlogPostImage(Guid identity, string mime, string name)
    {
        Identity = identity;

        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
