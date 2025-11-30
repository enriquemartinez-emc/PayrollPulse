namespace Payroll.Features.Auth;

public static class ListUsers
{
    public sealed record Response(Guid Id, string Email, string Role);

    public static async Task<IResult> Handle(PayrollDbContext db, CancellationToken ct)
    {
        var users = await db.Users.Select(u => new Response(u.Id, u.Email, u.Role)).ToListAsync(ct);

        return TypedResults.Ok(users);
    }
}
