using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Providers;

public sealed class TagsMaker : ITagsMaker
{
    private readonly ITagsRepository tagsRepository;

    public TagsMaker(ITagsRepository tagsRepository)
    {
        this.tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
    }

    public async Task<IEnumerable<Tag>> GetOrCreateTagsByNameAsync(IEnumerable<string> tagNames)
    {
        if (tagNames is null) throw new ArgumentNullException(nameof(tagNames));

        var existingTags = await tagsRepository.GetTagsByNameAsync(tagNames);

        var existingTagNames = existingTags.Select(t => t.Name);

        var missingTagNames = tagNames.Except(existingTagNames);

        var allTags = new List<Tag>(existingTags);

        foreach (var missingTagName in missingTagNames)
        {
            var newTag = new Tag(Guid.NewGuid(), missingTagName);

            await tagsRepository.SaveAsync(newTag);

            allTags.Add(newTag);
        }

        return allTags;
    }
}
