using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MyAPIController : ControllerBase
{
    protected readonly ISender Sender;
    public MyAPIController(ISender sender)
    {
        Sender = sender;
    }
}
