namespace CA.Assessment.Infrastructure.Extensions;

public static class BinaryReaderExtensions
{
    public static async Task<byte[]> ReadAllAsync(this BinaryReader binaryReader)
    {
        if (binaryReader is null) throw new ArgumentNullException(nameof(binaryReader));

        await using var memoryStream = new MemoryStream();

        var buffer = new byte[8192];

        var bytesRead = 0;

        do
        {
            bytesRead = binaryReader.Read(buffer, 0, 4096);

            if (bytesRead != 0)
            {
                await memoryStream.WriteAsync(buffer, 0, bytesRead);
            }
        }
        while (bytesRead != 0);

        return memoryStream.ToArray();
    }
}
