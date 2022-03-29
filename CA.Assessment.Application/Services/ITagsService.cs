namespace CA.Assessment.Application.Services;

public interface ITagsService
{
    Task TagAsync(Guid blogPostId, IEnumerable<string> tags);

    Task UntagAsync(Guid blogPostId, IEnumerable<string> tags);
}
