using Microsoft.AspNetCore.Mvc;
using Entity.ModelResponse;
using Entity.ModelRequest;
using System.Net.Http.Json;

namespace Client.ControllerMvc
{
    public class AdminTagController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private static string? url;

        public AdminTagController(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync(url + "/tag");
                
                if (response.IsSuccessStatusCode)
                {
                    var tags = await response.Content.ReadFromJsonAsync<IEnumerable<TagResponse>>();
                    return View(tags);
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tải danh sách tag";
                    return View(new List<TagResponse>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return View(new List<TagResponse>());
            }
        }

        public IActionResult Create()
        {
            return View(new TagAdd());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TagAdd tagAdd)
        {
            if (!ModelState.IsValid)
            {
                return View(tagAdd);
            }

            try
            {
                var response = await _client.PostAsJsonAsync(url + "/tag", tagAdd);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tag đã được tạo thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Tạo tag thất bại: {errorMessage}");
                    return View(tagAdd);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return View(tagAdd);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync(url + $"/tag/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var tag = await response.Content.ReadFromJsonAsync<TagResponse>();
                    var tagAdd = new TagAdd
                    {
                        TagName = tag.TagName,
                        Note = tag.Note
                    };
                    return View(tagAdd);
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin tag";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TagAdd tagAdd)
        {
            if (!ModelState.IsValid)
            {
                return View(tagAdd);
            }

            try
            {
                var response = await _client.PutAsJsonAsync(url + $"/tag/{id}", tagAdd);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tag đã được cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Cập nhật tag thất bại: {errorMessage}");
                    return View(tagAdd);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return View(tagAdd);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            try
            {
                Console.WriteLine($"[ADMIN_DELETE_TAG] Bắt đầu xóa tag với ID: {id}");
                Console.WriteLine($"[ADMIN_DELETE_TAG] URL API: {url}/tag/{id}");
                
                var response = await _client.DeleteAsync(url + $"/tag/{id}");
                
                Console.WriteLine($"[ADMIN_DELETE_TAG] Response status: {response.StatusCode}");
                Console.WriteLine($"[ADMIN_DELETE_TAG] Response is success: {response.IsSuccessStatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ADMIN_DELETE_TAG] Xóa tag thành công");
                    TempData["SuccessMessage"] = "Tag đã được xóa thành công!";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ADMIN_DELETE_TAG] Lỗi từ API: {errorMessage}");
                    TempData["ErrorMessage"] = $"Xóa tag thất bại: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ADMIN_DELETE_TAG] Exception: {ex.Message}");
                Console.WriteLine($"[ADMIN_DELETE_TAG] Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
