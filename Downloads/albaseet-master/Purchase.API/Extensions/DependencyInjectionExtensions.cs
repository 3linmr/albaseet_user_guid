using Purchases.Service.Helpers.Data;
using InjectServices = Shared.Service.Helpers.Data.InjectServices;

namespace Purchases.API.Extensions
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
		{
			InjectServices.InjectSharedServices(services, configuration);
			services.InjectPurchasesServices(configuration);
			services.InjectPurchasesFluentValidations();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			return services;
		}
	}
}
