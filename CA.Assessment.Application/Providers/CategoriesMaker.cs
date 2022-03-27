using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Providers;

public sealed class CategoriesMaker : ICategoriesMaker
{
    private readonly ICategoryRepository categoriesRepository;

    public CategoriesMaker(ICategoryRepository categoriesRepository)
    {
        this.categoriesRepository =
            categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
    }

    public async Task<Category> GetOrCreateCategoryByNameAsync(string categoryName)
    {
        if (categoryName is null)
        {
            throw new ArgumentNullException(nameof(categoryName));
        }

        var maybeExistingCategory = await categoriesRepository.GetByNameAsync(categoryName);

        if (maybeExistingCategory is not null)
        {
            return maybeExistingCategory;
        }

        var blogPostCategory = new Category(Guid.NewGuid(), categoryName);

        await categoriesRepository.SaveAsync(blogPostCategory);

        return blogPostCategory;
    }
}
