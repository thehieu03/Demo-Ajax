using Entity.ModelResponse;
using Microsoft.AspNetCore.Mvc;

namespace Client.ControllerMvc;

public class HomeController : Controller
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private static string? url;

    public HomeController(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
        url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
    }
    public async Task<IActionResult> Index()
    {
        var newsArticlesResponse = await _client.GetAsync(url + "/NewsArticle");
        var categoriesResponse = await _client.GetAsync(url + "/Category");
        if (categoriesResponse.IsSuccessStatusCode && newsArticlesResponse.IsSuccessStatusCode)
        {
            var categories = await categoriesResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>();
            var newsArticles = await newsArticlesResponse.Content.ReadFromJsonAsync<List<NewsArticleResponse>>();
            var data = new DataResponse
            {
                Categories = categories ?? [],
                NewsArticles = newsArticles ?? []
            };
            return View(data);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Failed to load data from the server.");
        }
        return View();
    }

    public class DataResponse
    {
        public List<CategoryResponse> Categories { get; set; } = [];
        public List<NewsArticleResponse> NewsArticles { get; set; } = [];
    }
}