namespace CA.Assessment.Application.Responses;

public sealed class BlogPostSummary
{
    public Guid Identity { get; }

    public string Title { get; }

    public BlogPostSummary(Guid identity, string title)
    {
        Identity = identity;

        Title = title ?? throw new ArgumentNullException(nameof(title));
    }
}
