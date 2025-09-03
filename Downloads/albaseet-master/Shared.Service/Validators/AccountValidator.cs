using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.Service.Services.Accounts;

namespace Shared.Service.Validators
{
	public class AccountValidator : AbstractValidator<Account>
	{

		public AccountValidator(IStringLocalizer<AccountService> localizer)
		{
			RuleFor(x => x.AccountId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.AccountCode)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CurrencyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.AccountNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 500);

			RuleFor(x => x.AccountNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 500);


		}
	}
}
