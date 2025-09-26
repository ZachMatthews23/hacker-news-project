using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using HackerNewsApi.Services;
using FluentAssertions;
using System.Net;
using Moq.Protected;

namespace HackerNewsApi.Tests.Services
{
  public class HackerNewsServiceTests
  {
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ILogger<HackerNewsService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public HackerNewsServiceTests()
    {
      _mockCache = new Mock<IMemoryCache>();
      _mockLogger = new Mock<ILogger<HackerNewsService>>();
      _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
      _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
    }

    [Fact]
    public async Task GetStoryByIdAsync_ShouldReturnNull_WhenHttpRequestFails()
    {
      // Arrange
      _mockHttpMessageHandler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
          )
          .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

      var service = new HackerNewsService(_httpClient, _mockCache.Object, _mockLogger.Object);

      // Act
      var result = await service.GetStoryByIdAsync(123);

      // Assert
      result.Should().BeNull();
    }
  }
}