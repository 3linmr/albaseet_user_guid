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
	public class SellerCommissionMethodValidator : AbstractValidator<SellerCommissionMethod>
	{

		public SellerCommissionMethodValidator(IStringLocalizer<SellerCommissionMethodService> localizer)
		{
			RuleFor(x => x.SellerCommissionMethodId)
				.NotEmpty()
				.NotNull();
			
			RuleFor(x => x.SellerCommissionTypeId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.SellerCommissionMethodNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.SellerCommissionMethodNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}