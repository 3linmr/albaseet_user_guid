using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;

namespace Accounting.Service.Helpers.Data
{
    public static class InjectFluentValidations
    {
        public static IServiceCollection InjectAccountingFluentValidations(this IServiceCollection services)
        {
            return services;
        }
    }
}
