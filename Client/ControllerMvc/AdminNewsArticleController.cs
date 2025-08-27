using Microsoft.AspNetCore.Mvc;
using Entity.ModelResponse;

namespace Client.ControllerMvc
{
    public class AdminNewsArticleController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly string? url;

        public AdminNewsArticleController(HttpClient client,IConfiguration configuration) {
            _client=client;
            _configuration=configuration;
            url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
        }
        public IActionResult Index()
        {
            var response =  _client.GetAsync(url + "/NewsArticle").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadFromJsonAsync<List<NewsArticleResponse>>().Result;
                return View(data);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var response = _client.DeleteAsync(url + $"/NewsArticle/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Xóa bài viết thất bại";
            return RedirectToAction(nameof(Index));
        }
    }
}
