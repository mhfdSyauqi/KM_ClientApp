using KM_ClientApp.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Endpoint.Config;

public class ConfigController : MyAPIController
{
    public ConfigController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetInitialConfiguration(CancellationToken cancellationToken)
    {
        var command = new GetConfigurationCommand();
        var response = await Sender.Send(command, cancellationToken);

        return response.IsSuccess ? Ok(response.MapResponse()) : NotFound(response.Error);
    }
}
