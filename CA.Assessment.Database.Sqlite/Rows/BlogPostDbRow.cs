namespace CA.Assessment.Database.Sqlite.Rows;

internal sealed class BlogPostDbRow
{
    public string Id { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string ImageId { get; set; } = null!;

    public string CategoryId { get; set; } = null!;
}
