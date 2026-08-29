namespace be.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? EmployeeNumber { get; set; }

    public string? PasswordHash { get; set; }

    public string RegistrationSource { get; set; } = "SELF_REGISTERED";

    public string ApprovalStatus { get; set; } = "PENDING";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}