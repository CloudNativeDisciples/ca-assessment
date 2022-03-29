using System.Data.Common;
using CA.Assessment.Application.Factories;

namespace CA.Assessment.Store;

public sealed class DatabaseSessionManager : IDatabaseSessionManager, IDatabaseSession, IDisposable
{
    private readonly IDatabaseConnectionFactory connectionFactory;

    public DatabaseSessionManager(IDatabaseConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public DbConnection? Connection { get; private set; }

    public DbTransaction? Transaction { get; private set; }

    public async Task BeginTransactionAsync()
    {
        if (Connection is null) await OpenConnectionAsync();

        if (Transaction is not null) await RollbackTransactionAsync();

        var newTransaction = await Connection!.BeginTransactionAsync();

        Transaction = newTransaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (Connection is null) return;

        if (Transaction is null) return;

        await Transaction.CommitAsync();
        await Transaction.DisposeAsync();

        Transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (Connection is null) return;

        if (Transaction is null) return;

        await Transaction.RollbackAsync();
        await Transaction.DisposeAsync();

        Transaction = null;
    }

    public async Task OpenConnectionAsync()
    {
        if (Transaction is not null) await RollbackTransactionAsync();

        if (Connection is not null) await CloseConnectionAsync();

        var newConnection = connectionFactory.GetNewConnection();

        await newConnection.OpenAsync();

        Connection = newConnection;
    }

    public async Task CloseConnectionAsync()
    {
        if (Connection is null) return;

        if (Transaction is not null) await RollbackTransactionAsync();

        await Connection.CloseAsync();
        await Connection.DisposeAsync();

        Connection = null;
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection?.Dispose();
    }
}
