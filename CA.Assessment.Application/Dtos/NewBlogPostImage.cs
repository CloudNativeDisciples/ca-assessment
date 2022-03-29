namespace CA.Assessment.Application.Dtos;

public sealed class NewBlogPostImage
{
    public NewBlogPostImage(string name, string mime, byte[] content)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public string Name { get; }

    public string Mime { get; }

    public byte[] Content { get; }
}
