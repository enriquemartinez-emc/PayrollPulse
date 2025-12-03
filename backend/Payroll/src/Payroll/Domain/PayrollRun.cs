using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;

namespace Payroll.Domain;

public sealed class PayrollRun
{
    private readonly List<Payslip> _payslips = [];
    private readonly List<PayrollRunFailure> _failures = [];

    public Guid Id { get; }
    public PayPeriod Period { get; }
    public DateTime CreatedAtUtc { get; }
    public IReadOnlyCollection<Payslip> Payslips => _payslips.AsReadOnly();
    public IReadOnlyCollection<PayrollRunFailure> Failures => _failures.AsReadOnly();

    public Money TotalGross { get; private set; } = Money.Zero();
    public Money TotalNet { get; private set; } = Money.Zero();
    public Money TotalEarnings { get; private set; } = Money.Zero();
    public Money TotalDeductions { get; private set; } = Money.Zero();

    private PayrollRun() { }

    private PayrollRun(Guid id, PayPeriod period)
    {
        Id = id;
        Period = period;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Result<PayrollRun> Create(PayPeriod period)
    {
        if (period == default)
        {
            return Result.Failure<PayrollRun>(Errors.Payroll.InvalidPeriod("Period is missing"));
        }

        return Result.Success(new PayrollRun(Guid.NewGuid(), period));
    }

    public void AddPayslips(IEnumerable<Payslip>? payslips)
    {
        if (payslips == null)
            return;
        _payslips.AddRange(payslips);
    }

    public void AddFailures(IEnumerable<PayrollRunFailure>? failures)
    {
        if (failures == null)
            return;
        _failures.AddRange(failures);
    }

    public void RecalculateTotals()
    {
        TotalGross = Sum(p => p.BaseSalary);
        TotalEarnings = Sum(p => p.TotalEarnings);
        TotalDeductions = Sum(p => p.TotalDeductions);
        TotalNet = Sum(p => p.NetPay);
    }

    private Money Sum(Func<Payslip, Money> selector)
    {
        var total = Money.Zero();
        foreach (var payslip in _payslips)
            total = total.Add(selector(payslip));

        return total;
    }
}
