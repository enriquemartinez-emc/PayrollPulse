namespace Payroll.Features.Payroll;

public static class PayrollEndpoints
{
    public static void MapPayrollEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payroll");

        group
            .MapGet("", List.Handle)
            .WithName("ListPayrollruns")
            .RequireAuthorization("role:admin")
            .WithSummary("List all payroll runs");

        group
            .MapPost("/process", Process.Handle)
            .WithName("ProcessPayroll")
            .ProducesValidationProblem()
            .RequireAuthorization("role:admin")
            .WithSummary("Generates a new payroll for a given period");
    }
}
