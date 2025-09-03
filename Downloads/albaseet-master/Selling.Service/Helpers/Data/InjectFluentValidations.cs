using Microsoft.Extensions.DependencyInjection;

namespace Sales.Service.Helpers.Data
{
    public static class InjectFluentValidations
    {
        public static IServiceCollection InjectSalesFluentValidations(this IServiceCollection services)
        {
            return services;
        }
    }
}
