using System.Net;
using System.Net.Http.Json;
using HackerNewsApi.Models;
using HackerNewsApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HackerNewsApi.Tests;

public class ProgramTests : IClassFixture<ProgramTests.HackerNewsApplicationFactory>
{
  private readonly HttpClient _client;

  public ProgramTests(HackerNewsApplicationFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task GetNewestStoriesEndpoint_ReturnsSeedData()
  {
    var response = await _client.GetAsync("/api/hackernews/newest");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var payload = await response.Content.ReadFromJsonAsync<StoriesResponse>();
    payload.Should().NotBeNull();
    payload!.Stories.Should().HaveCount(1);
  }

  [Fact]
  public async Task SearchStoriesEndpoint_ValidatesEmptyQuery()
  {
    var response = await _client.GetAsync("/api/hackernews/search?page=1&pageSize=20");

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  public class HackerNewsApplicationFactory : WebApplicationFactory<Program>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(services =>
      {
        services.RemoveAll<IHackerNewsService>();
        services.AddSingleton<IHackerNewsService>(new StubHackerNewsService());
      });
    }
  }

  private sealed class StubHackerNewsService : IHackerNewsService
  {
    public Task<StoriesResponse> GetNewestStoriesAsync(int page = 1, int pageSize = 20)
      => Task.FromResult(new StoriesResponse
      {
        Stories = new List<HackerNewsItem>
        {
          new()
          {
            Id = 1,
            Title = "Stub story",
            Score = 10,
            Descendants = 3,
            By = "tester",
            Type = "story"
          }
        },
        TotalCount = 1,
        Page = page,
        PageSize = pageSize,
        TotalPages = 1
      });

    public Task<HackerNewsItem?> GetStoryByIdAsync(int id)
      => Task.FromResult<HackerNewsItem?>(new HackerNewsItem { Id = id, Title = "Stub" });

    public Task<StoriesResponse> SearchStoriesAsync(string query, int page = 1, int pageSize = 20)
      => Task.FromResult(new StoriesResponse
      {
        Stories = query.Contains("stub", StringComparison.OrdinalIgnoreCase)
          ? new List<HackerNewsItem> { new() { Id = 2, Title = "Stub match" } }
          : new List<HackerNewsItem>(),
        TotalCount = query.Contains("stub", StringComparison.OrdinalIgnoreCase) ? 1 : 0,
        Page = page,
        PageSize = pageSize,
        TotalPages = 1
      });
  }
}
