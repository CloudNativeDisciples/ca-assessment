namespace CA.Assessment.Application.Services;

public interface IImageContent
{
    public Guid Ref { get; }

    public Stream OpenReadStream();
}
