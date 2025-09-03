using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.Service.Services.Taxes;

namespace Shared.Service.Validators
{
	public class TaxValidator : AbstractValidator<Tax>
	{
		public TaxValidator(IStringLocalizer<TaxService> localizer)
		{
			RuleFor(x => x.TaxId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.TaxTypeId)
				.NotEmpty()
				.NotNull();
		}
	}
}
