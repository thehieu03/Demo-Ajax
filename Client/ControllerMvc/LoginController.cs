namespace Client.ControllerMvc;

public class LoginController : Controller
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private static string? url;

    public LoginController(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
        url = _configuration.GetValue<string>("URL") ?? throw new Exception("URL configuration is missing");
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", loginRequest);
        }

        var response = await _client.PostAsJsonAsync(url + "/account/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Đăng nhập thất bại");
            return View("Index", loginRequest);
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

        if (result == null)
        {
            ModelState.AddModelError("", "Phản hồi không hợp lệ từ server");
            return View("Index", loginRequest);
        }

        HttpContext.Session.SetString("Token", result.Token);
        HttpContext.Session.SetString("Username", result.AccountName);

        if (result.AccountRole == 2)
        {
            return RedirectToAction("Index", "AdminCategory");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

}