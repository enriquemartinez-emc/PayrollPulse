using Payroll.Domain;
using Payroll.Domain.ValueObjects;

namespace UnitTests.Domain;

public class PayrollRunTests
{
    [Fact]
    public void Create_InvalidPeriod_ReturnsFailure()
    {
        var result = PayrollRun.Create(default);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void AddPayslipsAndRecalculateTotals_Works()
    {
        var period = PayPeriod.Create(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)).Value;
        var run = PayrollRun.Create(period).Value;

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
        var payslip = Payslip.Create(employee, period).Value;
        payslip.AddItem(
            PayrollPolicy.Create(
                "p",
                "d",
                CompensationType.Earning,
                CalculationType.FixedAmount,
                100m
            ),
            null,
            new Money(100m, "USD")
        );

        run.AddPayslips([payslip]);
        run.RecalculateTotals();

        Assert.Equal(100m, run.TotalEarnings.Amount);
        Assert.Equal(0m, run.TotalDeductions.Amount);
    }
}
