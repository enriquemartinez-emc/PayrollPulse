namespace Payroll.Features.Payroll;

public sealed record PayrollResponse(
    Guid PayrollRunId,
    DateOnly StartDate,
    DateOnly EndDate,
    int TotalEmployeesProcessed,
    decimal TotalGross,
    decimal TotalNet,
    decimal TotalBonuses,
    decimal TotalDeductions,
    DateTime CreatedAtUtc
);
