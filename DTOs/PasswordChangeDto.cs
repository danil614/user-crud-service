namespace UserCrudService.DTOs;

// Смена пароля
public record PasswordChangeDto(
    string OldPassword,
    string NewPassword
);