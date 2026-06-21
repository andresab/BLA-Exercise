namespace BookCatalog.Application.Users;

public interface IPasswordHasher
{
    string HashPassword(string plain);
    bool Verify(string plain, string hash);
}
