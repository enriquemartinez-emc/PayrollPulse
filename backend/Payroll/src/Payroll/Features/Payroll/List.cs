namespace Payroll.Features.Payroll;

public static class List
{
    public static async Task<IResult> Handle(
        [AsParameters] PaginationQuery query,
        PayrollDbContext db,
        CancellationToken ct = default
    )
    {
        var payrollQuery = db
            .PayrollRuns.AsNoTracking()
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new PayrollResponse(
                r.Id,
                r.Period.Start,
                r.Period.End,
                r.Payslips.Count,
                r.TotalGross.Amount,
                r.TotalNet.Amount,
                r.TotalBonuses.Amount,
                r.TotalDeductions.Amount,
                r.CreatedAtUtc
            ));
        ;

        var paginated = await payrollQuery.ToPaginatedListAsync(query, ct);

        return TypedResults.Ok(paginated);
    }
}
