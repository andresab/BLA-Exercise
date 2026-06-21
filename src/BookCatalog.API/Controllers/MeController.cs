using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookCatalog.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.API.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController(ILogger<MeController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult<UserResponseDto> Get()
    {
        logger.LogInformation("GET api/me");
        var idText = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? string.Empty;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? User.FindFirstValue(ClaimTypes.Email)
            ?? string.Empty;
        var username = User.FindFirstValue("username") ?? string.Empty;

        if (!Guid.TryParse(idText, out var id))
        {
            return Unauthorized();
        }

        return Ok(new UserResponseDto(id, username, email, DateTime.UnixEpoch));
    }
}
