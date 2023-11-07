using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        // Validate the username and password (you should use more secure methods)
        if (login.Username == "daniel" && login.Password == "123")
        {
            // Create a session
            _httpContextAccessor.HttpContext.Session.SetInt32("UserId", 1);

            return Ok("Login successful.");
        }

        return Unauthorized("Invalid credentials.");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // Clear the session
        _httpContextAccessor.HttpContext.Session.Clear();

        return Ok("Logged out.");
    }
}
