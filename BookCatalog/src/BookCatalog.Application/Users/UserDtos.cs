namespace BookCatalog.Application.Users;

public sealed record RegisterDto(string Username, string Email, string Password);

public sealed record LoginDto(string Email, string Password);

public sealed record AuthResponseDto(string Token, DateTime ExpiresAt, Guid UserId, string Username);

public sealed record UserResponseDto(Guid Id, string Username, string Email, DateTime CreatedAt);
