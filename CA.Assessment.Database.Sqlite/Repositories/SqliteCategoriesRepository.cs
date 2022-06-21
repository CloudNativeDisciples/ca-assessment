using CA.Assessment.Application.Repositories;
using CA.Assessment.Database.Sqlite.Mappers;
using CA.Assessment.Database.Sqlite.Rows;
using CA.Assessment.Model;
using CA.Assessment.Store;
using Dapper;

namespace CA.Assessment.Database.Sqlite.Repositories;

internal sealed class SqliteCategoriesRepository : ICategoryRepository
{
    private readonly IDatabaseSession _databaseSession;

    public SqliteCategoriesRepository(IDatabaseSession databaseSession)
    {
        _databaseSession = databaseSession ?? throw new ArgumentNullException(nameof(databaseSession));
    }

    public async Task<Category?> GetByNameAsync(string categoryName)
    {
        if (categoryName is null)
        {
            throw new ArgumentNullException(nameof(categoryName));
        }

        if (_databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "You must open a connection in the current database session before calling repository methods");
        }

        var query = @"
            SELECT id, name
            FROM categories
            WHERE name = @Name
        ";

        var queryParams = new
        {
            Name = categoryName
        };

        var categoryRow = await _databaseSession.Connection.QuerySingleOrDefaultAsync<CategoryDbRow>(query,
            queryParams,
            _databaseSession.Transaction);

        if (categoryRow is null)
        {
            return null;
        }

        return CategoryRowsMapper.MapOne(categoryRow);
    }

    public async Task SaveAsync(Category category)
    {
        if (category is null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        if (_databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        if (_databaseSession.Transaction is null)
        {
            throw new InvalidOperationException(
                "No transaction in the database session. You must start a transaction before calling repository methods");
        }

        var query = @"
            INSERT INTO categories(id, name)
            VALUES (@Id, @Name)
        ";

        var queryParams = new
        {
            Id = category.Identity.ToString(),
            category.Name
        };

        await _databaseSession.Connection.ExecuteAsync(query,
            queryParams,
            _databaseSession.Transaction);
    }

    public async Task<Category?> GetAsync(Guid categoryId)
    {
        if (_databaseSession.Connection is null)
        {
            throw new InvalidOperationException(
                "No connection in the database session. You must open a connection before calling repository methods");
        }

        var query = @"
            SELECT *
            FROM categories
            WHERE id = @CategoryId
        ";

        var queryParams = new
        {
            CategoryId = categoryId.ToString()
        };

        var maybeCategory = await _databaseSession.Connection.QueryFirstOrDefaultAsync<CategoryDbRow>(query,
            queryParams,
            _databaseSession.Transaction);

        if (maybeCategory is null)
        {
            return null;
        }

        return CategoryRowsMapper.MapOne(maybeCategory);
    }
}
