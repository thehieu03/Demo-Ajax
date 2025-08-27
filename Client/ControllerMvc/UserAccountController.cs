using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Client.ControllerMvc
{
    public class UserAccountController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private static string? url;

        public UserAccountController(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
        }
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(url + "/account/getAccountById");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Login");
            }
            var result = await response.Content.ReadFromJsonAsync<SystemAccountUserResponse>();
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Entity.ModelResponse.SystemAccountUserResponse model)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsJsonAsync(url + "/account/updateAccount", model);
            // riset usernaem in session
            if (!string.IsNullOrEmpty(model.AccountName))
            {
                HttpContext.Session.SetString("Username", model.AccountName);
            }
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Update failed");
                return View("Index", model);
            }
            var updated = await response.Content.ReadFromJsonAsync<Entity.ModelResponse.SystemAccountUserResponse>();
            TempData["SuccessMessage"] = "Updated successfully";

            return View("Index", updated ?? model);
        }
    }
}
