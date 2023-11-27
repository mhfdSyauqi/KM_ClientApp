using Microsoft.Data.SqlClient;

namespace KM_ClientApp.Commons.Connection;

public interface ISQLConnectionFactory
{
    Task<SqlConnection> CreateConnectionAsync();
}

