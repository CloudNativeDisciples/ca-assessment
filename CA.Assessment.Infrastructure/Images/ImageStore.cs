using CA.Assessment.Infrastructure.Extensions;

namespace CA.Assessment.Infrastructure.Images;

internal sealed class ImageStore
{
    private readonly ImageStoreFolder imageStoreFolder;

    public ImageStore(ImageStoreFolder imageStoreFolder)
    {
        this.imageStoreFolder = imageStoreFolder ?? throw new ArgumentNullException(nameof(imageStoreFolder));
    }

    public async Task SaveImageAsync(Guid imageRef, byte[] imageData)
    {
        if (imageData is null) throw new ArgumentNullException(nameof(imageData));

        if (Directory.Exists(imageStoreFolder.Value) is false)
        {
            Directory.CreateDirectory(imageStoreFolder.Value);
        }

        var fileName = GenerateFileName(imageRef);

        var filePath = Path.Combine(imageStoreFolder.Value, fileName);

        await using var fileStream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write);
        await using var binaryWriter = new BinaryWriter(fileStream);

        binaryWriter.Write(imageData);
    }

    public async Task<byte[]> GetImageAsync(Guid imageRef)
    {
        if (imageRef == Guid.Empty)
        {
            //TODO: Maybe a default image would have been more appropriate

            return Array.Empty<byte>();
        }

        var fileName = GenerateFileName(imageRef);

        var filePath = Path.Combine(imageStoreFolder.Value, fileName);

        var fileInfo = new FileInfo(filePath);

        if (fileInfo.Exists is false)
        {
            //TODO: Throwing here is an exaggeration here, probably logging a Warning would be appropriate

            return Array.Empty<byte>();
        }

        await using var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read);

        using var binaryReader = new BinaryReader(fileStream);

        return await binaryReader.ReadAllAsync();
    }

    private static string GenerateFileName(Guid imageRef)
    {
        return $"CA_assessment_img_{imageRef:N}";
    }
}
