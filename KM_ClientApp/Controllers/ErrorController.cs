using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        var path = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Endpoint?.ToString();

        return Problem(detail: exception?.Message);
    }
}
