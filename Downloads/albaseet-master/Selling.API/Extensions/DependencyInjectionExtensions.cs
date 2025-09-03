using Sales.Service.Helpers.Data;
using InjectServices = Shared.Service.Helpers.Data.InjectServices;

namespace Sales.API.Extensions
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
		{
			InjectServices.InjectSharedServices(services, configuration);
			services.InjectSalesServices(configuration);
			services.InjectSalesFluentValidations();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			return services;
		}
	}
}
