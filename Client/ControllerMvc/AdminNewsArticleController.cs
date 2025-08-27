using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc
{
    public class AdminNewsArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
