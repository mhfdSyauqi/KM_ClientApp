using KM_ClientApp.Controllers;
using KM_ClientApp.Endpoint.Session.Command;
using KM_ClientApp.Endpoint.Session.Query;
using KM_ClientApp.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Session;

public class SessionController : MyAPIController
{
    public SessionController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentSession(CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        string userName = computerName.Split("\\")[1];

        var query = new GetSessionQuery(userName);
        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddEmptySession(CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        string userName = computerName.Split("\\")[1];

        var command = new AddSessionCommand(userName);
        var response = await Sender.Send(command, cancellationToken);

        return response.IsSuccess ? Created("", response.Value) : BadRequest(response.Error);
    }


    [HttpPatch]
    public async Task<IActionResult> PatchSession([FromBody] PatchSessionRequest request, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        request.User_Name = computerName.Split("\\")[1];

        var command = new PatchSessionCommand(request);
        var response = await Sender.Send(command, cancellationToken);

        return response.IsSuccess ? NoContent() : BadRequest(response.Error);
    }

    [HttpDelete]
    public async Task<IActionResult> EndSession([FromBody] EndSessionRequest request, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        request.User_Name = computerName.Split("\\")[1];

        var command = new EndSessionCommand(request);
        var response = await Sender.Send(command, cancellationToken);

        return response.IsSuccess ? NoContent() : BadRequest(response.Error);
    }

}
