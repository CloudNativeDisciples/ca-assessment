using System.Data.Common;
using CA.Assessment.Domain.Anemic;

namespace CA.Assessment.Application.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByNameAsync(string categoryName);

    Task SaveAsync(Category category);

    Task<Category?> GetAsync(Guid categoryId);
}
