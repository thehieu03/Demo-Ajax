using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entity.ModelRequest;

public class LoginRequest
{
      [JsonPropertyName("email")]
      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Email is not valid")]
      public string Email { get; set; } = string.Empty;

      [JsonPropertyName("password")]
      [Required(ErrorMessage = "Password is required")]
      public string Password { get; set; } = string.Empty;
}