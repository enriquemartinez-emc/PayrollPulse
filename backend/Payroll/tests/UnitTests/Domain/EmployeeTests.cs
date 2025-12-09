using System;
using Payroll.Domain;
using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class EmployeeTests
{
    [Fact]
    public void Create_SetsFields()
    {
        // Arrange
        var id = EmployeeId.New();
        var dept = new Department("Dev");
        var baseSalary = new Money(1000m, "USD");

        // Act
        var result = Employee.Create(
            id,
            "John",
            "Doe",
            "john@example.com",
            baseSalary,
            dept,
            new DateOnly(2020, 1, 1)
        );

        // Assert
        Assert.True(result.IsSuccess);
        var emp = result.Value;
        Assert.Equal("John", emp.FirstName);
        Assert.Equal("John Doe", emp.FullName);
    }

    [Fact]
    public void Terminate_BeforeHire_ReturnsFailure()
    {
        var id = EmployeeId.New();
        var dept = new Department("Dev");
        var baseSalary = new Money(1000m, "USD");
        var emp = Employee
            .Create(
                id,
                "John",
                "Doe",
                "john@example.com",
                baseSalary,
                dept,
                new DateOnly(2020, 1, 10)
            )
            .Value;

        var result = emp.Terminate(new DateOnly(2020, 1, 1));

        Assert.True(result.IsFailure);
        Assert.Equal("Employee.TerminationBeforeHire", result.Error!.Code);
    }

    [Fact]
    public void AdjustSalary_InvalidAmount_ReturnsFailure()
    {
        var id = EmployeeId.New();
        var dept = new Department("Dev");
        var baseSalary = new Money(1000m, "USD");
        var emp = Employee
            .Create(
                id,
                "John",
                "Doe",
                "john@example.com",
                baseSalary,
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;

        var result = emp.AdjustSalary(0);

        Assert.True(result.IsFailure);
        Assert.Equal("Employee.InvalidSalary", result.Error!.Code);
    }

    [Fact]
    public void AssignPolicy_Mismatch_ReturnsFailure()
    {
        var id = EmployeeId.New();
        var dept = new Department("Dev");
        var baseSalary = new Money(1000m, "USD");
        var emp = Employee
            .Create(
                id,
                "John",
                "Doe",
                "john@example.com",
                baseSalary,
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;

        var otherEmpId = EmployeeId.New();
        var policy = PayrollPolicy.Create(
            "p",
            "d",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            100m
        );
        var assignment = EmployeePayrollPolicy.Assign(otherEmpId, policy);

        var result = emp.AssignPolicy(assignment);

        Assert.True(result.IsFailure);
        Assert.Equal("Employee.PolicyAssignmentMismatch", result.Error!.Code);
    }

    [Fact]
    public void IsActiveDuring_VariousStates()
    {
        var id = EmployeeId.New();
        var dept = new Department("Dev");
        var baseSalary = new Money(1000m, "USD");
        var emp = Employee
            .Create(
                id,
                "John",
                "Doe",
                "john@example.com",
                baseSalary,
                dept,
                new DateOnly(2020, 1, 1)
            )
            .Value;

        var period = PayPeriod.Create(new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 31)).Value;
        Assert.True(emp.IsActiveDuring(period));

        emp.Terminate(new DateOnly(2020, 1, 15));
        Assert.False(emp.IsActiveDuring(period));
    }
}
