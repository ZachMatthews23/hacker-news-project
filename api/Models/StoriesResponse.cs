using System.Text.Json.Serialization;

namespace HackerNews.Functions.Models;

public class StoriesResponse
{
    [JsonPropertyName("stories")]
    public List<HackerNewsItem> Stories { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}
