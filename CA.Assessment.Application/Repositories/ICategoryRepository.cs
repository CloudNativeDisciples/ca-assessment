using CA.Assessment.Model;

namespace CA.Assessment.Application.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByNameAsync(string categoryName);

    Task SaveAsync(Category category);

    Task<Category?> GetAsync(Guid categoryId);
}
