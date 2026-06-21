using BookCatalog.API.Controllers;
using BookCatalog.Application.Users;
using BookCatalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookCatalog.Tests.Unit.API;

public sealed class AuthControllerTests
{
    private readonly Mock<IUserService> _service = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_service.Object, Mock.Of<ILogger<AuthController>>());
    }

    [Fact]
    public async Task Register_Returns201_WithUserDto()
    {
        var dto = new UserResponseDto(Guid.NewGuid(), "admin", "admin@bookcatalog.io", DateTime.UtcNow);
        _service.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Register(new RegisterDto("admin", "admin@bookcatalog.io", "Admin1234!"), CancellationToken.None);

        var created = result.Result.Should().BeOfType<CreatedResult>().Subject;
        created.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Login_Returns200_WithToken()
    {
        var dto = new AuthResponseDto("token", DateTime.UtcNow.AddHours(1), Guid.NewGuid(), "admin");
        _service.Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var result = await _controller.Login(new LoginDto("admin@bookcatalog.io", "Admin1234!"), CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Login_Returns401_WhenCredentialsAreInvalid()
    {
        _service.Setup(x => x.LoginAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("Invalid email or password."));

        var result = await _controller.Login(new LoginDto("admin@bookcatalog.io", "wrong"), CancellationToken.None);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
