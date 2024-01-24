using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;

namespace KM_ClientApp.Endpoint.Config;

public class ConfigRepository : IConfigRepository
{
    private readonly ISQLConnectionFactory _connection;

    public ConfigRepository(ISQLConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<Configuration?> GetAppConfigurationAsync(CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT 
                    [APP_NAME],
                    [APP_IMAGE],
                    [DELAY_TYPING],
                    [IDLE_ATTEMPT],
                    [IDLE_DURATION]
                FROM 
                    [dbo].[View_Configuration_App]
        ";

        var command = new CommandDefinition(query, cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<Configuration>(command);

        return result;

    }
}

public interface IConfigRepository
{
    Task<Configuration?> GetAppConfigurationAsync(CancellationToken cancellationToken);
}
