using System.Net.Mail;
using System.Text.RegularExpressions;
using BookCatalog.Application.Common;
using BookCatalog.Domain.Entities;
using BookCatalog.Domain.Exceptions;
using BookCatalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BookCatalog.Application.Users;

public sealed class UserService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ILogger<UserService> logger) : IUserService
{
    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        ValidateEmail(dto.Email);
        ValidatePassword(dto.Password);

        if (await users.GetByEmailAsync(dto.Email, cancellationToken) is not null)
        {
            throw new ValidationException($"Email '{dto.Email}' is already registered.");
        }

        if (string.IsNullOrWhiteSpace(dto.Username))
        {
            throw new ValidationException("Username is required.");
        }

        var user = new User(
            Guid.NewGuid(),
            dto.Username.Trim(),
            dto.Email.Trim().ToLowerInvariant(),
            passwordHasher.HashPassword(dto.Password),
            DateTime.UtcNow);

        var created = await users.CreateAsync(user, cancellationToken);
        logger.LogInformation("Registered user {UserId}", created.Id);
        return ToDto(created);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        ValidateEmail(dto.Email);
        var user = await users.GetByEmailAsync(dto.Email, cancellationToken)
            ?? throw new ValidationException("Invalid email or password.");

        if (!passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email or password.");
        }

        var token = jwtTokenService.GenerateToken(user);
        logger.LogInformation("User {UserId} logged in", user.Id);
        return new AuthResponseDto(token, DateTime.UtcNow.AddHours(1), user.Id, user.Username);
    }

    private static void ValidateEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new ValidationException("Email format is invalid.");
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < ValidationRules.MinPasswordLength)
        {
            throw new ValidationException("Password must be at least 8 characters long.");
        }

        if (!Regex.IsMatch(password, "[A-Z]") || !Regex.IsMatch(password, "\\d"))
        {
            throw new ValidationException("Password must contain at least one uppercase letter and one digit.");
        }
    }

    private static UserResponseDto ToDto(User user) => new(user.Id, user.Username, user.Email, user.CreatedAt);
}
