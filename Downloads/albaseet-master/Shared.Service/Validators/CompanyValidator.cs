using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Basics;
using Shared.Service.Services.Modules;

namespace Shared.Service.Validators
{
    public class CompanyValidator : AbstractValidator<Company>
    {

        public CompanyValidator(IStringLocalizer<CompanyService> localizer)
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CurrencyId)
	            .NotEmpty()
	            .NotNull();

			RuleFor(x => x.CompanyNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 200);

            RuleFor(x => x.CompanyNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 1000);

            //RuleFor(x => x.Email)
	           // .EmailAddress();

		}
    }
}
