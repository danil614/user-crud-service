using System.Collections.Concurrent;
using UserCrudService.Models;

namespace UserCrudService.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public IEnumerable<User> GetAll() => _users.Values;

    public User? GetByLogin(string login)
        => _users.GetValueOrDefault(login);

    public void Add(User user)
    {
        if (!_users.TryAdd(user.Login, user))
            throw new InvalidOperationException("The login already exists");
    }

    public void Update(User user)
        => _users[user.Login] = user;

    public void Remove(User user)
        => _users.TryRemove(user.Login, out _);
}