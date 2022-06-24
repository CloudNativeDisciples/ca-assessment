namespace CA.Assessment.Application.Requests;

public sealed class SearchBlogPost
{
    public string? Category { get; }

    public IEnumerable<string>? Tags { get; }

    public string? Title { get; }

    public bool None => string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Category) && Tags is null;

    public SearchBlogPost(string? category, IEnumerable<string>? tags, string? title)
    {
        Title = title;
        Category = category;
        Tags = tags;
    }
}
