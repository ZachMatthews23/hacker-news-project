using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("random")]
public class RandomController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        var value = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return Ok(new { value });
    }
}
