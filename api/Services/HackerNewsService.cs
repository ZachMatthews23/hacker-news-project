using System.Text.Json;
using HackerNews.Functions.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HackerNews.Functions.Services;

public class HackerNewsService : IHackerNewsService
{
    private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";
    private const string NewStoriesCacheKey = "newstories";
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<HackerNewsService> _logger;

    public HackerNewsService(HttpClient httpClient, IMemoryCache cache, ILogger<HackerNewsService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<StoriesResponse> GetNewestStoriesAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var storyIds = await GetNewStoryIdsAsync();
            var totalCount = storyIds.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var skip = (page - 1) * pageSize;
            var pagedIds = storyIds.Skip(skip).Take(pageSize);

            var storyTasks = pagedIds.Select(GetStoryByIdAsync);
            var storyResults = await Task.WhenAll(storyTasks);
            var stories = storyResults
                .Where(story => story is not null)
                .Select(story => story!)
                .ToList();

            return new StoriesResponse
            {
                Stories = stories,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching newest stories");
            throw;
        }
    }

    public async Task<StoriesResponse> SearchStoriesAsync(string query, int page = 1, int pageSize = 20)
    {
        var allStories = await GetNewestStoriesAsync(1, 500);

        var filteredStories = allStories.Stories
            .Where(s => s.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        var totalCount = filteredStories.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var skip = (page - 1) * pageSize;
        var pagedStories = filteredStories.Skip(skip).Take(pageSize).ToList();

        return new StoriesResponse
        {
            Stories = pagedStories,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<HackerNewsItem?> GetStoryByIdAsync(int id)
    {
        var cacheKey = $"story_{id}";

        if (_cache.TryGetValue(cacheKey, out HackerNewsItem? cachedStory))
        {
            return cachedStory;
        }

        try
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}/item/{id}.json");
            var story = JsonSerializer.Deserialize<HackerNewsItem>(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (story != null)
            {
                _cache.Set(cacheKey, story, _cacheExpiry);
            }

            return story;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching story {StoryId}", id);
            return null;
        }
    }

    private async Task<List<int>> GetNewStoryIdsAsync()
    {
        if (_cache.TryGetValue(NewStoriesCacheKey, out List<int>? cachedIds))
        {
            return cachedIds ?? new List<int>();
        }

        try
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}/newstories.json");
            var storyIds = JsonSerializer.Deserialize<List<int>>(response) ?? new List<int>();

            _cache.Set(NewStoriesCacheKey, storyIds, _cacheExpiry);
            return storyIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching new story IDs");
            return new List<int>();
        }
    }
}
