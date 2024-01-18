using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Session;

public class SessionRepository : ISessionRepository
{
    private readonly ISQLConnectionFactory _connection;

    public SessionRepository(ISQLConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<CreatedSession?> AddEmptySessionAsync(string userName, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Add_Empty_Session]";

        var command = new CommandDefinition(
            storedProcedureName,
            new { UserName = userName },
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryFirstOrDefaultAsync<CreatedSession?>(command);

        return result;
    }

    public async Task<int> EndSessionAsync(EndSessionRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[End_User_Active_Session]";

        EndSession endedSession = new()
        {
            Uid = Guid.Parse(request.Id),
            User_Name = request.User_Name,
            Ended_By = request.Ended_By,
        };

        var command = new CommandDefinition(
            storedProcedureName,
            endedSession,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.ExecuteAsync(command);

        return result;
    }

    public async Task<GetSession?> GetSessionByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
                SELECT
                    uid,
                    is_active,
                    has_feedback,
                    records
                FROM 
                    [dbo].[View_Active_User_Session_Record]
                WHERE 
                    create_by = @UserName
        ";

        var command = new CommandDefinition(query, new { UserName = userName }, cancellationToken: cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<GetSession?>(command);

        return result;
    }

    public async Task<int> PatchActiveSessionAsync(PatchSessionRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Patch_User_Session]";

        PatchSession patchedSession = new()
        {
            Uid = Guid.Parse(request.Id),
            User_Name = request.User_Name,
            Records = request.New_Records
        };

        var command = new CommandDefinition(
            storedProcedureName,
            patchedSession,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.ExecuteAsync(command);

        return result;
    }
}

public interface ISessionRepository
{
    Task<CreatedSession?> AddEmptySessionAsync(string userName, CancellationToken cancellationToken);
    Task<int> EndSessionAsync(EndSessionRequest request, CancellationToken cancellationToken);
    Task<GetSession?> GetSessionByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<int> PatchActiveSessionAsync(PatchSessionRequest request, CancellationToken cancellationToken);
}
