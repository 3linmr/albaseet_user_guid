using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.Service.Services.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Modules;

namespace Shared.Service.Validators
{
	public class PaymentMethodValidator : AbstractValidator<PaymentMethod>
	{
		public PaymentMethodValidator(IStringLocalizer<PaymentMethodService> localizer)
		{
			RuleFor(x => x.PaymentMethodId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.PaymentTypeId)
				.NotEmpty()
				.NotNull();


			RuleFor(x => x.PaymentAccountId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.PaymentMethodNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
			RuleFor(x => x.PaymentMethodNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
