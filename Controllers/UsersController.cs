using Microsoft.AspNetCore.Mvc;
using UserCrudService.DTOs;
using UserCrudService.Services;

namespace UserCrudService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(IUserService svc) : ControllerBase
{
    // actingLogin передаётся простым заголовком X-Login (демо-авторизация)
    private string ActingLogin =>
        Request.Headers["X-Login"].FirstOrDefault()
        ?? throw new UnauthorizedAccessException("X-Login header is missing.");

    // 1. Создание (Admin)
    [HttpPost]
    public ActionResult<UserDto> Create(UserCreateDto dto)
        => Ok(svc.Create(dto, ActingLogin));

    // 2. Изменение профиля
    [HttpPatch("{login}/profile")]
    public ActionResult<UserDto> UpdateProfile(string login, UserUpdateDto dto)
        => Ok(svc.UpdateProfile(login, dto, ActingLogin));

    // 3. Смена пароля
    [HttpPatch("{login}/password")]
    public IActionResult ChangePassword(string login, PasswordChangeDto dto)
    {
        svc.ChangePassword(login, dto, ActingLogin);
        return NoContent();
    }

    // 4. Смена логина
    [HttpPatch("{login}/login")]
    public IActionResult ChangeLogin(string login, LoginChangeDto dto)
    {
        svc.ChangeLogin(login, dto, ActingLogin);
        return NoContent();
    }

    // 5. Список активных
    [HttpGet("active")]
    public ActionResult<IEnumerable<UserDto>> GetActive()
        => Ok(svc.GetActive(ActingLogin));

    // 6. Информация по логину
    [HttpGet("{login}")]
    public ActionResult<UserDto> GetByLogin(string login)
        => Ok(svc.GetByLogin(login, ActingLogin));

    // 7. Аутентификация (сам пользователь)
    [HttpPost("auth")]
    public ActionResult<UserDto> Auth([FromQuery] string login, [FromQuery] string password)
        => Ok(svc.Auth(login, password));

    // 8. Старше возраста
    [HttpGet("older/{age:int}")]
    public ActionResult<IEnumerable<UserDto>> OlderThen(int age)
        => Ok(svc.GetOlderThen(age, ActingLogin));

    // 9. Удаление (soft=true по умолчанию)
    [HttpDelete("{login}")]
    public IActionResult Delete(string login, [FromQuery] bool soft = true)
    {
        svc.Delete(login, soft, ActingLogin);
        return NoContent();
    }

    // 10. Восстановление
    [HttpPost("{login}/restore")]
    public IActionResult Restore(string login)
    {
        svc.Restore(login, ActingLogin);
        return NoContent();
    }
}