using Payroll.Domain;
using Payroll.Domain.ValueObjects;

namespace Payroll.Infrastructure.Auth;

public sealed class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public byte[] PasswordHash { get; private set; } = [];
    public byte[] PasswordSalt { get; private set; } = [];
    public string Role { get; private set; } = Roles.Employee; // default

    public EmployeeId? EmployeeId { get; private set; }
    public Employee? Employee { get; private set; }

    private User() { } // EF

    public static User CreateStandalone(
        string email,
        byte[] passwordHash,
        byte[] passwordSalt,
        string role
    )
    {
        return new User
        {
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = role,
        };
    }

    public static User CreateForEmployee(
        EmployeeId employeeId,
        byte[] passwordHash,
        byte[] passwordSalt,
        string role
    )
    {
        return new User
        {
            EmployeeId = employeeId,
            Email = string.Empty,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = role,
        };
    }

    public void SetEmail(string email) => Email = email;

    public void LinkEmployee(EmployeeId employeeId) => EmployeeId = employeeId;

    public void SetPassword(byte[] hash, byte[] salt)
    {
        PasswordHash = hash;
        PasswordSalt = salt;
    }

    public void SetRole(string role) => Role = role;
}
