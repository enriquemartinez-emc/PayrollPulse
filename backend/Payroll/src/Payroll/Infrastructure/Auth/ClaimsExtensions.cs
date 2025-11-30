using System.Security.Claims;

namespace Payroll.Infrastructure.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return id is null ? Guid.Empty : Guid.Parse(id);
    }

    public static string? GetUserEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email);

    public static string? GetUserRole(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role);

    public static bool IsAuthenticated(this ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated ?? false;

    public static bool IsAdmin(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role) == Roles.Admin.ToString();
}
