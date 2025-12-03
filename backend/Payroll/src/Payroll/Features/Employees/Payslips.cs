using Payroll.Features.Payslips;

namespace Payroll.Features.Employees;

public static class Payslips
{
    public static async Task<IResult> Handle(Guid id, PayrollDbContext db, CancellationToken ct)
    {
        var employee = await db
            .Employees.AsNoTracking()
            .Include(e => e.Payslips)
            .FirstOrDefaultAsync(e => e.EmployeeId == new EmployeeId(id), ct); // TODO: Add TagWith later on

        if (employee is null)
            return TypedResults.NotFound();

        var payslips = employee
            .Payslips.OrderByDescending(p => p.Period.Start)
            .Select(p => new PayslipResponse(
                p.Id,
                p.Period.Start,
                p.Period.End,
                p.NetPay.Amount,
                p.TotalEarnings.Amount,
                p.TotalDeductions.Amount,
                Items: []
            ))
            .ToList();

        return TypedResults.Ok(payslips);
    }
}
