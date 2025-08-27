using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc
{
    public class UserAccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
