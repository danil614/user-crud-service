using UserCrudService.DTOs;
using UserCrudService.Models;
using UserCrudService.Repositories;

namespace UserCrudService.Services;

public class UserService(IUserRepository repo) : IUserService
{
    // Создаём Admin/admin при первом запуске
    public void EnsureAdmin()
    {
        if (repo.GetByLogin("Admin") != null) return;
        var admin = new User
        {
            Login = "Admin",
            Password = "Admin",
            Name = "Administrator",
            Gender = 2,
            Birthday = null,
            Admin = true,
            CreatedBy = "system"
        };
        repo.Add(admin);
    }

    private static UserDto ToDto(User u) =>
        new(u.Guid, u.Login, u.Name, u.Gender, u.Birthday, u.Admin,
            u.RevokedOn == null);

    private User Require(string login) =>
        repo.GetByLogin(login) ?? throw new KeyNotFoundException("User not found");

    private User Acting(string login) =>
        Require(login);

    private static bool IsActive(User u) => u.RevokedOn == null;

    private static int CalcAge(User u) =>
        !u.Birthday.HasValue ? 0 : (int)((DateTime.Today - u.Birthday.Value.Date).TotalDays / 365.2425);

    // Проверка роли (коротко)
    private static void DemandAdmin(User acting) =>
        _ = acting.Admin
            ? true
            : throw new UnauthorizedAccessException("Administrator privileges are required.");

    public UserDto Create(UserCreateDto dto, string actingLogin)
    {
        var acting = Acting(actingLogin);
        DemandAdmin(acting);

        User.Validate(dto.Login, dto.Password, dto.Name);

        var user = new User
        {
            Login = dto.Login,
            Password = dto.Password,
            Name = dto.Name,
            Gender = dto.Gender,
            Birthday = dto.Birthday,
            Admin = dto.Admin,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = actingLogin
        };
        repo.Add(user);
        return ToDto(user);
    }

    public UserDto UpdateProfile(string login, UserUpdateDto dto, string actingLogin)
    {
        var target = Require(login);
        var acting = Acting(actingLogin);

        if (!acting.Admin && acting.Login != target.Login)
            throw new UnauthorizedAccessException("Insufficient privileges.");

        if (!IsActive(target))
            throw new InvalidOperationException("User is disabled.");

        User.Validate(target.Login, target.Password, dto.Name);

        target.Name = dto.Name;
        target.Gender = dto.Gender;
        target.Birthday = dto.Birthday;
        target.ModifiedOn = DateTime.UtcNow;
        target.ModifiedBy = actingLogin;

        repo.Update(target);
        return ToDto(target);
    }

    public void ChangePassword(string login, PasswordChangeDto dto, string actingLogin)
    {
        var target = Require(login);
        var acting = Acting(actingLogin);

        if (!acting.Admin && acting.Login != target.Login)
            throw new UnauthorizedAccessException("Insufficient privileges.");

        if (!IsActive(target))
            throw new InvalidOperationException("User account is disabled.");

        if (!acting.Admin && target.Password != dto.OldPassword)
            throw new InvalidOperationException("The old password is incorrect.");

        User.Validate(target.Login, dto.NewPassword, target.Name);

        target.Password = dto.NewPassword;
        target.ModifiedOn = DateTime.UtcNow;
        target.ModifiedBy = actingLogin;

        repo.Update(target);
    }

    public void ChangeLogin(string login, LoginChangeDto dto, string actingLogin)
    {
        var target = Require(login);
        var acting = Acting(actingLogin);

        if (!acting.Admin && acting.Login != target.Login)
            throw new UnauthorizedAccessException("Insufficient privileges.");

        if (!IsActive(target))
            throw new InvalidOperationException("User account is disabled.");

        User.Validate(dto.NewLogin, target.Password, target.Name);

        if (repo.GetByLogin(dto.NewLogin) != null)
            throw new InvalidOperationException("The new login is already taken.");

        // удалить старый, сохранить новый (просто пересоздаём ключ в словаре)
        repo.Remove(target);
        target.Login = dto.NewLogin;
        target.ModifiedOn = DateTime.UtcNow;
        target.ModifiedBy = actingLogin;
        repo.Add(target);
    }

    public IEnumerable<UserDto> GetActive(string actingLogin)
    {
        DemandAdmin(Acting(actingLogin));
        return repo.GetAll()
            .Where(IsActive)
            .OrderBy(u => u.CreatedOn)
            .Select(ToDto);
    }

    public UserDto GetByLogin(string login, string actingLogin)
    {
        DemandAdmin(Acting(actingLogin));
        return ToDto(Require(login));
    }

    public UserDto Auth(string login, string password)
    {
        var user = Require(login);
        if (!IsActive(user))
            throw new UnauthorizedAccessException("User account is disabled.");

        if (user.Password != password)
            throw new UnauthorizedAccessException("Incorrect password.");

        return ToDto(user);
    }

    public IEnumerable<UserDto> GetOlderThen(int age, string actingLogin)
    {
        DemandAdmin(Acting(actingLogin));
        return repo.GetAll()
            .Where(u => IsActive(u) && CalcAge(u) > age)
            .Select(ToDto);
    }

    public void Delete(string login, bool soft, string actingLogin)
    {
        DemandAdmin(Acting(actingLogin));
        var user = Require(login);

        if (soft)
        {
            user.RevokedOn = DateTime.UtcNow;
            user.RevokedBy = actingLogin;
            repo.Update(user);
        }
        else
        {
            repo.Remove(user);
        }
    }

    public void Restore(string login, string actingLogin)
    {
        DemandAdmin(Acting(actingLogin));
        var user = Require(login);

        user.RevokedOn = null;
        user.RevokedBy = null;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = actingLogin;
        repo.Update(user);
    }
}