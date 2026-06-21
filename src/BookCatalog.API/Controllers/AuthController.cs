using BookCatalog.Application.Users;
using BookCatalog.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(IUserService users, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("POST api/auth/register");
        var created = await users.RegisterAsync(dto, cancellationToken);
        return Created($"/api/users/{created.Id}", created);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("POST api/auth/login");
        try
        {
            var auth = await users.LoginAsync(dto, cancellationToken);
            return Ok(auth);
        }
        catch (ValidationException exception)
        {
            logger.LogWarning(exception, "Invalid login attempt for {Email}", dto.Email);
            return Unauthorized(new { error = exception.Message });
        }
    }
}
