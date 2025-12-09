using System;
using Payroll.Domain;
using Payroll.Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class PayrollPolicyTests
{
    [Fact]
    public void Create_SetsProperties()
    {
        // Arrange & Act
        var policy = PayrollPolicy.Create(
            "Tax",
            "Tax desc",
            CompensationType.Deduction,
            CalculationType.PercentageOfBase,
            10m
        );

        // Assert
        Assert.Equal("Tax", policy.Name);
        Assert.Equal(10m, policy.RateOrAmount);
        Assert.True(policy.IsActive);
    }

    [Fact]
    public void Apply_FixedAmount_ReturnsAmount()
    {
        // Arrange
        var policy = PayrollPolicy.Create(
            "Bonus",
            "desc",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            200m
        );
        var baseSalary = new Money(1000m, "USD");

        // Act
        var result = policy.Apply(baseSalary);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(200m, result.Value.Amount);
    }

    [Fact]
    public void Apply_PercentageOfBase_ReturnsCalculatedAmount()
    {
        var policy = PayrollPolicy.Create(
            "Tax",
            "desc",
            CompensationType.Deduction,
            CalculationType.PercentageOfBase,
            10m
        );
        var baseSalary = new Money(2000m, "USD");

        var result = policy.Apply(baseSalary);

        Assert.True(result.IsSuccess);
        Assert.Equal(200m, result.Value.Amount);
    }

    [Fact]
    public void Apply_InvalidBaseSalary_ReturnsFailure()
    {
        var policy = PayrollPolicy.Create(
            "Tax",
            "desc",
            CompensationType.Deduction,
            CalculationType.PercentageOfBase,
            10m
        );
        var result = policy.Apply(default);

        Assert.True(result.IsFailure);
        Assert.Equal("PayrollPolicy.InvalidBaseSalary", result.Error!.Code);
    }

    [Fact]
    public void ToExplanation_IncludesKeyParts()
    {
        var policy = PayrollPolicy.Create(
            "Name",
            "Description",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            123m
        );
        var explanation = policy.ToExplanation();

        Assert.Contains("Name", explanation);
        Assert.Contains("Description", explanation);
        Assert.Contains("Rate/Amount", explanation);
    }
}
