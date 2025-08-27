using System.Text.Json.Serialization;

namespace Entity.ModelRequest;

public class CategoryAdd
{
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = null!;
    [JsonPropertyName("categoryDesciption")]

    public string CategoryDesciption { get; set; } = null!;
    [JsonPropertyName("parentCategoryId")]

    public short? ParentCategoryId { get; set; }
    [JsonPropertyName("isActive")]

    public bool? IsActive { get; set; }
}
