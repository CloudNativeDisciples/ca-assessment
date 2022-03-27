namespace CA.Assessment.Application.Dtos;

public sealed class UpdateBlogPost
{
    public UpdateBlogPost(string? title, string? content, string? author, string? category, IEnumerable<string>? tags)
    {
        Title = title;
        Content = content;
        Author = author;
        Category = category;
        Tags = tags;
    }

    public string? Title { get; }

    public string? Content { get; }

    public string? Author { get; }

    public string? Category { get; }

    public IEnumerable<string>? Tags { get; }
}
