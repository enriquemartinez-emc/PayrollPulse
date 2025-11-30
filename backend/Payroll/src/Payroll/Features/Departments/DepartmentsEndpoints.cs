namespace Payroll.Features.Departments;

public static class DepartmentsEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/departments").WithTags("Departments");

        group.MapGet("/", List.Handle).WithName("GetDepartments");
    }
}
