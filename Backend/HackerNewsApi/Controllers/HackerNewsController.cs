using Microsoft.AspNetCore.Mvc;
using HackerNewsApi.Models;
using HackerNewsApi.Services;

namespace HackerNewsApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class HackerNewsController : ControllerBase
  {
    private readonly IHackerNewsService _hackerNewsService;

    public HackerNewsController(IHackerNewsService hackerNewsService)
    {
      _hackerNewsService = hackerNewsService;
    }

    [HttpGet("newest")]
    public async Task<ActionResult<StoriesResponse>> GetNewestStories([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
      if (page < 1 || pageSize < 1 || pageSize > 100)
      {
        return BadRequest("Invalid page or pageSize parameters");
      }

      var result = await _hackerNewsService.GetNewestStoriesAsync(page, pageSize);
      return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<StoriesResponse>> SearchStories([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
      if (string.IsNullOrWhiteSpace(query))
      {
        return BadRequest("Query parameter is required");
      }

      if (page < 1 || pageSize < 1 || pageSize > 100)
      {
        return BadRequest("Invalid page or pageSize parameters");
      }

      var result = await _hackerNewsService.SearchStoriesAsync(query, page, pageSize);
      return Ok(result);
    }

    [HttpGet("story/{id}")]
    public async Task<ActionResult<HackerNewsItem>> GetStory(int id)
    {
      if (id <= 0)
      {
        return BadRequest("Invalid story ID");
      }

      var story = await _hackerNewsService.GetStoryByIdAsync(id);
      if (story == null)
      {
        return NotFound();
      }

      return Ok(story);
    }
  }
}