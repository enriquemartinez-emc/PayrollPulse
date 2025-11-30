using System.Security.Claims;

namespace Payroll.Features.Auth;

public static class Register
{
    public sealed record RegisterUserRequest(
        string Email,
        string Password,
        string Role,
        Guid? EmployeeId
    );

    public sealed record UserResponse(Guid Id, string Email, string Role, Guid? EmployeeId);

    public sealed class RequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Role).NotEmpty();
        }
    }

    public static async Task<IResult> Handle(
        RegisterUserRequest request,
        ClaimsPrincipal currentUser,
        PayrollDbContext db,
        IValidator<RegisterUserRequest> validator,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        if (!currentUser.IsAuthenticated() || !currentUser.IsInRole(Roles.Admin))
            return TypedResults.Forbid();

        var exists = await db.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (exists)
            return TypedResults.Conflict(new { Message = "Email already used" });

        var (hash, salt) = PasswordHasher.HashPassword(request.Password);

        User user;

        if (request.EmployeeId.HasValue)
        {
            var employee = await db.Employees.FindAsync(
                [new EmployeeId(request.EmployeeId.Value)],
                ct
            );

            if (employee is null)
                return TypedResults.NotFound(new { Message = "Employee not found" });

            user = User.CreateForEmployee(
                new EmployeeId(request.EmployeeId.Value),
                hash,
                salt,
                request.Role
            );

            user.SetEmail(employee.Email);
        }
        else
        {
            user = User.CreateStandalone(request.Email, hash, salt, request.Role);
        }

        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);

        var response = new UserResponse(user.Id, user.Email, user.Role, user.EmployeeId?.Value);

        return TypedResults.Created($"/api/users/{user.Id}", response);
    }
}
