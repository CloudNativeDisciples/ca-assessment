using System.Data.Common;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Providers;

public interface ITagsMaker
{
    Task<IEnumerable<Tag>> GetOrCreateTagsByNameAsync(IEnumerable<string> tagNames);
}
