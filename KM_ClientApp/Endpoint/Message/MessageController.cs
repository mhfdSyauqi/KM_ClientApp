using KM_ClientApp.Controllers;
using KM_ClientApp.Models.Entity;
using KM_ClientApp.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Message;

public class MessageController : MyAPIController
{
    public MessageController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    [Route("closing")]
    public async Task<IActionResult> GetClosingMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.CLOSING,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("feedback")]
    public async Task<IActionResult> GetFeedbackMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.FEEDBACK,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("idle")]
    public async Task<IActionResult> GetIdleMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.IDLE,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("layers/{layer}")]
    public async Task<IActionResult> GetLayersMsg(int layer, [FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var type = layer == 3 ? BotMessageType.LAYER_THREE : layer == 2 ? BotMessageType.LAYER_TWO : BotMessageType.LAYER_ONE;

        var query = new GetBotMessageQuery(
                type,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("sended_mail")]
    public async Task<IActionResult> GetSendedMailMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.MAIL_SENDED,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("not_found")]
    public async Task<IActionResult> GetNotFoundMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.NOT_FOUND,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("reasked")]
    public async Task<IActionResult> GetReaskedMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.REASKED,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("searched")]
    public async Task<IActionResult> GetSearchedMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.SEARCHED,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("solved")]
    public async Task<IActionResult> GetSolvedMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.SOLVED,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("sugestion")]
    public async Task<IActionResult> GetSugetionMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.SUGGESTION,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }

    [HttpGet]
    [Route("welcome")]
    public async Task<IActionResult> GetWelcomeMsg([FromBody] BotMessageRequest messageRequest, CancellationToken cancellationToken)
    {
        string computerName = User.Identity?.Name ?? "Error\\NotAuthUser";
        messageRequest.User_Name = computerName.Split("\\")[1];

        var query = new GetBotMessageQuery(
                BotMessageType.WELCOME,
                messageRequest);

        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }


}
