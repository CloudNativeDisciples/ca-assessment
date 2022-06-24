namespace CA.Assessment.Application.Requests;

public sealed class AttachImageToBlogPost
{
    public Guid BlogPostId { get; }

    public Guid ImageId { get; }

    public string Name { get; }

    public string Mime { get; }

    public Stream ImageStream { get; }

    public AttachImageToBlogPost(Guid blogPostId, Guid imageId, string name, string mime, Stream imageStream)
    {
        BlogPostId = blogPostId;
        ImageId = imageId;

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Mime = mime ?? throw new ArgumentNullException(nameof(mime));

        ImageStream = imageStream ?? throw new ArgumentNullException(nameof(imageStream));
    }
}
