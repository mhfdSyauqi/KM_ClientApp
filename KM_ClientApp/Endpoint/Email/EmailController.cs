using KM_ClientApp.Controllers;
using KM_ClientApp.Endpoint.Session.Query;
using KM_ClientApp.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Email;

public class EmailController : MyAPIController
{
    private readonly IPublisher Publisher;
    public EmailController(ISender sender, IPublisher publisher) : base(sender)
    {
        Publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> SendMailNotification([FromBody] EmailHistoryRequest emailHistories, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        string loginName = computerName.Split("\\")[1];

        var query = new GetSessionFeedbackQuery(loginName);
        var result = await Sender.Send(query, cancellationToken);

        var emailHistory = new EmailHistory(loginName,result.Value.Id, emailHistories);
        
        await Publisher.Publish(emailHistory, cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("helpdesk")]
    public async Task<IActionResult> SendMailHelpdesk([FromBody] MailHelpdeskRequest request, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        string loginName = computerName.Split("\\")[1];
        request.Send_By = loginName;

        var emailHelpdesk = new EmailHelpdesk(request);
        await Publisher.Publish(emailHelpdesk, cancellationToken);

        return NoContent();
    }
}
