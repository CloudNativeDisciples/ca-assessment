using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Providers;

public interface ICategoriesMaker
{
    Task<Category> GetOrCreateCategoryByNameAsync(string categoryName);
}
