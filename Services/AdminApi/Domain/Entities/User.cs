namespace AdminApi.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Operator"; // Admin, Operator vb.
    public bool IsActive { get; set; } = true;

    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetTokenExpiresAtUtc { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}