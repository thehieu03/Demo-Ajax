using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc
{
    public class AdminCategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
