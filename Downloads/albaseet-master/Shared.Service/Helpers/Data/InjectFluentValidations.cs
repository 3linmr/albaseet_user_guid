using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Validators;

namespace Shared.Service.Helpers.Data
{
    public static class InjectFluentValidations
    {
        public static IServiceCollection InjectMyFluentValidations(this IServiceCollection services)
        {
            services.AddScoped<IValidator<Country>, CountryValidator>();
            services.AddScoped<IValidator<State>, StateValidator>();
            services.AddScoped<IValidator<City>, CityValidator>();


            ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();
            return services;
        }
    }
}
