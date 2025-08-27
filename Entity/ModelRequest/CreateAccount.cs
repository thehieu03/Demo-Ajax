using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entity.ModelRequest;

public class CreateAccount
{
    [Required(ErrorMessage = "Tên không được để trống")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên phải có từ 2 đến 50 ký tự")]
    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }
    
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [JsonPropertyName("accountEmail")]
    public string? AccountEmail { get; set; }
    
    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có từ 6 đến 100 ký tự")]
    [JsonPropertyName("accountPassword")]
    public string? AccountPassword { get; set; }
}