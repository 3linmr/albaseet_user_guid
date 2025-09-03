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
	public class BankValidator : AbstractValidator<Bank>
	{

		public BankValidator(IStringLocalizer<BankService> localizer)
		{
			RuleFor(x => x.BankId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.BankNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.BankNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			//RuleFor(x => x.VisaFees)
			//	.GreaterThan(0)
			//	.LessThan(100);
		}
	}
}