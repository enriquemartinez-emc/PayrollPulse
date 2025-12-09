using Payroll.Domain;
using Payroll.Domain.Services;
using Payroll.Domain.ValueObjects;

namespace UnitTests.Services;

public class PayrollProcessorTests
{
    [Fact]
    public void GeneratePayslip_ForEmployeeWithoutPolicies_ReturnsPayslipWithZeroes()
    {
        // Arrange
        var empId = EmployeeId.New();
        var dept = new Department("Dev");
        var employee = Employee
            .Create(
                empId,
                "A",
                "B",
                "a@b.com",
                new Money(1000m, "USD"),
                dept,
                new DateOnly(2025, 1, 1)
            )
            .Value;
        var period = PayPeriod.Create(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)).Value;

        // Act
        var result = PayrollProcessor.GeneratePayroll([employee], period);

        // Assert
        Assert.True(result.IsSuccess);
        var run = result.Value;
        Assert.Single(run.Payslips);
        Assert.Equal(0m, run.TotalEarnings.Amount);
    }

    [Fact]
    public void GeneratePayroll_MultipleEmployees_ProducesRun()
    {
        var emp1 = Employee
            .Create(
                EmployeeId.New(),
                "A",
                "B",
                "a@b.com",
                new Money(1000m, "USD"),
                new Department("Dev"),
                new DateOnly(2025, 1, 1)
            )
            .Value;
        var emp2 = Employee
            .Create(
                EmployeeId.New(),
                "C",
                "D",
                "c@d.com",
                new Money(2000m, "USD"),
                new Department("Dev"),
                new DateOnly(2025, 1, 1)
            )
            .Value;
        var period = PayPeriod.Create(new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)).Value;

        var result = PayrollProcessor.GeneratePayroll([emp1, emp2], period);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Payslips.Count);
    }
}
