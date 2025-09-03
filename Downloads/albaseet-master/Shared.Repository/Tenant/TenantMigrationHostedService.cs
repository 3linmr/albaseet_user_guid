using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using Shared.Helper.Services.Tenant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;

namespace Shared.Repository.Tenant
{
	public class TenantMigrationHostedService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<TenantMigrationHostedService> logger, IHttpClientFactory httpClientFactory) : IHostedService
	{
		private readonly IConfiguration _configuration = configuration;
		private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// Get all tenant Ids from identity server  
			var tenantIds = await GetTenantIdsAsync(cancellationToken);

			logger.LogInformation("Starting tenant migration...");

			if (tenantIds.Any())
			{
				foreach (var tenantId in tenantIds)
				{
					try
					{
						using var scope = serviceProvider.CreateScope();
						// Set the current tenant for this scope.
						var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
						tenantProvider.CurrentTenant = new TenantDto() { CompanyId = tenantId };

						// Get the ApplicationDbContext configured for the current tenant.
						var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

						logger.LogInformation("Migrating database for tenant: {DatabaseName}", tenantProvider.CurrentTenant.DatabaseName);

						// Run the migration inside a transaction. EF Core automatically wraps migrations
						// in a transaction if the provider supports it.
						await dbContext.Database.MigrateAsync(cancellationToken);

						logger.LogInformation("Migration applied successfully for tenant: {DatabaseName}", tenantProvider.CurrentTenant.DatabaseName);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "Error migrating tenant with ID {TenantId}. Attempting rollback.", tenantId);

						// Here you could implement additional logic to rollback manually if needed.
						// For example, you might execute a custom command to revert to a previous migration.
						// In many cases, the automatic transaction rollback (provided by MigrateAsync) is sufficient.
					}
				}

				logger.LogInformation("Tenant migration completed.");
			}
			else
			{
				logger.LogInformation("No tenants found...");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		/// <summary>
		/// Retrieves tenant IDs from an external API.
		/// </summary>
		private async Task<List<int>> GetTenantIdsAsync(CancellationToken cancellationToken)
		{
			var adminApiUrl = _configuration.GetValue<string>("Application:AdminAPI");
			if (string.IsNullOrEmpty(adminApiUrl))
			{
				logger.LogWarning("Admin API endpoint is not configured.");
				return [];
			}

			var link = $"{adminApiUrl}/api/AdminPublicAPI/GetAllCompanyIdList";
			try
			{
				var client = _httpClientFactory.CreateClient();
				var response = await client.GetAsync(link, cancellationToken);
				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync(cancellationToken);
					var data = JsonConvert.DeserializeObject<List<int>>(json);
					logger.LogInformation("Retrieved {Count} tenant IDs from API.", data?.Count ?? 0);
					return data ?? [];
				}
				else
				{
					logger.LogError("Failed to retrieve tenant IDs. Status code: {StatusCode}", response.StatusCode);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Exception occurred while calling Admin API.");
			}

			return [];
		}
	}
}
