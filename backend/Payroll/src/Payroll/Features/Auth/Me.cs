using System.Security.Claims;

namespace Payroll.Features.Auth;

public static class Me
{
    public static async Task<IResult> Handle(
        ClaimsPrincipal currentUser,
        PayrollDbContext db,
        CancellationToken ct
    )
    {
        var user = await db
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUser.GetUserId(), ct);

        if (user is null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(
            new
            {
                user.Id,
                user.Email,
                user.Role,
                EmployeeId = user.EmployeeId?.Value,
            }
        );
    }
}
