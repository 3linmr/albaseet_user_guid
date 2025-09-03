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
	public class CurrencyRateValidator : AbstractValidator<CurrencyRate>
	{
		public CurrencyRateValidator(IStringLocalizer<CurrencyRateService> localizer)
		{
			RuleFor(x => x.CurrencyRateId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.FromCurrencyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ToCurrencyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CurrencyRateValue)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.FromCurrencyId).NotEqual(x=>x.ToCurrencyId);
		}
	}
}
