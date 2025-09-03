using Inventory.Service.Helpers.Data;
using Shared.Service.Helpers.Data;
using InjectServices = Shared.Service.Helpers.Data.InjectServices;

namespace Inventory.API.Extensions
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
		{
			services.InjectSharedServices(configuration);
			services.InjectInventoryServices(configuration);
			services.InjectInventoryFluentValidations();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			return services;
		}
	}
}
