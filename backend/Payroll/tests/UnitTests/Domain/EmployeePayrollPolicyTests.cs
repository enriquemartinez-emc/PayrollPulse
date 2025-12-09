using System;
using Payroll.Domain;
using Payroll.Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class EmployeePayrollPolicyTests
{
    [Fact]
    public void Assign_SetsProperties()
    {
        var empId = EmployeeId.New();
        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            100m
        );
        var assignment = EmployeePayrollPolicy.Assign(empId, policy, 50m);

        Assert.Equal(empId, assignment.EmployeeId);
        Assert.Equal(policy.Id, assignment.PayrollPolicyId);
        Assert.Equal(50m, assignment.OverrideRateOrAmount);
    }

    [Fact]
    public void IsActiveForPeriod_RespectsEffectiveDatesAndActiveFlag()
    {
        var empId = EmployeeId.New();
        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            100m
        );
        var assignment = EmployeePayrollPolicy.Assign(
            empId,
            policy,
            null,
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 12, 31)
        );

        var period = PayPeriod.Create(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 30)).Value;
        Assert.True(assignment.IsActiveForPeriod(period));

        assignment.Deactivate();
        Assert.False(assignment.IsActiveForPeriod(period));
    }
}
