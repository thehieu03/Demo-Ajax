using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        // TODO: Implement login logic here
        // For now, just return to index
        return RedirectToAction("Index");
    }

    [HttpPost("register")]
    public IActionResult Register(string username, string email, string password, string confirmPassword)
    {
        return RedirectToAction("Index");
    }
}