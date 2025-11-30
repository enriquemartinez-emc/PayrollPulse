using Payroll;
using Payroll.Features.Auth;
using Payroll.Features.Departments;
using Payroll.Features.Employees;
using Payroll.Features.Payroll;
using Payroll.Features.Payslips;
using Payroll.Features.Policies;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

builder
    .Configuration.AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();

builder.AddConfiguredServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseExceptionHandler();

app.MapEmployeeEndpoints();
app.MapPayrollEndpoints();
app.MapDepartmentEndpoints();
app.MapAuthEndpoints();
app.MapPayslipsEndpoints();
app.MapPoliciesEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayrollDbContext>();
    await DbInitializer.InitializeAsync(db);
}

app.Run();
