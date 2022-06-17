namespace CA.Assessment.Images.Stores.FileSystem;

public sealed class FileSystemImageStoreOptions
{
    public string Folder { get; set; }
    
    public bool Overwrite { get; set; }
}