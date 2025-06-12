namespace UserCrudService.DTOs;

// Создание
public record UserCreateDto(
    string Login,
    string Password,
    string Name,
    int Gender,
    DateTime? Birthday,
    bool Admin
);