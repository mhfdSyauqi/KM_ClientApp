using KM_ClientApp.Controllers;
using KM_ClientApp.Models.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Content;

public class ContentController : MyAPIController
{
    public ContentController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetContentById([FromBody] BotContentRequest contentRequest, CancellationToken cancellationToken)
    {
        var query = new GetContentQuery(contentRequest);
        var response = await Sender.Send(query, cancellationToken);

        return response.IsSuccess ? Ok(response.CreateResponseObject()) : NotFound(response.Error);
    }
}
