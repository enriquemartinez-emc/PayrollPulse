using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;

namespace Payroll.Domain;

public sealed class PayslipItem
{
    public Guid Id { get; private set; }

    public Guid PayslipId { get; private set; }
    public Payslip? Payslip { get; private set; }

    public Guid PolicyId { get; private set; }
    public PayrollPolicy? PayrollPolicy { get; private set; }

    public string PolicyName { get; private set; } = string.Empty;

    public CompensationType CompensationType { get; private set; }
    public decimal? OverrideRateOrAmount { get; private set; }
    public Money Amount { get; private set; }

    private PayslipItem() { } // EF Core

    private PayslipItem(
        Guid payslipId,
        PayrollPolicy policy,
        decimal? overrideRateOrAmount,
        Money amount
    )
    {
        Id = Guid.NewGuid();
        PayslipId = payslipId;
        PolicyId = policy.Id;
        PolicyName = policy.Name;
        CompensationType = policy.CompensationType;
        OverrideRateOrAmount = overrideRateOrAmount;
        Amount = amount;
    }

    public static Result<PayslipItem> Create(
        Guid payslipId,
        PayrollPolicy policy,
        decimal? overrideRateOrAmount,
        Money amount
    )
    {
        //if (policy == null)
        //    return Result.Failure<PayslipItem>(
        //        Errors.Compensation.PolicyNotAssigned()
        //    );

        if (!policy.IsActive)
            return Result.Failure<PayslipItem>(Errors.PayrollPolicy.PolicyInactive(policy.Id));

        var item = new PayslipItem(payslipId, policy, overrideRateOrAmount, amount);

        return Result.Success(item);
    }
}
