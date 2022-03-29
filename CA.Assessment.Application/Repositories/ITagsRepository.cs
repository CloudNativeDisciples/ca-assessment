using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Repositories;

public interface ITagsRepository
{
    Task SaveAsync(Tag tagToSave);

    Task<IEnumerable<Tag>> GetTagsByNameAsync(IEnumerable<string> tagNames);

    Task<IEnumerable<Tag>> GetManyAsync(IEnumerable<Guid> tagIds);

    Task AddTagsToBlogPostAsync(Guid blogPostId, IEnumerable<Tag> tagsToAdd);

    Task RemoveTagsToBlogPostAsync(Guid blogPostId, IEnumerable<Tag> tagsToRemove);
}
