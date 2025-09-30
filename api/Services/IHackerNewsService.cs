using HackerNews.Functions.Models;

namespace HackerNews.Functions.Services;

public interface IHackerNewsService
{
    Task<StoriesResponse> GetNewestStoriesAsync(int page = 1, int pageSize = 20);
    Task<StoriesResponse> SearchStoriesAsync(string query, int page = 1, int pageSize = 20);
    Task<HackerNewsItem?> GetStoryByIdAsync(int id);
}
