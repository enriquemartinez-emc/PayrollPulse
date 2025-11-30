using Microsoft.AspNetCore.Mvc;

namespace Payroll.Features.Employees;

public static class Search
{
    public record Response(Guid Id, string FullName, string Email);

    public static async Task<IResult> Handle(
        [FromQuery] string query,
        PayrollDbContext db,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return TypedResults.Ok(Array.Empty<object>());

        var normalized = $"%{query.Trim()}%"; // prepare for Postgresql ILIKE search

        var response = await db
            .Employees.AsNoTracking()
            .Where(e =>
                EF.Functions.ILike(e.FirstName, normalized)
                || EF.Functions.ILike(e.LastName, normalized)
                || EF.Functions.ILike(e.Email, normalized)
            )
            .OrderBy(e => e.FirstName)
            .Take(20)
            .Select(e => new Response(e.EmployeeId.Value, $"{e.FirstName} {e.LastName}", e.Email))
            .ToListAsync(ct);

        return TypedResults.Ok(response);
    }
}
