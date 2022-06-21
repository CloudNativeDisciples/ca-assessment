using CA.Assessment.Application.Responses;
using CA.Assessment.Database.Sqlite.Rows;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal static class BlogPostSummaryRowMapper
{
    private static BlogPostSummary MapOne(BlogPostSummaryDbRow blogPostSummaryDbRow)
    {
        if (blogPostSummaryDbRow is null)
        {
            throw new ArgumentNullException(nameof(blogPostSummaryDbRow));
        }

        return new BlogPostSummary(Guid.Parse(blogPostSummaryDbRow.Id), blogPostSummaryDbRow.Title);
    }

    public static IEnumerable<BlogPostSummary> MapMany(IEnumerable<BlogPostSummaryDbRow> blogPostSummaryRows)
    {
        if (blogPostSummaryRows is null)
        {
            throw new ArgumentNullException(nameof(blogPostSummaryRows));
        }

        return blogPostSummaryRows.Select(MapOne).ToList();
    }
}
