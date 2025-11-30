namespace Payroll.Domain.ValueObjects;

public readonly record struct EmployeeId(Guid Value)
{
    public static EmployeeId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
