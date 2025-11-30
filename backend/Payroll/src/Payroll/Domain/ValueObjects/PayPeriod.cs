using Payroll.Domain.Common;

namespace Payroll.Domain.ValueObjects;

public readonly record struct PayPeriod(DateOnly Start, DateOnly End)
{
    public static Result<PayPeriod> Create(DateOnly start, DateOnly end)
    {
        if (end <= start)
        {
            return Result.Failure<PayPeriod>(
                Errors.Payroll.InvalidPayPeriod("End date must be after start date")
            );
        }

        return Result.Success(new PayPeriod(start, end));
    }

    public int TotalDays => End.DayNumber - Start.DayNumber + 1;

    public override string ToString() => $"{Start:yyyy-MM-dd} - {End:yyyy-MM-dd}";
}
