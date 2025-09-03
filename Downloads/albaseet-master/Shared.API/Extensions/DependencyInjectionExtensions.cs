using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Shared.Repository;
using System.Reflection;
using Shared.Service.Helpers.Data;

namespace Shared.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.InjectSharedServices(configuration);
            services.InjectMyFluentValidations();
            //services.Configure<MailSettingDto>(configuration.GetSection("MailSettings"));
            //services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
