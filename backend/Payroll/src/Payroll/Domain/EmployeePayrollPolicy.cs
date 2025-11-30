using Payroll.Domain.ValueObjects;

namespace Payroll.Domain;

public sealed class EmployeePayrollPolicy
{
    public Guid Id { get; private set; }
    public EmployeeId EmployeeId { get; private set; }

    public Guid PayrollPolicyId { get; private set; }
    public PayrollPolicy PayrollPolicy { get; private set; } = default!;

    public decimal? OverrideRateOrAmount { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }

    private EmployeePayrollPolicy() { } // EF

    private EmployeePayrollPolicy(
        Guid id,
        EmployeeId employeeId,
        PayrollPolicy policy,
        decimal? overrideRateOrAmount,
        DateOnly? effectiveFrom,
        DateOnly? effectiveTo
    )
    {
        Id = id;
        EmployeeId = employeeId;
        PayrollPolicyId = policy.Id;
        PayrollPolicy = policy;
        OverrideRateOrAmount = overrideRateOrAmount;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public static EmployeePayrollPolicy Assign(
        EmployeeId employeeId,
        PayrollPolicy policy,
        decimal? overrideRateOrAmount = null,
        DateOnly? effectiveFrom = null,
        DateOnly? effectiveTo = null
    ) => new(Guid.NewGuid(), employeeId, policy, overrideRateOrAmount, effectiveFrom, effectiveTo);

    public bool IsActiveForPeriod(PayPeriod period)
    {
        if (!IsActive)
            return false;
        if (EffectiveFrom.HasValue && EffectiveFrom.Value > period.End)
            return false;
        if (EffectiveTo.HasValue && EffectiveTo.Value < period.Start)
            return false;
        return true;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public void UpdateOverride(decimal? overrideValue) => OverrideRateOrAmount = overrideValue;
}
