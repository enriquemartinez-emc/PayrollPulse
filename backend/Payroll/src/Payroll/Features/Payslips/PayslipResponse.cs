namespace Payroll.Features.Payslips;

public sealed record PayslipResponse(
    Guid Id,
    DateOnly Start,
    DateOnly End,
    decimal NetPay,
    decimal TotalEarnings,
    decimal TotalDeductions,
    IReadOnlyList<PayslipItemResponse> Items
);

public sealed record PayslipItemResponse(
    string CompensationType,
    decimal Amount,
    string PolicyName,
    string PolicyDescription
);
