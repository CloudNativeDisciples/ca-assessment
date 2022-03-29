namespace CA.Assessment.Application.Dtos;

public sealed class BlogPostSummary
{
    public Guid Identity { get; }
    public string Title { get; }

    public BlogPostSummary(Guid identity, string title)
    {
        Identity = identity;
        Title = title;
    }
}
