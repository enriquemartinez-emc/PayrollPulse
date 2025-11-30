using Microsoft.AspNetCore.Http.HttpResults;

namespace Payroll.Features.Departments;

public static class List
{
    public sealed record DepartmentResponse(Guid Id, string Name);

    public static async Task<Ok<List<DepartmentResponse>>> Handle(
        PayrollDbContext db,
        CancellationToken ct
    )
    {
        var departments =
            await db
                .Departments.AsNoTracking()
                .Select(d => new DepartmentResponse(d.Id, d.Name))
                .ToListAsync(ct) ?? [];

        return TypedResults.Ok(departments);
    }
}
