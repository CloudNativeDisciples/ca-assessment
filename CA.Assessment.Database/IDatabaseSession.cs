using System.Data.Common;

namespace CA.Assessment.Store;

public interface IDatabaseSession
{
    /// <summary>
    /// Current <see cref="DbConnection"/> managed by this <see cref="IDatabaseSessionManager"/>
    /// Attempting to call any of the <see cref="DbConnection"/> methods directly will result in undefined behavior
    /// <code>null</code> if current <see cref="IDatabaseSession"/> has no connection open
    /// </summary>
    DbConnection? Connection { get; }

    /// <summary>
    /// Current <see cref="DbTransaction"/> managed by this <see cref="IDatabaseSessionManager"/>
    /// Attempting to call any of the <see cref="DbTransaction"/> methods directly will result in undefined behavior
    /// <code>null</code> if current <see cref="IDatabaseSession"/> does not have a transaction running
    /// </summary>
    DbTransaction? Transaction { get; }
}
