namespace Payroll.Features.Auth;

public static partial class Login
{
    public sealed record LoginRequest(string Email, string Password);

    public sealed record LoginResponse(string Token, Guid UserId, string Role, Guid? EmployeeId);

    public sealed class RequestValidator : AbstractValidator<LoginRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);

            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public static async Task<IResult> Handle(
        LoginRequest request,
        PayrollDbContext db,
        Jwt jwt,
        IValidator<LoginRequest> validator,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct); // TODO: Use a filter so we don't repeat this
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var user = await db
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null)
            return TypedResults.Unauthorized();

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            return TypedResults.Unauthorized();

        var token = jwt.GenerateToken(user);

        return TypedResults.Ok(
            new LoginResponse(token, user.Id, user.Role, user.EmployeeId?.Value)
        );
    }
}
