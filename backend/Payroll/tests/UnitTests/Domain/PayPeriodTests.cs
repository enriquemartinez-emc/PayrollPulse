using System;
using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class PayPeriodTests
{
    [Fact]
    public void Create_InvalidEndBeforeOrEqualStart_ReturnsFailure()
    {
        // Arrange
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 9);

        // Act
        var result = PayPeriod.Create(start, end);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Payroll.InvalidPayPeriod", result.Error!.Code);
    }

    [Fact]
    public void Create_ValidPeriod_ReturnsSuccessAndDays()
    {
        // Arrange
        var start = new DateOnly(2025, 1, 1);
        var end = new DateOnly(2025, 1, 31);

        // Act
        var result = PayPeriod.Create(start, end);

        // Assert
        Assert.True(result.IsSuccess);
        var period = result.Value;
        Assert.Equal(31, period.TotalDays);
        Assert.Equal("2025-01-01 - 2025-01-31", period.ToString());
    }
}
