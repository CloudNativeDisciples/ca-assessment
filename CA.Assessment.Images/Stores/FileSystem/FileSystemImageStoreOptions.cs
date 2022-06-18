namespace CA.Assessment.Images.Stores.FileSystem;

public sealed class FileSystemImageStoreOptions
{
    public string Folder { get; set; } = "/tmp/ca_assessment";

    public bool Overwrite { get; set; } = true;
}