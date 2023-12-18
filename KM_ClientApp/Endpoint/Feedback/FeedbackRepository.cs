using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Feedback;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ISQLConnectionFactory _connection;

    public FeedbackRepository(ISQLConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<int> AddUserFeedbackAsync(UserFeedbackRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Add_User_Feedback]";

        UserFeedback userFeeback = new()
        {
            Session_Id = Guid.Parse(request.Session_Id),
            Rating = request.Rating,
            Remark = request.Remark,
            User_Name = request.User_Name
        };

        var command = new CommandDefinition(
            storedProcedureName,
            userFeeback,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.ExecuteAsync(command);

        return result;
    }
}

public interface IFeedbackRepository
{
    Task<int> AddUserFeedbackAsync(UserFeedbackRequest request, CancellationToken cancellationToken);
}
