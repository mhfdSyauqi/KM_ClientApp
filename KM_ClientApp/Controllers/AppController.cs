using Microsoft.AspNetCore.Mvc;

namespace KM_ClientApp.Controllers;
public class AppController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
