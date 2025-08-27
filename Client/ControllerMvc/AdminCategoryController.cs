using Entity.ModelResponse;
using Microsoft.AspNetCore.Mvc;

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
    }
}
