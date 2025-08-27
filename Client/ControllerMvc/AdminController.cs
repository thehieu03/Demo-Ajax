using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
