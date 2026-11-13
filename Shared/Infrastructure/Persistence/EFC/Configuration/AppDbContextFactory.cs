using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NovaScale.Platform.API.Shared.Application.Security;

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true).AddUserSecrets<AppDbContextFactory>(optional: true).AddEnvironmentVariables().Build();
        var connection = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=127.0.0.1;Database=novaleads;User=configure_user;Password=configure_password";
        if (string.IsNullOrWhiteSpace(connection)) connection = "Server=127.0.0.1;Database=novaleads;User=configure_user;Password=configure_password";
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseMySQL(connection).Options, new DesignTimeRequestContext());
    }

    private sealed class DesignTimeRequestContext : IRequestContext
    {
        public long UserId => 0;
        public long OrganizationId => 0;
    }
}
