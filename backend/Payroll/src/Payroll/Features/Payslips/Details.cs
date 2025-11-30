namespace Payroll.Features.Payslips;

public static class Details
{
    public static async Task<IResult> Handle(Guid id, PayrollDbContext db, CancellationToken ct)
    {
        var payslip = await db
            .Payslips.AsNoTracking()
            .Include(p => p.Items)
            .ThenInclude(i => i.PayrollPolicy)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (payslip is null)
            return TypedResults.NotFound();

        var response = new PayslipResponse(
            payslip.Id,
            payslip.Period.Start,
            payslip.Period.End,
            payslip.NetPay.Amount,
            payslip.TotalBonuses.Amount,
            payslip.TotalDeductions.Amount,
            [
                .. payslip
                    .Items.OrderBy(i => i.CompensationType.ToString())
                    .Select(i => new PayslipItemResponse(
                        i.CompensationType.ToString(),
                        i.Amount.Amount,
                        i.PayrollPolicy?.Name ?? "",
                        i.PayrollPolicy?.Description ?? ""
                    )),
            ]
        );

        return TypedResults.Ok(response);
    }
}
