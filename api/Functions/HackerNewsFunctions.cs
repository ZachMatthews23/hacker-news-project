using System.Net;
using HackerNews.Functions.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace HackerNews.Functions.Functions;

public class HackerNewsFunctions
{
    private readonly IHackerNewsService _hackerNewsService;
    private readonly ILogger<HackerNewsFunctions> _logger;

    public HackerNewsFunctions(IHackerNewsService hackerNewsService, ILogger<HackerNewsFunctions> logger)
    {
        _hackerNewsService = hackerNewsService;
        _logger = logger;
    }

    [Function("GetNewestStories")]
    public async Task<HttpResponseData> GetNewestStoriesAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hackernews/newest")] HttpRequestData req)
    {
        var query = QueryHelpers.ParseQuery(req.Url.Query);

        if (!TryReadPaging(query, out var page, out var pageSize, out var errorMessage))
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, errorMessage);
        }

        var stories = await _hackerNewsService.GetNewestStoriesAsync(page, pageSize);
        return await CreateOkResponseAsync(req, stories);
    }

    [Function("SearchStories")]
    public async Task<HttpResponseData> SearchStoriesAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hackernews/search")] HttpRequestData req)
    {
        var query = QueryHelpers.ParseQuery(req.Url.Query);

        if (!query.TryGetValue("query", out var queryValues) || string.IsNullOrWhiteSpace(queryValues.ToString()))
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, "Query parameter is required");
        }

        if (!TryReadPaging(query, out var page, out var pageSize, out var errorMessage))
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, errorMessage);
        }

        var stories = await _hackerNewsService.SearchStoriesAsync(queryValues.ToString(), page, pageSize);
        return await CreateOkResponseAsync(req, stories);
    }

    private static async Task<HttpResponseData> CreateOkResponseAsync<T>(HttpRequestData req, T payload)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(payload);
        return response;
    }

    private async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData req, HttpStatusCode statusCode, string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new { error = message });
        _logger.LogWarning("Request validation failed with status {StatusCode}: {Message}", (int)statusCode, message);
        return response;
    }

    private static bool TryReadPaging(IDictionary<string, StringValues> query, out int page, out int pageSize, out string error)
    {
        page = 1;
        pageSize = 20;
        error = string.Empty;

        if (query.TryGetValue("page", out var pageValues) && !string.IsNullOrWhiteSpace(pageValues.ToString()))
        {
            if (!int.TryParse(pageValues.ToString(), out page) || page < 1)
            {
                error = "Invalid page parameter";
                return false;
            }
        }

        if (query.TryGetValue("pageSize", out var pageSizeValues) && !string.IsNullOrWhiteSpace(pageSizeValues.ToString()))
        {
            if (!int.TryParse(pageSizeValues.ToString(), out pageSize) || pageSize < 1 || pageSize > 100)
            {
                error = "Invalid pageSize parameter";
                return false;
            }
        }

        if (pageSize > 100)
        {
            error = "Invalid pageSize parameter";
            return false;
        }

        return true;
    }
}
