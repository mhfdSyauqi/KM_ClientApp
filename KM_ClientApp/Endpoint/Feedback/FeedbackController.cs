using KM_ClientApp.Controllers;
using KM_ClientApp.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Feedback;

public class FeedbackController : MyAPIController
{
    public FeedbackController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> AddUserFeedback([FromBody] UserFeedbackRequest request, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        request.User_Name = computerName.Split("\\")[1];

        var command = new FeedbackPostCommand(request);

        var response = await Sender.Send(command, cancellationToken);

        return response.IsSuccess ? NoContent() : BadRequest(response.Error);
    }
}
