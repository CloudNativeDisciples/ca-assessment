using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Database.Sqlite.Mappers;

internal sealed class ImageRowsMapper
{
    public Task<Image> MapOneAsync(ImageRow imageRow)
    {
        if (imageRow is null)
        {
            throw new ArgumentNullException(nameof(imageRow));
        }

        var imageIdentity = Guid.Parse(imageRow.Id);

        var image = new Image(imageIdentity, imageRow.Mime, imageRow.Name);

        return Task.FromResult(image);
    }
}