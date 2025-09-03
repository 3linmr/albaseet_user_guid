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
	public class SellerValidator : AbstractValidator<Seller>
	{

		public SellerValidator(IStringLocalizer<SellerService> localizer)
		{
			RuleFor(x => x.SellerId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.SellerNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.SellerNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.SellerTypeId)
				.NotEmpty()
				.NotNull();
			
			RuleFor(x => x.ContractDate)
				.NotEmpty()
				.NotNull();
		}
	}
}