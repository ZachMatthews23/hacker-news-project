using System.Globalization;
using System.Net;
using System.Text;
using HackerNewsApi.Models;
using HackerNewsApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace HackerNewsApi.Tests.Services;

public class HackerNewsServiceTests : IDisposable
{
  private readonly MemoryCache _cache = new(new MemoryCacheOptions());

  private HackerNewsService CreateService(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
  {
    var handler = new StubHttpMessageHandler(responder);
    var client = new HttpClient(handler);
    return new HackerNewsService(client, _cache, NullLogger<HackerNewsService>.Instance);
  }

  [Fact]
  public async Task GetNewestStoriesAsync_ReturnsPagedStories()
  {
    // Arrange
    var requests = new Dictionary<string, int>();

    var service = CreateService((request, _) =>
    {
      var uri = request.RequestUri!.ToString();
      requests[uri] = requests.TryGetValue(uri, out var count) ? count + 1 : 1;

      if (uri.EndsWith("newstories.json", StringComparison.Ordinal))
      {
        return Task.FromResult(JsonResponse("[101,102]"));
      }

      var storyId = int.Parse(uri.Split('/').Last().Replace(".json", string.Empty, StringComparison.Ordinal));
      var json = $"{{\"id\":{storyId},\"title\":\"Story {storyId}\",\"by\":\"author{storyId}\",\"score\":{storyId},\"descendants\":{storyId},\"time\":1710000000,\"type\":\"story\"}}";
      return Task.FromResult(JsonResponse(json));
    });

    // Act
    var result = await service.GetNewestStoriesAsync(page: 1, pageSize: 2);

    // Assert
    result.Stories.Should().HaveCount(2)
      .And.OnlyContain(item => item.Type == "story");
    result.TotalCount.Should().Be(2);
    result.Page.Should().Be(1);
    result.TotalPages.Should().Be(1);
    requests.Count.Should().BeGreaterThan(0);
  }

  [Fact]
  public async Task GetNewestStoriesAsync_ReusesCachedStoryIds()
  {
    // Arrange
    var newStoriesCalls = 0;
    var service = CreateService((request, _) =>
    {
      var uri = request.RequestUri!.ToString();
      if (uri.EndsWith("newstories.json", StringComparison.Ordinal))
      {
        Interlocked.Increment(ref newStoriesCalls);
        return Task.FromResult(JsonResponse("[201,202]"));
      }

      var storyId = int.Parse(uri.Split('/').Last().Replace(".json", string.Empty, StringComparison.Ordinal));
      var json = $"{{\"id\":{storyId},\"title\":\"Story {storyId}\",\"by\":\"author\",\"score\":42,\"descendants\":7,\"time\":1710000000,\"type\":\"story\"}}";
      return Task.FromResult(JsonResponse(json));
    });

    // Act
    await service.GetNewestStoriesAsync(page: 1, pageSize: 1);
    await service.GetNewestStoriesAsync(page: 2, pageSize: 1);

    // Assert
    newStoriesCalls.Should().Be(1);
  }

  [Fact]
  public async Task GetNewestStoriesAsync_ReturnsEmpty_WhenFetchingIdsFails()
  {
    // Arrange
    var service = CreateService((request, _) =>
    {
      if (request.RequestUri!.AbsoluteUri.EndsWith("newstories.json", StringComparison.Ordinal))
      {
        throw new HttpRequestException("boom");
      }

      return Task.FromResult(JsonResponse("{}"));
    });

    // Act
    var result = await service.GetNewestStoriesAsync();

    // Assert
    result.Stories.Should().BeEmpty();
    result.TotalCount.Should().Be(0);
  }

  [Fact]
  public async Task GetStoryByIdAsync_ReturnsCachedValue_WhenPresent()
  {
    // Arrange
    var story = new HackerNewsItem
    {
      Id = 300,
      Title = "Cached",
      Score = 99,
      By = "cache",
      Descendants = 5,
      Type = "story"
    };
    _cache.Set("story_300", story);

    var httpCallCount = 0;
    var service = CreateService((_, _) =>
    {
      httpCallCount++;
      return Task.FromResult(JsonResponse("{}"));
    });

    // Act
    var result = await service.GetStoryByIdAsync(300);

    // Assert
    result.Should().BeSameAs(story);
    httpCallCount.Should().Be(0);
  }

  [Fact]
  public async Task GetStoryByIdAsync_CachesResult_AfterFetching()
  {
    // Arrange
    var callCount = 0;
    var service = CreateService((request, _) =>
    {
      callCount++;
      var json = "{\"id\":400,\"title\":\"Fresh\",\"by\":\"api\",\"score\":10,\"descendants\":1,\"time\":1710000000,\"type\":\"story\"}";
      return Task.FromResult(JsonResponse(json));
    });

    // Act
    var first = await service.GetStoryByIdAsync(400);
    var second = await service.GetStoryByIdAsync(400);

    // Assert
    first.Should().NotBeNull();
    second.Should().BeSameAs(first);
    callCount.Should().Be(1);
  }

  [Fact]
  public async Task GetStoryByIdAsync_ReturnsNull_WhenRequestFails()
  {
    // Arrange
    var service = CreateService((_, _) => throw new HttpRequestException("not found"));

    // Act
    var result = await service.GetStoryByIdAsync(500);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public async Task SearchStoriesAsync_FiltersNewestStories()
  {
    // Arrange
    var newestRequests = 0;
    var itemRequests = new Dictionary<int, int>();

    var service = CreateService((request, _) =>
    {
      var uri = request.RequestUri!.ToString();
      if (uri.EndsWith("newstories.json", StringComparison.Ordinal))
      {
        newestRequests++;
        return Task.FromResult(JsonResponse("[601,602,603]"));
      }

      var storyId = uri.Split('/').Last().Replace(".json", string.Empty, StringComparison.Ordinal);
      var id = int.Parse(storyId, CultureInfo.InvariantCulture);
      itemRequests[id] = itemRequests.TryGetValue(id, out var count) ? count + 1 : 1;

      var payload = id switch
      {
        601 => "{\"id\":601,\"title\":\"Angular Rocks\",\"by\":\"dev\",\"score\":21,\"descendants\":4,\"time\":1710000000,\"type\":\"story\"}",
        602 => "{\"id\":602,\"title\":\"Learning Angular Testing\",\"by\":\"dev\",\"score\":18,\"descendants\":2,\"time\":1710000500,\"type\":\"story\"}",
        _ => "{\"id\":603,\"title\":\"React Hooks Guide\",\"by\":\"dev\",\"score\":12,\"descendants\":1,\"time\":1710000600,\"type\":\"story\"}"
      };

      var storyJson = payload;
      return Task.FromResult(JsonResponse(storyJson));
    });

    // Act
    var result = await service.SearchStoriesAsync("angular", page: 1, pageSize: 2);

    // Assert
    newestRequests.Should().Be(1);
    itemRequests.Keys.Should().BeEquivalentTo(new[] { 601, 602, 603 });
    result.TotalCount.Should().Be(2);
    result.TotalPages.Should().Be(1);
    result.Stories.Should().HaveCount(2)
      .And.OnlyContain(s => s.Title!.Contains("Angular", StringComparison.OrdinalIgnoreCase));
  }

  public void Dispose()
  {
    _cache.Dispose();
  }

  private static HttpResponseMessage JsonResponse(string json)
    => new(HttpStatusCode.OK)
    {
      Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

  private sealed class StubHttpMessageHandler : HttpMessageHandler
  {
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

    public StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
    {
      _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      => _handler(request, cancellationToken);
  }
}
