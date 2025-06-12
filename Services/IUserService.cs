using UserCrudService.DTOs;

namespace UserCrudService.Services;

public interface IUserService
{
    // Админ по умолчанию
    void EnsureAdmin();

    // CRUD
    UserDto Create(UserCreateDto dto, string actingLogin);
    UserDto UpdateProfile(string login, UserUpdateDto dto, string actingLogin);
    void ChangePassword(string login, PasswordChangeDto dto, string actingLogin);
    void ChangeLogin(string login, LoginChangeDto dto, string actingLogin);
    IEnumerable<UserDto> GetActive(string actingLogin);
    UserDto GetByLogin(string login, string actingLogin);
    UserDto Auth(string login, string password); // для самого пользователя
    IEnumerable<UserDto> GetOlderThen(int age, string actingLogin);
    void Delete(string login, bool soft, string actingLogin);
    void Restore(string login, string actingLogin);
}