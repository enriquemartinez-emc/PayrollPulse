namespace Payroll.Features.Payroll;

public sealed record PayrollResponse(
    Guid PayrollRunId,
    DateOnly StartDate,
    DateOnly EndDate,
    int TotalEmployeesProcessed,
    decimal TotalGross,
    decimal TotalNet,
    decimal TotalEarnings,
    decimal TotalDeductions,
    DateTime CreatedAtUtc
);
