namespace Payroll.Domain.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency = "USD")
{
    public static Money Zero(string currency = "USD") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money(Amount - other.Amount, Currency);
    }
}
