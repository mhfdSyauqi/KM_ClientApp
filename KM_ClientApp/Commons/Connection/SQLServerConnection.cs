using Microsoft.Data.SqlClient;

namespace KM_ClientApp.Commons.Connection;

public class SQLServerConnection : ISQLConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SQLServerConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<SqlConnection> CreateConnectionAsync()
    {
        var Connection = new SqlConnection(_configuration.GetConnectionString("DevDB"));

        return await Task.FromResult(Connection);
    }
}

