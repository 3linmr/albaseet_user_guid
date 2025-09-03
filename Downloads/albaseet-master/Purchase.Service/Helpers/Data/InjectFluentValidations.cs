using Microsoft.Extensions.DependencyInjection;

namespace Purchases.Service.Helpers.Data
{
    public static class InjectFluentValidations
    {
        public static IServiceCollection InjectPurchasesFluentValidations(this IServiceCollection services)
        {
            return services;
        }
    }
}
