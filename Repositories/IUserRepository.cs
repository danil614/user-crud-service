using UserCrudService.Models;

namespace UserCrudService.Repositories;

public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetByLogin(string login);
    void Add(User user);
    void Update(User user);
    void Remove(User user); // жёсткое удаление
}