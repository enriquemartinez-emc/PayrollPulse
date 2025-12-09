using System;
using Payroll.Domain;
using Payroll.Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class PayslipTests
{
    [Fact]
    public void Create_SetsInitialValues()
    {
        var empId = EmployeeId.New();
        var dept = new Department("Dev");
        var employee = Employee
            .Create(
                empId,
                "Jane",
                "Doe",
                "jane@example.com",
                new Money(1000m, "USD"),
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;
        var period = PayPeriod.Create(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 31)).Value;

        var result = Payslip.Create(employee, period);

        Assert.True(result.IsSuccess);
        var payslip = result.Value;
        Assert.Equal(employee.EmployeeId, payslip.EmployeeId);
        Assert.Equal(0m, payslip.NetPay.Amount);
    }

    [Fact]
    public void AddItem_DuplicatePolicy_ReturnsFailure()
    {
        var empId = EmployeeId.New();
        var dept = new Department("Dev");
        var employee = Employee
            .Create(
                empId,
                "Jane",
                "Doe",
                "jane@example.com",
                new Money(1000m, "USD"),
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;
        var period = PayPeriod.Create(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 31)).Value;
        var payslip = Payslip.Create(employee, period).Value;

        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            100m
        );
        payslip.AddItem(policy, null, new Money(100m, "USD"));
        var result = payslip.AddItem(policy, null, new Money(100m, "USD"));

        Assert.True(result.IsFailure);
        Assert.Equal("Payslip.DuplicatePolicy", result.Error!.Code);
    }

    [Fact]
    public void AddItem_Earning_AdjustsTotals()
    {
        var empId = EmployeeId.New();
        var dept = new Department("Dev");
        var employee = Employee
            .Create(
                empId,
                "Jane",
                "Doe",
                "jane@example.com",
                new Money(1000m, "USD"),
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;
        var period = PayPeriod.Create(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 31)).Value;
        var payslip = Payslip.Create(employee, period).Value;

        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            100m
        );
        var result = payslip.AddItem(policy, null, new Money(100m, "USD"));

        Assert.True(result.IsSuccess);
        Assert.Equal(100m, payslip.TotalEarnings.Amount);
        Assert.Equal(100m, payslip.NetPay.Amount);
    }

    [Fact]
    public void AddItem_Deduction_AdjustsTotals()
    {
        var empId = EmployeeId.New();
        var dept = new Department("Dev");
        var employee = Employee
            .Create(
                empId,
                "Jane",
                "Doe",
                "jane@example.com",
                new Money(1000m, "USD"),
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;
        var period = PayPeriod.Create(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 31)).Value;
        var payslip = Payslip.Create(employee, period).Value;

        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Deduction,
            CalculationType.FixedAmount,
            100m
        );
        var result = payslip.AddItem(policy, null, new Money(100m, "USD"));

        Assert.True(result.IsSuccess);
        Assert.Equal(100m, payslip.TotalDeductions.Amount);
        Assert.Equal(-100m, payslip.NetPay.Amount);
    }
}
