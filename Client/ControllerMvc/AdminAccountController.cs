using Microsoft.AspNetCore.Mvc;
using Entity.ModelResponse;
using System.Net.Http.Json;

namespace Client.ControllerMvc
{
    public class AdminAccountController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private static string? url;

        public AdminAccountController(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync(url + "/account/getAllAccount");
                
                if (response.IsSuccessStatusCode)
                {
                    var accounts = await response.Content.ReadFromJsonAsync<IEnumerable<SystemAccountResponse>>();
                    return View(accounts);
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tải danh sách tài khoản";
                    return View(new List<SystemAccountResponse>());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return View(new List<SystemAccountResponse>());
            }
        }
    }
}
