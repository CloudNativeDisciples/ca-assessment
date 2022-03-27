using System.Data.Common;

namespace CA.Assessment.Store;

public interface IDatabaseSessionManager
{
    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();

    Task OpenConnectionAsync();

    Task CloseConnectionAsync();
}
