using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc
{
    public class AdminAccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
