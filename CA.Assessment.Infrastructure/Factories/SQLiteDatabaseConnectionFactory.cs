using System.Data.Common;
using System.Data.SQLite;
using CA.Assessment.Application.Factories;
using CA.Assessment.Store;

namespace CA.Assessment.Infrastructure.Factories;

public sealed class SQLiteDatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseConnectionString databaseConnectionString;

    public SQLiteDatabaseConnectionFactory(DatabaseConnectionString databaseConnectionString)
    {
        this.databaseConnectionString = databaseConnectionString ??
                                        throw new ArgumentNullException(nameof(databaseConnectionString));
    }

    public DbConnection GetNewConnection()
    {
        return new SQLiteConnection(databaseConnectionString.Value);
    }
}
