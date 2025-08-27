using Entity.ModelRequest;
using Entity.ModelResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client.ControllerMvc
{
    public class AdminCategoryController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private static string? url;
        public AdminCategoryController(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
        }
        public IActionResult Index()
        {
            var dataResponse = _client.GetAsync(url + "/Category").Result;
            if(dataResponse.IsSuccessStatusCode)
            {
                var data = dataResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>().Result;
                return View(data);
            }
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            var dataResponse = _client.GetAsync(url + "/Category").Result;
            var categories = dataResponse.IsSuccessStatusCode
                ? dataResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>().Result
                : new List<CategoryResponse>();
            ViewBag.ParentCategories = new SelectList(categories, nameof(CategoryResponse.CategoryId), nameof(CategoryResponse.CategoryName));
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryAdd request)
        {
            var response = _client.PostAsJsonAsync(url + "/Category", request).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Tạo category thất bại");
            var dataResponse = _client.GetAsync(url + "/Category").Result;
            var categories = dataResponse.IsSuccessStatusCode
                ? dataResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>().Result
                : new List<CategoryResponse>();
            ViewBag.ParentCategories = new SelectList(categories, nameof(CategoryResponse.CategoryId), nameof(CategoryResponse.CategoryName));
            return View(request);
        }

        [HttpGet]
        public IActionResult Edit(short id)
        {
            var response = _client.GetAsync(url + $"/Category/{id}").Result;
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            var data = response.Content.ReadFromJsonAsync<CategoryResponse>().Result;
            if (data == null) return RedirectToAction(nameof(Index));
            var allResponse = _client.GetAsync(url + "/Category").Result;
            var categories = allResponse.IsSuccessStatusCode
                ? allResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>().Result
                : new List<CategoryResponse>();
            ViewBag.ParentCategories = new SelectList(categories, nameof(CategoryResponse.CategoryId), nameof(CategoryResponse.CategoryName), data.ParentCategoryId);
            var vm = new CategoryAdd
            {
                CategoryName = data.CategoryName,
                CategoryDesciption = data.CategoryDesciption,
                ParentCategoryId = data.ParentCategoryId,
                IsActive = data.IsActive
            };
            ViewBag.CategoryId = data.CategoryId;
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(short id, CategoryAdd request)
        {
            var response = _client.PutAsJsonAsync(url + $"/Category/{id}", request).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Cập nhật category thất bại");
            ViewBag.CategoryId = id;
            var dataResponse = _client.GetAsync(url + "/Category").Result;
            var categories = dataResponse.IsSuccessStatusCode
                ? dataResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>().Result
                : new List<CategoryResponse>();
            ViewBag.ParentCategories = new SelectList(categories, nameof(CategoryResponse.CategoryId), nameof(CategoryResponse.CategoryName), request.ParentCategoryId);
            return View(request);
        }

        [HttpPost]
        public IActionResult Delete(short id)
        {
            var response = _client.DeleteAsync(url + $"/Category/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Xóa category thất bại";
            return RedirectToAction(nameof(Index));
        }
    }
}
