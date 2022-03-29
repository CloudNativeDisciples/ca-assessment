namespace CA.Assessment.Application.Dtos;

public sealed class SearchBlogPostsFilters
{
    public SearchBlogPostsFilters(string? category, IEnumerable<string>? tags, string? title)
    {
        Title = title;
        Category = category;
        Tags = tags;
    }

    public string? Category { get; }

    public IEnumerable<string>? Tags { get; }

    public string? Title { get; }

    public bool None => string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Category) && Tags is null;
}
