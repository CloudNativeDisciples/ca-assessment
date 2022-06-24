namespace CA.Assessment.Application.Requests;

public sealed class UpdateBlogPost
{
    public Guid Id { get; }

    public string? Title { get; }

    public string? Content { get; }

    public string? Author { get; }

    public string? Category { get; }

    public IEnumerable<string>? Tags { get; }

    public UpdateBlogPost(Guid id, string? title, string? content, string? author, string? category, IEnumerable<string>? tags)
    {
        Id = id;
        Title = title;
        Content = content;
        Author = author;
        Category = category;
        Tags = tags;
    }
}
