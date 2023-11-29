using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Message;

public class MessageRepository : IMessageRepository
{
    private readonly ISQLConnectionFactory _connection;

    public MessageRepository(ISQLConnectionFactory connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<BotMessage>> GetBotMessageAsync(BotMessageType type, BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        var query = @"
               SELECT 
	                [sequence],
	                [contents]
                FROM
	                [dbo].[Bot_Message]
                WHERE
	                [type] = @Type And 
	                [is_active] = 1
                ORDER BY [sequence] ASC
        ";

        var command = new CommandDefinition(query, new { Type = type.value }, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<BotMessage>(command);

        foreach (var msg in result)
        {
            bool isUserName = msg.Contents.Contains("@UserName");
            bool isSelected = msg.Contents.Contains("@SelectedCategory");

            if (isUserName)
            {
                msg.Contents = msg.Contents.Replace("@UserName", messageRequest.User_Name);
            }

            if (isSelected)
            {
                msg.Contents = msg.Contents.Replace("@SelectedCategory", messageRequest.Selected_Category);
            }
        }

        return result;
    }
}

public interface IMessageRepository
{
    Task<IEnumerable<BotMessage>> GetBotMessageAsync(BotMessageType type, BotMessageRequest messageRequest, CancellationToken cancellationToken);
}
