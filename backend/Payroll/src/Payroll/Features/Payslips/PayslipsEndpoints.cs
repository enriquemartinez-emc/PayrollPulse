namespace Payroll.Features.Payslips;

public static class PayslipsEndpoints
{
    public static void MapPayslipsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payslips");

        group
            .MapGet("/{id}", Details.Handle)
            .WithName("PayslipDetails")
            .RequireAuthorization()
            .ProducesValidationProblem()
            .WithSummary("Display the details of a payslip");

        group
            .MapPost("/explain", Explain.Handle)
            .WithName("PayslipExplanation")
            .RequireAuthorization()
            .ProducesValidationProblem()
            .WithSummary("Display the explanation of a payslip");
    }
}
