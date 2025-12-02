using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Payroll.Infrastructure.AI;

namespace Payroll.Features.Payslips;

public static class Explain
{
    public sealed record ExplainRequest(Guid PayslipId, string Message);

    public sealed class RequestValidator : AbstractValidator<ExplainRequest>
    {
        public RequestValidator()
        {
            RuleFor(x => x.PayslipId).NotEmpty();
            RuleFor(x => x.Message).NotEmpty().MaximumLength(1000);
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task<IResult> Handle(
        ExplainRequest request,
        ClaimsPrincipal currentUser,
        PayrollDbContext db,
        Kernel kernel,
        IValidator<ExplainRequest> validator,
        IConfiguration config,
        IWebHostEnvironment env,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var payslip = await db
            .Payslips.AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.PayrollPolicy)
            .FirstOrDefaultAsync(x => x.Id == request.PayslipId, ct);

        if (payslip == null)
            return TypedResults.NotFound();

        if (!currentUser.IsAdmin())
        {
            var employeeId = await db
                .Users.Where(u => u.Id == currentUser.GetUserId())
                .Select(u => u.EmployeeId)
                .FirstOrDefaultAsync(ct);

            if (payslip.EmployeeId != employeeId)
                return TypedResults.Forbid();
        }

        var relativePath = config["AI:PromptPath"] ?? "";
        var fullPath = Path.Combine(env.ContentRootPath, relativePath);
        var template = await File.ReadAllTextAsync(fullPath, ct);

        var aiContext = new PayslipForAI
        {
            Id = payslip.Id,
            EmployeeId = payslip.EmployeeId.Value,
            NetPay = payslip.NetPay.Amount,
            TotalBonuses = payslip.TotalBonuses.Amount,
            TotalDeductions = payslip.TotalDeductions.Amount,
            Items =
            [
                .. payslip.Items.Select(i => new ItemForAI
                {
                    Name = i.PolicyName,
                    Amount = i.Amount.Amount,
                    PolicyName = i.PayrollPolicy?.Name ?? "",
                    PolicyDescription = i.PayrollPolicy?.Description ?? "",
                    PolicyCalculationType = i.PayrollPolicy?.CalculationType.ToString() ?? "",
                    PolicyRateOrAmount = i.PayrollPolicy?.RateOrAmount.ToString() ?? "",
                }),
            ],
        };

        var vars = new KernelArguments
        {
            ["user_message"] = request.Message,
            ["payslip_json"] = JsonSerializer.Serialize(aiContext, JsonOptions),
        };

        var resultStream = kernel.InvokePromptStreamingAsync(template, vars, cancellationToken: ct);

        return TypedResults.Stream(
            async (stream) =>
            {
                try
                {
                    await foreach (var chunk in resultStream.WithCancellation(ct))
                    {
                        if (chunk?.ToString() is { } content && !string.IsNullOrWhiteSpace(content))
                        {
                            var data = Encoding.UTF8.GetBytes($"data: {chunk}\n\n");
                            await stream.WriteAsync(data);
                        }
                    }
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    // Graceful cancellation
                }
                catch (Exception)
                {
                    var errorData = Encoding.UTF8.GetBytes(
                        "data: Error occurred during generation\n\n"
                    );
                    await stream.WriteAsync(errorData, ct);
                }
            },
            contentType: "text/event-stream"
        );
    }
}
