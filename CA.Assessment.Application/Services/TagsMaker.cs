using CA.Assessment.Application.Repositories;
using CA.Assessment.Model;

namespace CA.Assessment.Application.Services;

public sealed class TagsMaker
{
    private readonly ITagsRepository _tagsRepository;

    public TagsMaker(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
    }

    public async Task<IEnumerable<Tag>> GetOrCreateTagsByNameAsync(IEnumerable<string> tagNames)
    {
        if (tagNames is null)
        {
            throw new ArgumentNullException(nameof(tagNames));
        }

        var existingTags = await _tagsRepository.GetTagsByNameAsync(tagNames);

        var existingTagNames = existingTags.Select(t => t.Name);

        var missingTagNames = tagNames.Except(existingTagNames);

        var allTags = new List<Tag>(existingTags);

        foreach (var missingTagName in missingTagNames)
        {
            var newTag = new Tag(Guid.NewGuid(), missingTagName);

            await _tagsRepository.SaveAsync(newTag);

            allTags.Add(newTag);
        }

        return allTags;
    }
}
