using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Modules;

namespace Shared.Service.Validators
{
	public class CurrencyValidator : AbstractValidator<Currency>
	{
		public CurrencyValidator(IStringLocalizer<CurrencyService> localizer)
		{
			RuleFor(x => x.CurrencyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CurrencyNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);


			RuleFor(x => x.CurrencyNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
