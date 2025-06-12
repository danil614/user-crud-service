namespace UserCrudService.DTOs;

// Ответ клиенту
public record UserDto(
    Guid Guid,
    string Login,
    string Name,
    int Gender,
    DateTime? Birthday,
    bool Admin,
    bool Active
);