using BookCatalog.Application.Users;
using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Exceptions;
using BookCatalog.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookCatalog.Tests.Unit.Application;

public sealed class UserServiceTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _tokenService = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_repository.Object, _passwordHasher.Object, _tokenService.Object, Mock.Of<ILogger<UserService>>());
    }

    [Fact]
    public async Task RegisterAsync_ThrowsValidationException_WhenEmailAlreadyExists()
    {
        _repository.Setup(x => x.GetByEmailAsync("admin@bookcatalog.io", It.IsAny<CancellationToken>())).ReturnsAsync(SampleUser());

        var act = () => _service.RegisterAsync(new RegisterDto("admin", "admin@bookcatalog.io", "Admin1234!"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*already registered*");
    }

    [Fact]
    public async Task RegisterAsync_ThrowsValidationException_WhenPasswordTooWeak()
    {
        var act = () => _service.RegisterAsync(new RegisterDto("admin", "admin@bookcatalog.io", "weak"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*Password*");
    }

    [Fact]
    public async Task RegisterAsync_ReturnsUserDto_WhenValid()
    {
        _repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _passwordHasher.Setup(x => x.HashPassword("Admin1234!")).Returns("hash");
        _repository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        var result = await _service.RegisterAsync(new RegisterDto("admin", "admin@bookcatalog.io", "Admin1234!"), CancellationToken.None);

        result.Email.Should().Be("admin@bookcatalog.io");
        result.Username.Should().Be("admin");
    }

    [Fact]
    public async Task LoginAsync_ThrowsValidationException_WhenEmailNotFound()
    {
        _repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var act = () => _service.LoginAsync(new LoginDto("missing@bookcatalog.io", "Admin1234!"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*Invalid email or password*");
    }

    [Fact]
    public async Task LoginAsync_ThrowsValidationException_WhenPasswordIsWrong()
    {
        _repository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(SampleUser());
        _passwordHasher.Setup(x => x.Verify("wrong", "hash")).Returns(false);

        var act = () => _service.LoginAsync(new LoginDto("admin@bookcatalog.io", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>().WithMessage("*Invalid email or password*");
    }

    [Fact]
    public async Task LoginAsync_ReturnsAuthDto_WhenCredentialsAreValid()
    {
        var user = SampleUser();
        _repository.Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("Admin1234!", user.PasswordHash)).Returns(true);
        _tokenService.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        var result = await _service.LoginAsync(new LoginDto(user.Email, "Admin1234!"), CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        result.UserId.Should().Be(user.Id);
    }

    private static User SampleUser() =>
        new(Guid.NewGuid(), "admin", "admin@bookcatalog.io", "hash", DateTime.UtcNow);
}
