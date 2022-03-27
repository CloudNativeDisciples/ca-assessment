namespace CA.Assessment.Application.Dtos;

public sealed class NewBlogPost
{
    public NewBlogPost(string title, string content, string author, string category, IEnumerable<string> tags)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Tags = tags ?? throw new ArgumentNullException(nameof(tags));
    }

    public string Title { get; }

    public string Content { get; }

    public string Author { get; }

    public string Category { get; }

    public IEnumerable<string> Tags { get; }
}
