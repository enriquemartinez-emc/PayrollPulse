namespace Payroll.Features.Employees;

public static class EmployeesEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees");

        group
            .MapPost("", Register.Handle)
            .WithName("RegisterEmployee")
            .ProducesValidationProblem()
            .RequireAuthorization("role:admin")
            .WithSummary("Registers a new employee");

        group
            .MapGet("", List.Handle)
            .WithName("ListEmployees")
            .RequireAuthorization("role:admin")
            .WithSummary("Lists employees with pagination and optional sorting");

        group
            .MapGet("/{id:guid}/payslips", Payslips.Handle)
            .WithName("ListEmployeesPayslips")
            .RequireAuthorization()
            .WithSummary("Lists employee's payslips");

        group
            .MapGet("/{id:guid}", Details.Handle)
            .WithName("GetEmployeeDetails")
            .RequireAuthorization()
            .WithSummary("Lists employee's details");

        group
            .MapGet("/search", Search.Handle)
            .WithName("SearchForEmployee")
            .RequireAuthorization("role:admin")
            .WithSummary("Search for an employee");
    }
}
