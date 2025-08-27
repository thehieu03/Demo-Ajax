using Entity.ModelRequest;
using Entity.ModelResponse;

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

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(CreateAccount createAccount)
    {
        if (!ModelState.IsValid)
        {
            return View(createAccount);
        }

        var response = await _client.PostAsJsonAsync(url + "/account/createAccount", createAccount);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Đăng ký thất bại: {errorMessage}");
            return View(createAccount);
        }

        TempData["SuccessMessage"] = "Tài khoản đã được tạo thành công! Vui lòng đăng nhập.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var response = await _client.PostAsJsonAsync(url + "/account/forgotPassword", request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mật khẩu mới đã được gửi đến email của bạn!";
                return RedirectToAction("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống";
                return View(request);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = "Có lỗi xảy ra, vui lòng thử lại";
                return View(request);
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
            return View(request);
        }
    }

}