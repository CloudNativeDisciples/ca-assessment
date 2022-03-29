using CA.Assessment.Application.Repositories;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Mappers;
using CA.Assessment.Infrastructure.Rows;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Infrastructure.Repositories;

internal sealed class SQLiteCategoriesRepository : ICategoryRepository
{
    private readonly CategoryRowsMapper categoryRowsMapper;
    private readonly IDatabaseSession databaseSession;

    public SQLiteCategoriesRepository(IDatabaseSession databaseSession, CategoryRowsMapper categoryRowsMapper)
    {
        this.databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
        this.categoryRowsMapper = categoryRowsMapper ?? throw new ArgumentNullException(nameof(categoryRowsMapper));
    }

    public async Task<Category?> GetByNameAsync(string categoryName)
    {
        if (categoryName is null) throw new ArgumentNullException(nameof(categoryName));

        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "You must open a connection in the current database session before calling repository methods");

        var query = @"
            SELECT id, name
            FROM categories
            WHERE name = @Name
        ";

        var queryParams = new
        {
            Name = categoryName
        };

        var categoryRow = await databaseSession.Connection.QuerySingleOrDefaultAsync<CategoryRow>(query,
            queryParams,
            databaseSession.Transaction);

        if (categoryRow is null) return null;

        return categoryRowsMapper.MapOne(categoryRow);
    }

    public async Task SaveAsync(Category category)
    {
        if (category is null) throw new ArgumentNullException(nameof(category));

        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        if (databaseSession.Transaction is null)
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");

        var query = @"
            INSERT INTO categories(id, name)
            VALUES (@Id, @Name)
        ";

        var queryParams = new
        {
            Id = category.Identity.ToString(),
            category.Name
        };

        await databaseSession.Connection.ExecuteAsync(query,
            queryParams,
            databaseSession.Transaction);
    }

    public async Task<Category?> GetAsync(Guid categoryId)
    {
        if (databaseSession.Connection is null)
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");

        var query = @"
            SELECT *
            FROM categories
            WHERE id = @CategoryId
        ";

        var queryParams = new
        {
            CategoryId = categoryId.ToString()
        };

        var maybeCategory = await databaseSession.Connection.QueryFirstOrDefaultAsync<CategoryRow>(query,
            queryParams,
            databaseSession.Transaction);

        if (maybeCategory is null) return null;

        return categoryRowsMapper.MapOne(maybeCategory);
    }
}
