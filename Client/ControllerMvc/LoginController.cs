using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}