namespace NovaScale.Platform.API.SalesManagement.Application.Services;

public record DashboardSummary(DateOnly From, DateOnly To, int TotalContacts, int ActiveLeads, int ActiveOpportunities,
    int WonSales, decimal WonRevenue, decimal ConversionRate, decimal SalesGoal);
public record SalespersonSummary(long UserId, string FullName, int WonSales, decimal WonRevenue);

public interface IDashboardQueryService
{
    Task<DashboardSummary> GetAsync(DateOnly? from, DateOnly? to, long? userId, CancellationToken cancellationToken);
    Task<IEnumerable<SalespersonSummary>> GetSalespeopleAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken);
}
