namespace HackerNews.Functions.Models;

public class StoriesResponse
{
    public List<HackerNewsItem> Stories { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
