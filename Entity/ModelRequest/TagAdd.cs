using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entity.ModelRequest
{
    public class TagAdd
    {
        [Required(ErrorMessage = "Tên tag không được để trống")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên tag phải có từ 2 đến 50 ký tự")]
        [JsonPropertyName("tagName")]
        public string? TagName { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được quá 200 ký tự")]
        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }
}
