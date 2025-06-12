using System.Text.RegularExpressions;

namespace UserCrudService.Models;

public class User
{
    public Guid Guid { get; init; } = Guid.NewGuid();
    public string Login { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Gender { get; set; } // 0-ж,1-м,2-?
    public DateTime? Birthday { get; set; }
    public bool Admin { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "system";

    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }

    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }

    // Валидация полей (бросает исключение при ошибке)
    public static void Validate(string login, string password, string name)
    {
        Check("^[A-Za-z0-9]+$", login, nameof(Login));
        Check("^[A-Za-z0-9]+$", password, nameof(Password));
        Check("^[A-Za-zА-Яа-я]+$", name, nameof(Name));
        return;

        static void Check(string pattern, string value, string field) =>
            _ = Regex.IsMatch(value, pattern)
                ? true
                : throw new ArgumentException($"The field {field} contains invalid characters");
    }
}