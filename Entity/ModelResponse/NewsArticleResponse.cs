using System.Text.Json.Serialization;

namespace Entity.ModelResponse;

public class NewsArticleResponse
{
    [JsonPropertyName("newsArticleId")] public int NewsArticleId { get; set; }
    [JsonPropertyName("newsTitle")] public string NewsTitle { get; set; } = string.Empty;
    [JsonPropertyName("headline")] public string Headline { get; set; } = string.Empty;
    [JsonPropertyName("newsContent")] public string NewsContent { get; set; } = string.Empty;
    [JsonPropertyName("newsSource")] public string? NewsSource { get; set; } = string.Empty;
    [JsonPropertyName("categoryId")] public short? CategoryId { get; set; }
    [JsonPropertyName("newsStatus")] public bool? NewsStatus { get; set;  }
    [JsonPropertyName("categoryName")] public string? CategoryName { get; set; } = string.Empty;
    [JsonPropertyName("accountName")] public string? AccountName { get; set; } = string.Empty;
}