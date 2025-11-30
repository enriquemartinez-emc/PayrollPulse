using Microsoft.AspNetCore.Http.HttpResults;

namespace Payroll.Features.Policies;

public static class List
{
    public sealed record PolicyResponse(Guid Id, string Name);

    public static async Task<Ok<List<PolicyResponse>>> Handle(
        PayrollDbContext db,
        CancellationToken ct
    )
    {
        var policies =
            await db
                .PayrollPolicies.AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(d => new PolicyResponse(d.Id, d.Name))
                .ToListAsync(ct) ?? [];

        return TypedResults.Ok(policies);
    }
}
