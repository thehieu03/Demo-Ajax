using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc;

public class LoginController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}