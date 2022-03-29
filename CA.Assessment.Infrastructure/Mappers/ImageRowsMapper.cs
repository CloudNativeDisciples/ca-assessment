using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Images;
using CA.Assessment.Infrastructure.Rows;

namespace CA.Assessment.Infrastructure.Mappers;

internal sealed class ImageRowsMapper
{
    private readonly ImageStore imageStore;

    public ImageRowsMapper(ImageStore imageStore)
    {
        this.imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
    }

    public async Task<Image> MapOneAsync(ImageRow imageRow)
    {
        if (imageRow is null) throw new ArgumentNullException(nameof(imageRow));

        var imageIdentity = Guid.Parse(imageRow.Id);

        var imageContent = await imageStore.GetImageAsync(imageIdentity);

        return new Image(imageIdentity, imageContent, imageRow.Mime, imageRow.Name);
    }
}
