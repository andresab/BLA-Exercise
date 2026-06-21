namespace BookCatalog.Domain.Entities;

public sealed record User(
    Guid Id,
    string Username,
    string Email,
    string PasswordHash,
    DateTime CreatedAt);
