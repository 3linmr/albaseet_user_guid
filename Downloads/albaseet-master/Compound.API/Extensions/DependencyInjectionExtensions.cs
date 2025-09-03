using Accounting.Service.Helpers.Data;
using Inventory.Service.Helpers.Data;
using Purchases.Service.Helpers.Data;
using Sales.Service.Helpers.Data;
using InjectServices = Shared.Service.Helpers.Data.InjectServices;

namespace Compound.API.Extensions
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
		{
			InjectServices.InjectSharedServices(services, configuration);
			services.InjectAccountingServices(configuration);
			services.InjectInventoryServices(configuration);
			services.InjectPurchasesServices(configuration);
			Service.Helpers.Data.InjectServices.InjectCompoundServices(services,configuration);
			services.InjectSalesServices(configuration);
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			return services;
		}
	}
}
