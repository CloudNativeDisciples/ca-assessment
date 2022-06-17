namespace CA.Assessment.Images;

public interface IImageFromStore
{
    public Guid Ref { get; }

    public Stream OpenReadStream();
}