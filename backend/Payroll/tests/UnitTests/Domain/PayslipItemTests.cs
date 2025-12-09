using Payroll.Domain;
using Payroll.Domain.ValueObjects;

namespace UnitTests.Domain;

public class PayslipItemTests
{
    [Fact]
    public void Create_InactivePolicy_ReturnsFailure()
    {
        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            50m
        );
        policy.Deactivate();

        var result = PayslipItem.Create(Guid.NewGuid(), policy, null, new Money(50m, "USD"));

        Assert.True(result.IsFailure);
        Assert.Equal("PayrollPolicy.PolicyInactive", result.Error!.Code);
    }

    [Fact]
    public void Create_ActivePolicy_ReturnsSuccess()
    {
        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            50m
        );
        var result = PayslipItem.Create(Guid.NewGuid(), policy, null, new Money(50m, "USD"));

        Assert.True(result.IsSuccess);
        Assert.Equal(50m, result.Value.Amount.Amount);
    }
}
