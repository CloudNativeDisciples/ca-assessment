namespace CA.Assessment.Tests.Helpers;

public static class StreamHelpers
{
    public static async Task<byte[]> ReadBytesFromStreamAsync(Stream someStream, int bytesToRead)
    {
        if (someStream is null)
        {
            throw new ArgumentNullException(nameof(someStream));
        }

        var buffer = new byte[bytesToRead];

        _ = await someStream.ReadAsync(buffer.AsMemory(0, bytesToRead));

        return buffer;
    }

    public static async Task<MemoryStream> NewBytesStreamAsync(byte[] data)
    {
        var inMemoryStream = new MemoryStream();

        await inMemoryStream.WriteAsync(data);
        await inMemoryStream.FlushAsync();

        inMemoryStream.Seek(0L, SeekOrigin.Begin);

        return inMemoryStream;
    }
}
