using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entity.ModelRequest;

public class CreateAccount
{
    [Required]
    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }
    [EmailAddress]
                                  [JsonPropertyName("accountEmail")]
    public string? AccountEmail { get; set; }
    [JsonPropertyName("accountPassword")]
    [Required]
    public string? AccountPassword { get; set; }
}