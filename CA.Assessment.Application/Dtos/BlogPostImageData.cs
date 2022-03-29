namespace CA.Assessment.Application.Dtos;

public sealed class BlogPostImageData
{
    public BlogPostImageData(byte[] content, string mime)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
    }

    public byte[] Content { get; }

    public string Mime { get; }
}
