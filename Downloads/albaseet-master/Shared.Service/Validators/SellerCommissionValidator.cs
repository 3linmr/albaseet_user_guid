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
	public class SellerCommissionValidator : AbstractValidator<SellerCommission>
	{

		public SellerCommissionValidator(IStringLocalizer<SellerCommissionService> localizer)
		{
			RuleFor(x => x.SellerCommissionId)
				.NotEmpty()
				.NotNull();
			RuleFor(x => x.SellerCommissionMethodId)
				.NotEmpty()
				.NotNull();

			//RuleFor(x => x.From)
			//	.NotEmpty()
			//	.NotNull()
			//	.GreaterThanOrEqualTo(0);

			RuleFor(x => x.To)
				.NotEmpty()
				.NotNull();
			RuleFor(x => x.CommissionPercent)
				.NotEmpty()
				.NotNull()
				.InclusiveBetween(0m,100.00m);
		}
	}
}