namespace TaskManagement.Domain.Entities;

public sealed class User
{
    private readonly List<TaskItem> _tasks = [];

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    public User(Guid id, string name, string email)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("User name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("User email is required.", nameof(email));
        }

        Id = id;
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();
}
