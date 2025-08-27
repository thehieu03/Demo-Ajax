using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc
{
    public class AdminTagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
