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
        var categoriesResponse = await _client.GetAsync(url + "/Category");
        if (categoriesResponse.IsSuccessStatusCode)
        {
            var categories = await categoriesResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>();
            var data = new DataResponse
            {
                Categories = categories ?? [],
                NewsArticles = []
            };
            return View(data);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Failed to load categories from the server.");
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetNewsArticles(int? categoryId = null, int page = 1, int pageSize = 6)
    {
        try
        {
            int skip = (page - 1) * pageSize;
            
            string odataQuery = $"?$skip={skip}&$top={pageSize}&$orderby=NewsArticleId desc";
            
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                odataQuery += $"&$filter=CategoryId eq {categoryId.Value}";
            }
            
            var apiUrl = url + "/NewsArticle" + odataQuery;
            var response = await _client.GetAsync(apiUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var articles = await response.Content.ReadFromJsonAsync<List<NewsArticleResponse>>();
                
                string countQuery = categoryId.HasValue && categoryId.Value > 0 
                    ? $"?$filter=CategoryId eq {categoryId.Value}&$count=true"
                    : "?$count=true";
                    
                var countResponse = await _client.GetAsync(url + "/NewsArticle" + countQuery);
                int totalCount = 0;
                
                if (countResponse.IsSuccessStatusCode)
                {
                    var countData = await countResponse.Content.ReadAsStringAsync();
                    if (countData.Contains("@odata.count"))
                    {
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(countData);
                        if (jsonDoc.RootElement.TryGetProperty("@odata.count", out var countElement))
                        {
                            totalCount = countElement.GetInt32();
                        }
                    }
                    else
                    {
                        var allArticles = await countResponse.Content.ReadFromJsonAsync<List<NewsArticleResponse>>();
                        totalCount = allArticles?.Count ?? 0;
                    }
                }
                
                var result = new
                {
                    articles = articles ?? new List<NewsArticleResponse>(),
                    totalCount = totalCount,
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    hasNextPage = skip + pageSize < totalCount,
                    hasPreviousPage = page > 1
                };
                
                return Json(result);
            }
            else
            {
                return Json(new { error = "Failed to load articles", statusCode = response.StatusCode });
            }
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            Console.WriteLine($"[STATS] Calling API: {url}/NewsArticle");
            var response = await _client.GetAsync(url + "/NewsArticle");
            Console.WriteLine($"[STATS] API Response Status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var articles = await response.Content.ReadFromJsonAsync<List<NewsArticleResponse>>();
                Console.WriteLine($"[STATS] Retrieved {articles?.Count ?? 0} articles");
                
                var stats = new
                {
                    totalArticles = articles?.Count ?? 0,
                    totalAuthors = articles?.Select(a => a.AccountName).Distinct().Count() ?? 0,
                    activeArticles = articles?.Count(a => a.NewsStatus == true) ?? 0
                };
                
                Console.WriteLine($"[STATS] Stats calculated: Total={stats.totalArticles}, Authors={stats.totalAuthors}, Active={stats.activeArticles}");
                return Json(stats);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[STATS] API Error: {response.StatusCode}, Content: {errorContent}");
                return Json(new { error = "Failed to load stats", statusCode = response.StatusCode, details = errorContent });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[STATS] Exception: {ex.Message}");
            return Json(new { error = ex.Message });
        }
    }

    public class DataResponse
    {
        public List<CategoryResponse> Categories { get; set; } = [];
        public List<NewsArticleResponse> NewsArticles { get; set; } = [];
    }
}