using KM_ClientApp.Controllers;
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

        var emailHistory = new EmailHistory(loginName, emailHistories);
        await Publisher.Publish(emailHistory, cancellationToken);

        return NoContent();
    }
}
