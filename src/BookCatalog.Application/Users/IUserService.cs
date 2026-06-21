namespace BookCatalog.Application.Users;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
}
