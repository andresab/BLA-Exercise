using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Auth;

public interface IJwtTokenGenerator
{
    string CreateToken(User user);
}
