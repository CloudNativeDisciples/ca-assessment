namespace CA.Assessment.Application.Requests;

public sealed class SearchBlogPostsFilters
{
    public string? Category { get; }

    public IEnumerable<string>? Tags { get; }

    public string? Title { get; }

    public bool None => string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Category) && Tags is null;

    public SearchBlogPostsFilters(string? category, IEnumerable<string>? tags, string? title)
    {
        Title = title;
        Category = category;
        Tags = tags;
    }
}
