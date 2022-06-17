namespace CA.Assessment.Application.Dtos;

public sealed class NewBlogPostImage
{
    public string Name { get; }

    public string Mime { get; }

    public Stream ImageStream { get; }

    public NewBlogPostImage(string name, string mime, Stream imageStream)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Mime = mime ?? throw new ArgumentNullException(nameof(mime));
        
        ImageStream = imageStream ?? throw new ArgumentNullException(nameof(imageStream));
    }
}