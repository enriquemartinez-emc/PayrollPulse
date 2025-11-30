using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;

namespace Payroll.Domain;

public sealed class PayrollPolicy
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty; // e.g. "Income Tax Policy"
    public string Description { get; private set; } = string.Empty; // human readable for AI/explanations
    public CompensationType CompensationType { get; private set; } // Bonus or Deduction
    public CalculationType CalculationType { get; private set; } // PercentageOfBase | FixedAmount
    public decimal RateOrAmount { get; private set; } // percent or absolute
    public bool IsActive { get; private set; }

    private PayrollPolicy() { }

    private PayrollPolicy(
        Guid id,
        string name,
        string description,
        CompensationType compensationType,
        CalculationType calculationType,
        decimal rateOrAmount,
        bool isActive
    )
    {
        Id = id;
        Name = name;
        Description = description;
        CompensationType = compensationType;
        CalculationType = calculationType;
        RateOrAmount = rateOrAmount;
        IsActive = isActive;
    }

    public static PayrollPolicy Create(
        string name,
        string description,
        CompensationType compensationType,
        CalculationType calculationType,
        decimal rateOrAmount
    )
    {
        return new PayrollPolicy(
            Guid.NewGuid(),
            name,
            description,
            compensationType,
            calculationType,
            rateOrAmount,
            true
        );
    }

    public Result<Money> Apply(Money baseSalary, decimal? overrideRateOrAmount = null)
    {
        if (baseSalary == default)
            return Result.Failure<Money>(Errors.PayrollPolicy.InvalidBaseSalary);

        var effective = overrideRateOrAmount ?? RateOrAmount;

        if (Name == "Base Salary") // TODO: Refactor later
        {
            effective = baseSalary.Amount;
        }

        var amount = CalculationType switch
        {
            CalculationType.FixedAmount => effective,
            CalculationType.PercentageOfBase => baseSalary.Amount * (effective / 100m),
            _ => 0m,
        };

        return Result.Success(new Money(amount, baseSalary.Currency));
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public void UpdateDescription(string description) => Description = description;

    public void UpdateRate(decimal rateOrAmount) => RateOrAmount = rateOrAmount;

    public string ToExplanation() =>
        $"{Name}: {Description}. Type: {CompensationType}, Calculation: {CalculationType}, "
        + $"Rate/Amount: {RateOrAmount}, Active: {IsActive}";
}
