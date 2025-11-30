using Payroll.Infrastructure;

namespace Payroll.Features.Employees;

public static class List
{
    public static async Task<IResult> Handle(
        [AsParameters] PaginationQuery query,
        PayrollDbContext db,
        CancellationToken ct
    )
    {
        var employeeQuery = db
            .Employees.Include(e => e.Department)
            .OrderByDescending(e => e.HireDate)
            .AsNoTracking()
            .Select(e => new EmployeeResponse(
                e.EmployeeId.Value,
                e.FirstName,
                e.LastName,
                e.Email,
                e.BaseSalary.Amount,
                e.Department!.Name,
                e.HireDate
            ));

        var paginated = await employeeQuery.ToPaginatedListAsync(query, ct);

        return TypedResults.Ok(paginated);
    }
}
