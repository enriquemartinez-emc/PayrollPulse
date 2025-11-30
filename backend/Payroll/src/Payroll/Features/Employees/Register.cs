namespace Payroll.Features.Employees;

public static class Register
{
    public sealed record RegisterEmployeeRequest(
        string FirstName,
        string LastName,
        string Email,
        decimal BaseSalary,
        Guid DepartmentId,
        Guid[] Policies
    );

    public sealed class RequestValidator : AbstractValidator<RegisterEmployeeRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);

            RuleFor(x => x.BaseSalary).GreaterThan(0);
        }
    }

    public static async Task<IResult> Handle(
        RegisterEmployeeRequest request,
        PayrollDbContext db,
        IValidator<RegisterEmployeeRequest> validator,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var department = await db.Departments.FindAsync(
            [request.DepartmentId],
            cancellationToken: ct
        );

        if (department is null)
            return TypedResults.BadRequest($"Department with ID {request.DepartmentId} not found.");

        var employeeResult = Employee.Create(
            EmployeeId.New(),
            request.FirstName,
            request.LastName,
            request.Email,
            new Money(request.BaseSalary),
            department,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        if (employeeResult.IsFailure)
            return TypedResults.BadRequest(employeeResult.Error);

        var employee = employeeResult.Value;

        var policies = await db
            .PayrollPolicies.Where(p => request.Policies.Contains(p.Id))
            .ToListAsync(ct);

        List<EmployeePayrollPolicy> assignments = [];

        foreach (var policy in policies)
        {
            var employeePayrollPolicy = EmployeePayrollPolicy.Assign(employee.EmployeeId, policy);
            assignments.Add(employeePayrollPolicy);
            employee.AssignPolicy(employeePayrollPolicy);
        }

        await db.EmployeePayrollPolicies.AddRangeAsync(assignments, ct);

        await db.Employees.AddAsync(employee, ct);
        await db.SaveChangesAsync(ct);

        var employeeResponse = new EmployeeResponse(
            employee.EmployeeId.Value,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.BaseSalary.Amount,
            employee.Department?.Name ?? "",
            employee.HireDate
        );

        return TypedResults.Created($"/employees/{employee.EmployeeId}", employeeResponse);
    }
}
