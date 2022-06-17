using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Rows;

namespace CA.Assessment.Infrastructure.Mappers;

internal sealed class ImageRowsMapper
{
    public Task<Image> MapOneAsync(ImageRow imageRow)
    {
        if (imageRow is null) throw new ArgumentNullException(nameof(imageRow));

        var imageIdentity = Guid.Parse(imageRow.Id);

        var image = new Image(imageIdentity, imageRow.Mime, imageRow.Name);

        return Task.FromResult(image);
    }
}