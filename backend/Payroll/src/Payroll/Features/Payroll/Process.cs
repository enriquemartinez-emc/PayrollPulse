using Payroll.Domain.Services;

namespace Payroll.Features.Payroll;

public static class Process
{
    public sealed record ProcessPayrollRequest(DateOnly StartDate, DateOnly EndDate);

    public sealed class ProcessPayrollRequestValidator : AbstractValidator<ProcessPayrollRequest>
    {
        public ProcessPayrollRequestValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Start date must be before end date.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");
        }
    }

    public static async Task<IResult> Handle(
        ProcessPayrollRequest request,
        PayrollDbContext db,
        IValidator<ProcessPayrollRequest> validator,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var employees = await db
            .Employees.Include(e => e.EmployeePayrollPolicies)
            .ThenInclude(ep => ep.PayrollPolicy)
            .Where(e => e.Status == EmploymentStatus.Active)
            .ToListAsync(ct);

        if (employees.Count == 0)
            return TypedResults.BadRequest("No active employees found.");

        var periodResult = PayPeriod.Create(request.StartDate, request.EndDate);
        if (periodResult.IsFailure)
            return TypedResults.BadRequest(periodResult.Error);

        var period = periodResult.Value;

        var payrollResult = PayrollProcessor.GeneratePayroll(employees, period);
        if (payrollResult.IsFailure)
            return TypedResults.BadRequest(payrollResult.Error);

        var payrollRun = payrollResult.Value;

        await db.PayrollRuns.AddAsync(payrollRun, ct);
        await db.SaveChangesAsync(ct);

        var response = new PayrollResponse(
            payrollRun.Id,
            payrollRun.Period.Start,
            payrollRun.Period.End,
            payrollRun.Payslips.Count,
            payrollRun.TotalGross.Amount,
            payrollRun.TotalNet.Amount,
            payrollRun.TotalBonuses.Amount,
            payrollRun.TotalDeductions.Amount,
            payrollRun.CreatedAtUtc
        );

        return TypedResults.Ok(response);
    }
}
