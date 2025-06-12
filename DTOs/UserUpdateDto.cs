namespace UserCrudService.DTOs;

// Обновление профиля (имя/пол/дата)
public record UserUpdateDto(
    string Name,
    int Gender,
    DateTime? Birthday
);