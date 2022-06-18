using System.Data.Common;
using System.Data.SQLite;
using CA.Assessment.Application.Factories;
using CA.Assessment.Store;

namespace CA.Assessment.Database.Sqlite.Factories;

public sealed class SqLiteDatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseConnectionString _dbConnectionString;

    public SqLiteDatabaseConnectionFactory(DatabaseConnectionString dbConnectionString)
    {
        _dbConnectionString = dbConnectionString ?? throw new ArgumentNullException(nameof(dbConnectionString));
    }

    public DbConnection GetNewConnection()
    {
        return new SQLiteConnection(_dbConnectionString.Value);
    }
}