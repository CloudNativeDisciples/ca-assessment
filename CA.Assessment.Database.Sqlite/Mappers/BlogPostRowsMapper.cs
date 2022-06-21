using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Model;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal static class BlogPostRowsMapper
{
    public static BlogPost MapOne(BlogPostDbRow blogPostDbRow, IEnumerable<BlogPostToTagDbRow> blogPostToTagRows)
    {
        if (blogPostDbRow is null)
        {
            throw new ArgumentNullException(nameof(blogPostDbRow));
        }

        if (blogPostToTagRows is null)
        {
            throw new ArgumentNullException(nameof(blogPostToTagRows));
        }

        var tags = blogPostToTagRows
            .Select(t => Guid.Parse(t.TagId))
            .ToList();

        return new BlogPost(Guid.Parse(blogPostDbRow.Id), blogPostDbRow.Title, blogPostDbRow.Author, blogPostDbRow.Content,
            Guid.Parse(blogPostDbRow.ImageId), tags, Guid.Parse(blogPostDbRow.CategoryId));
    }
}
