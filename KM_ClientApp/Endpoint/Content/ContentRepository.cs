using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;

namespace KM_ClientApp.Endpoint.Content;

public class ContentRepository : IContentRepository
{
    private readonly ISQLConnectionFactory _connection;

    public ContentRepository(ISQLConnectionFactory connectionFactory)
    {
        _connection = connectionFactory;
    }

    public async Task<BotContent?> GetBotContentByIdAsync(BotContentRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT
	                [uid],
	                [description] = [description_html]
                FROM
	                [dbo].[Bot_Content]
                WHERE
	                [status] = 'PUBLISHED' AND
	                [uid_bot_category] = @searcedId
        ";

        var command = new CommandDefinition(query, new { searcedId = Guid.Parse(request.Searched_Id) }, cancellationToken: cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<BotContent?>(command);

        return result;
    }
}

public interface IContentRepository
{
    Task<BotContent?> GetBotContentByIdAsync(BotContentRequest request, CancellationToken cancellationToken);
}