using CA.Assessment.Application.Repositories;
using CA.Assessment.Model;

namespace CA.Assessment.Application.Services;

public sealed class CategoriesMaker
{
    private readonly ICategoryRepository _categoriesRepository;

    public CategoriesMaker(ICategoryRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
    }

    public async Task<Category> GetOrCreateCategoryByNameAsync(string categoryName)
    {
        if (categoryName is null)
        {
            throw new ArgumentNullException(nameof(categoryName));
        }

        var maybeExistingCategory = await _categoriesRepository.GetByNameAsync(categoryName);

        if (maybeExistingCategory is not null)
        {
            return maybeExistingCategory;
        }

        var blogPostCategory = new Category(Guid.NewGuid(), categoryName);

        await _categoriesRepository.SaveAsync(blogPostCategory);

        return blogPostCategory;
    }
}
