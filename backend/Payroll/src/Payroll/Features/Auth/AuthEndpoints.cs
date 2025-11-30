namespace Payroll.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/login", Login.Handle);

        auth.MapPost("/register", Register.Handle).RequireAuthorization("role:admin");

        auth.MapGet("/me", Me.Handle).RequireAuthorization();

        auth.MapGet("/users", ListUsers.Handle).RequireAuthorization("role:admin");
    }
}
