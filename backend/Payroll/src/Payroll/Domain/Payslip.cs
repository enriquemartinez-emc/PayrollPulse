using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;

namespace Payroll.Domain;

public sealed class Payslip
{
    private readonly List<PayslipItem> _items = [];

    public Guid Id { get; }
    public EmployeeId EmployeeId { get; }
    public string EmployeeName { get; } = string.Empty;
    public PayPeriod Period { get; }
    public Money BaseSalary { get; }
    public IReadOnlyCollection<PayslipItem> Items => _items.AsReadOnly();
    public Money NetPay { get; private set; }
    public Money TotalBonuses { get; private set; }
    public Money TotalDeductions { get; private set; }

    public Guid PayrollRunId { get; private set; }
    public PayrollRun? PayrollRun { get; private set; }

    private Payslip() { }

    private Payslip(
        Guid id,
        EmployeeId employeeId,
        string employeeName,
        PayPeriod period,
        Money baseSalary
    )
    {
        Id = id;
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        Period = period;
        BaseSalary = baseSalary;
        NetPay = Money.Zero(baseSalary.Currency); //baseSalary; TODO: Review for better approach
        TotalBonuses = Money.Zero(baseSalary.Currency);
        TotalDeductions = Money.Zero(baseSalary.Currency);
    }

    public static Result<Payslip> Create(Employee employee, PayPeriod period)
    {
        var payslip = new Payslip(
            Guid.NewGuid(),
            employee.EmployeeId,
            employee.FullName,
            period,
            employee.BaseSalary
        );

        return Result.Success(payslip);
    }

    public Result AddItem(PayrollPolicy policy, decimal? overrideRateOrAmount, Money amount)
    {
        if (_items.Any(i => i.PolicyId == policy.Id))
            return Result.Failure(Errors.Payslip.DuplicatePolicy(policy.Id));

        var maybeItem = PayslipItem.Create(Id, policy, overrideRateOrAmount, amount);
        if (maybeItem.IsFailure)
            return Result.Failure(maybeItem.Error!);

        var item = maybeItem.Value!;

        _items.Add(item);

        if (policy.CompensationType == CompensationType.Bonus)
        {
            TotalBonuses = TotalBonuses.Add(amount);
            NetPay = NetPay.Add(amount);
        }
        else
        {
            TotalDeductions = TotalDeductions.Add(amount);
            NetPay = NetPay.Subtract(amount);
        }

        return Result.Success();
    }
}
