using Entity.ModelRequest;
using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        var data = new DataResponse();
        return View(data);
    }

    [HttpPost]
    public IActionResult Login([FromForm][Bind(Prefix = "Login")] LoginRequest login)
    {
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Register([FromForm][Bind(Prefix = "CreateAccount")] CreateAccount accountAdd)
    {
        return RedirectToAction("Index");
    }
    
}
public class DataResponse
{
    public LoginRequest Login { get; set; } = new LoginRequest();
    public CreateAccount CreateAccount { get; set; } = new CreateAccount();
}