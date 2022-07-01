namespace CA.Assessment.Images.Stores.FileSystem;

public sealed class FileSystemImagesContentStoreOptions
{
    public string? Folder { get; set; } 

    public bool Overwrite { get; set; } = true;
}
