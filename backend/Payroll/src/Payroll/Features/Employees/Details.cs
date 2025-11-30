namespace Payroll.Features.Employees;

public static class Details
{
    public static async Task<IResult> Handle(Guid id, PayrollDbContext db, CancellationToken ct)
    {
        var employee = await db
            .Employees.AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EmployeeId == new EmployeeId(id), ct);

        if (employee is null)
            return TypedResults.NotFound();

        var employeeResponse = new EmployeeResponse(
            employee.EmployeeId.Value,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.BaseSalary.Amount,
            employee.Department?.Name ?? "",
            employee.HireDate
        );

        return TypedResults.Ok(employeeResponse);
    }
}
