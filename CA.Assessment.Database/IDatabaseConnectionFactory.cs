using System.Data.Common;

namespace CA.Assessment.Application.Factories;

public interface IDatabaseConnectionFactory
{
    DbConnection GetNewConnection();
}
