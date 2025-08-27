using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Entity.ModelResponse;
using Entity.ModelRequest;

namespace Client.ControllerMvc
{
    public class NewsArticleController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly string? url;

        public NewsArticleController(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("Username") != null;

        private void LoadViewData()
        {
            var categoriesResponse = _client.GetAsync(url + "/Category").Result;
            var categories = categoriesResponse.IsSuccessStatusCode
                ? categoriesResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>().Result
                : new List<CategoryResponse>();
            ViewBag.Categories = new SelectList(categories, nameof(CategoryResponse.CategoryId), nameof(CategoryResponse.CategoryName));
            
            var tagsResponse = _client.GetAsync(url + "/Tag").Result;
            var tags = tagsResponse.IsSuccessStatusCode
                ? tagsResponse.Content.ReadFromJsonAsync<List<TagResponse>>().Result
                : new List<TagResponse>();
            ViewBag.Tags = tags;
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Login");
            LoadViewData();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateNewsArticleRequest model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Index", "Login");
            
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadViewData();
                    return View(model);
                }

                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError(string.Empty, "Token không hợp lệ, vui lòng đăng nhập lại");
                    return RedirectToAction("Index", "Login");
                }

                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = _client.PostAsJsonAsync(url + "/NewsArticle/create", model).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "UserAccount");
                }
                
                var errorContent = response.Content.ReadAsStringAsync().Result;
                ModelState.AddModelError(string.Empty, $"Tạo bài viết thất bại: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
            }

            LoadViewData();
            return View(model);
        }
    }
}


