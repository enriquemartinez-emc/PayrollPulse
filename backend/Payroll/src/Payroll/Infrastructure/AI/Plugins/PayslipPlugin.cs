using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;

namespace Payroll.Infrastructure.AI.Plugins;

public class PayslipPlugin
{
    private readonly PayrollDbContext _db;

    public PayslipPlugin(PayrollDbContext db)
    {
        _db = db;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [KernelFunction("get_payslip_json")]
    [Description("Returns structured JSON for a payslip, including items and policies.")]
    public async Task<string> GetPayslipJsonAsync(Guid payslipId, CancellationToken ct)
    {
        var payslip = await _db
            .Payslips.AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.PayrollPolicy)
            .FirstOrDefaultAsync(x => x.Id == payslipId, ct);

        if (payslip is null)
            return "{}";

        var aiContext = new PayslipForAI
        {
            Id = payslip.Id,
            EmployeeId = payslip.EmployeeId!.Value,
            NetPay = payslip.NetPay.Amount,
            TotalEarnings = payslip.TotalEarnings.Amount,
            TotalDeductions = payslip.TotalDeductions.Amount,
            Items =
            [
                .. payslip.Items.Select(i => new ItemForAI
                {
                    Name = i.PolicyName,
                    Amount = i.Amount.Amount,
                    PolicyName = i.PayrollPolicy?.Name ?? "",
                    PolicyDescription = i.PayrollPolicy?.Description ?? "",
                    PolicyCompensationType = i.PayrollPolicy?.CompensationType.ToString() ?? "",
                    PolicyCalculationType = i.PayrollPolicy?.CalculationType.ToString() ?? "",
                    PolicyRateOrAmount = i.PayrollPolicy?.RateOrAmount.ToString() ?? "",
                }),
            ],
        };

        return JsonSerializer.Serialize(aiContext, options: JsonOptions);
    }

    [KernelFunction("explain_item")]
    [Description("Explains a specific payslip item with its calculation rationale.")]
    public async Task<string> ExplainItemAsync(
        Guid payslipId,
        string itemName,
        CancellationToken ct
    )
    {
        var item = await _db
            .Payslips.AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.PayrollPolicy)
            .Where(p => p.Id == payslipId)
            .SelectMany(p => p.Items)
            .FirstOrDefaultAsync(i => i.PolicyName == itemName, ct);

        if (item is null)
            return $"No item named '{itemName}' was found.";

        var policy = item.PayrollPolicy;
        var details = new
        {
            item = item.PolicyName,
            amount = item.Amount.Amount,
            policyName = policy?.Name,
            description = policy?.Description,
            calculationType = policy?.CalculationType.ToString(),
            rateOrAmount = policy?.RateOrAmount.ToString(),
        };

        return $"Item explanation:\n{JsonSerializer.Serialize(details)}";
    }

    [KernelFunction("list_items")]
    [Description("Lists all items in the payslip with amounts.")]
    public async Task<string> ListItemsAsync(Guid payslipId, CancellationToken ct)
    {
        var items = await _db
            .Payslips.AsNoTracking()
            .Include(x => x.Items)
            .Where(p => p.Id == payslipId)
            .SelectMany(p => p.Items)
            .Select(i => new { name = i.PolicyName, amount = i.Amount.Amount })
            .ToListAsync(ct);

        if (items.Count == 0)
            return "No items found.";

        return JsonSerializer.Serialize(items, options: JsonOptions);
    }
}
