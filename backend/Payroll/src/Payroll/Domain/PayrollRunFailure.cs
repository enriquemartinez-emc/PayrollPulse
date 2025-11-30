namespace Payroll.Domain;

public sealed class PayrollRunFailure
{
    public Guid Id { get; set; }
    public Guid PayrollRunId { get; set; }
    public Guid? EmployeeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
