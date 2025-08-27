using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entity.ModelRequest;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
