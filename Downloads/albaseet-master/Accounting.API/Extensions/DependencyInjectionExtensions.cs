using Accounting.Service.Helpers.Data;
using InjectServices = Shared.Service.Helpers.Data.InjectServices;

namespace Accounting.API.Extensions
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
		{
			InjectServices.InjectSharedServices(services, configuration);
			services.InjectAccountingServices(configuration);
			services.InjectAccountingFluentValidations();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			return services;
		}
	}
}
