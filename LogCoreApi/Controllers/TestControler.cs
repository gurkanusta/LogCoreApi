using Microsoft.AspNetCore.Mvc;
using LogCoreApi.Exceptions;


using Microsoft.AspNetCore.Authorization;
namespace LogCoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("notfound")]
    public IActionResult NotFoundTest()
    {
        throw new NotFoundException("User not found."); 
    }

    [HttpPost("validate")]
    public IActionResult ValidateTest([FromBody] object body)
    {
        var errors = new Dictionary<string, string[]>
        {
            ["Email"] = new[] { "Email is required.", "Email format is invalid." },

            ["Password"] = new[] { "Password must be at least 8 characters." }
        };

        throw new ValidationException(errors); 
    }

    [Authorize]
    [HttpGet("secure-ping")]
    public IActionResult SecurePing()
    {
        return Ok(LogCoreApi.Models.ApiResponse<string>.Ok("ok", HttpContext));
    }






    [HttpGet("boom")]
    public IActionResult Boom()
    {
        throw new Exception("DB connection failed"); 
    }




}
