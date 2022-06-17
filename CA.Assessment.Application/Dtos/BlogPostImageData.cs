namespace CA.Assessment.Application.Dtos;

public sealed class BlogPostImageData
{
    public string Mime { get; }

    public Stream ImageStream { get; }

    public BlogPostImageData(string mime, Stream imageStream)
    {
        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
        ImageStream = imageStream ?? throw new ArgumentNullException(nameof(imageStream));
    }
}