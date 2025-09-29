using HackerNewsApi.Controllers;
using HackerNewsApi.Models;
using HackerNewsApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HackerNewsApi.Tests.Controllers;

public class HackerNewsControllerTests
{
  private readonly Mock<IHackerNewsService> _serviceMock = new();
  private readonly HackerNewsController _controller;

  public HackerNewsControllerTests()
  {
    _controller = new HackerNewsController(_serviceMock.Object);
  }

  [Fact]
  public async Task GetNewestStories_ReturnsBadRequest_WhenPageOutOfRange()
  {
    var result = await _controller.GetNewestStories(0, 20);

    result.Result.Should().BeOfType<BadRequestObjectResult>();
    _serviceMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task GetNewestStories_ReturnsOk_WithPayload()
  {
    var response = new StoriesResponse
    {
      Stories = new List<HackerNewsItem> { new() { Id = 1, Title = "Test" } },
      TotalCount = 1,
      Page = 1,
      PageSize = 20,
      TotalPages = 1
    };

    _serviceMock
      .Setup(s => s.GetNewestStoriesAsync(1, 20))
      .ReturnsAsync(response);

    var result = await _controller.GetNewestStories(1, 20);

    result.Result.Should().BeOfType<OkObjectResult>()
      .Which.Value.Should().BeEquivalentTo(response);
  }

  [Fact]
  public async Task SearchStories_ReturnsBadRequest_WhenQueryMissing()
  {
    var result = await _controller.SearchStories(string.Empty, 1, 20);

    result.Result.Should().BeOfType<BadRequestObjectResult>();
    _serviceMock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task SearchStories_ReturnsOk_WithMatches()
  {
    var response = new StoriesResponse
    {
      Stories = new List<HackerNewsItem> { new() { Id = 2, Title = "Angular" } },
      TotalCount = 1,
      Page = 1,
      PageSize = 20,
      TotalPages = 1
    };

    _serviceMock
      .Setup(s => s.SearchStoriesAsync("angular", 1, 20))
      .ReturnsAsync(response);

    var result = await _controller.SearchStories("angular", 1, 20);

    result.Result.Should().BeOfType<OkObjectResult>()
      .Which.Value.Should().BeEquivalentTo(response);
  }

  [Fact]
  public async Task SearchStories_ReturnsBadRequest_WhenPageOutOfRange()
  {
    var result = await _controller.SearchStories("query", 0, 20);

    result.Result.Should().BeOfType<BadRequestObjectResult>();
    _serviceMock.VerifyNoOtherCalls();
  }
}
