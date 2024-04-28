using Dapper;
using KM_ClientApp.Commons.Connection;
using KM_ClientApp.Models.Entity;

namespace KM_ClientApp.Endpoint.Email;

public class EmailRepository : IEmailRepository
{
    private readonly ISQLConnectionFactory _connection;

    public EmailRepository(ISQLConnectionFactory connectionFactory)
    {
        _connection = connectionFactory;
    }

    public async Task<EmailHelpdeskConfig?> GetEmailHelpdeskConfigAsync(CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT 
	                [MAIL_HELPDESK_CONTENT_HTML],
                    [MAIL_HELPDESK_FROM],
                    [MAIL_HELPDESK_SUBJECT],
                    [MAIL_HELPDESK_TO],
	                [MAIL_CONFIG_USERNAME],
	                [MAIL_CONFIG_PASSWORD],
	                [MAIL_CONFIG_SERVER],
	                [MAIL_CONFIG_PORT]
                FROM 
	                [dbo].[View_Configuration_Email]
        ";

        var command = new CommandDefinition(query, cancellationToken: cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<EmailHelpdeskConfig?>(command);

        return result;
    }

    public async Task<EmailHelpdeskFormat?> GetEmalHelpdeskFormat(string LoginName, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT 
                    [Login_Name],
	                [Full_Name],
                    [Job_Title],
                    [Office_Location]
                FROM 
	                [ITAssetsManagement].[dbo].[User_Profile]
                WHERE
	                [Login_Name] = @Login_Name
        ";

        var command = new CommandDefinition(query, new { Login_Name = LoginName }, cancellationToken: cancellationToken);
        var result = await connection.QueryFirstOrDefaultAsync<EmailHelpdeskFormat?>(command);

        return result;
    }

    public async Task<EmailHistoryConfig?> GetMailConfigAsync(CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT 
	                [MAIL_HISTORY_STATUS],
	                [MAIL_HISTORY_FROM],
	                [MAIL_HISTORY_SUBJECT],
	                [MAIL_CONFIG_USERNAME],
	                [MAIL_CONFIG_PASSWORD],
	                [MAIL_CONFIG_SERVER],
	                [MAIL_CONFIG_PORT]
                FROM 
	                [dbo].[View_Configuration_Email]
        ";

        var command = new CommandDefinition(query, cancellationToken: cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<EmailHistoryConfig?>(command);

        return result;
    }

    public async Task<EmailHistoryRecipient?> GetMailRecepientAsync(string loginName, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string query = @"
               SELECT 
	                [Full_Name],    
	                [Email]   
                FROM 
	                [ITAssetsManagement].[dbo].[User_Profile]
                WHERE
	                [Login_Name] = @Login_Name
        ";

        var command = new CommandDefinition(query, new { Login_Name = loginName }, cancellationToken: cancellationToken);
        var result = await connection.QueryFirstOrDefaultAsync<EmailHistoryRecipient?>(command);

        return result;
    }

    public async Task<int> PostMailHelpdeskAsync(EmailHelpdeskFilter email, CancellationToken cancellationToken)
    {
        using var connection = await _connection.CreateConnectionAsync();

        string storedProcedureName = "[dbo].[Add_Mail_Helpdesk]";

        var command = new CommandDefinition(
            storedProcedureName,
            email,
            commandType: System.Data.CommandType.StoredProcedure,
            cancellationToken: cancellationToken
        );

        var result = await connection.ExecuteAsync(command);

        return result;
    }
}

public interface IEmailRepository
{
    Task<EmailHistoryConfig?> GetMailConfigAsync(CancellationToken cancellationToken);
    Task<EmailHistoryRecipient?> GetMailRecepientAsync(string LoginName, CancellationToken cancellationToken);
    Task<EmailHelpdeskConfig?> GetEmailHelpdeskConfigAsync(CancellationToken cancellationToken);
    Task<EmailHelpdeskFormat?> GetEmalHelpdeskFormat(string LoginName, CancellationToken cancellationToken);
    Task<int> PostMailHelpdeskAsync(EmailHelpdeskFilter email, CancellationToken cancellationToken);
}
