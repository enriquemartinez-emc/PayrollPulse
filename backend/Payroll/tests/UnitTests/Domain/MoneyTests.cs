using System;
using Payroll.Domain.ValueObjects;
using Xunit;

namespace UnitTests.Domain;

public class MoneyTests
{
    [Fact]
    public void Zero_DefaultsCurrencyAndAmount()
    {
        // Arrange & Act
        var zero = Money.Zero("USD");

        // Assert
        Assert.Equal(0m, zero.Amount);
        Assert.Equal("USD", zero.Currency);
    }

    [Fact]
    public void Add_Subtract_WithSameCurrency_Works()
    {
        // Arrange
        var a = new Money(100m, "USD");
        var b = new Money(50m, "USD");

        // Act
        var sum = a.Add(b);
        var diff = a.Subtract(b);

        // Assert
        Assert.Equal(150m, sum.Amount);
        Assert.Equal(50m, diff.Amount);
    }

    [Fact]
    public void Add_DifferentCurrency_Throws()
    {
        // Arrange
        var a = new Money(10m, "USD");
        var b = new Money(5m, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => a.Add(b));
        Assert.Throws<InvalidOperationException>(() => a.Subtract(b));
    }
}
