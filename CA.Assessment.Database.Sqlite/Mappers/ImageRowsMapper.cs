using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Model;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal static class ImageRowsMapper
{
    public static Task<BlogPostImage> MapOneAsync(ImageDbRow imageDbRow)
    {
        if (imageDbRow is null)
        {
            throw new ArgumentNullException(nameof(imageDbRow));
        }

        var imageIdentity = Guid.Parse(imageDbRow.Id);

        var image = new BlogPostImage(imageIdentity, imageDbRow.Mime, imageDbRow.Name);

        return Task.FromResult(image);
    }
}
