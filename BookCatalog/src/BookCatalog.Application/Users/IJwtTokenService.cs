using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Users;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
