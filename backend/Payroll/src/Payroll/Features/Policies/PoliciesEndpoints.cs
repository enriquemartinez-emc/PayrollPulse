namespace Payroll.Features.Policies;

public static class PoliciesEndpoints
{
    public static void MapPoliciesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/policies").WithTags("Policies");

        group.MapGet("/", List.Handle).WithName("GetPolicies");
    }
}
