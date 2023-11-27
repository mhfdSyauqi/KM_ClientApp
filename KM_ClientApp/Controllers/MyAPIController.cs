using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MyAPIController : ControllerBase
{
    protected readonly ISender Sender;
    public MyAPIController(ISender sender)
    {
        Sender = sender;
    }
}
