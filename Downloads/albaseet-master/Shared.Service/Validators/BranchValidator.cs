using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.Service.Services.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Modules;

namespace Shared.Service.Validators
{
	internal class BranchValidator : AbstractValidator<Branch>
	{
		public BranchValidator(IStringLocalizer<BranchService> localizer)
		{
			RuleFor(x => x.BranchId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.BranchNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 1000);

			//RuleFor(x => x.ResponsibleNameEn)
			//	.NotEmpty()
			//	.NotNull()
			//	.Length(3, 100);

			//RuleFor(x => x.BranchEmail)
			//	.EmailAddress();

			//RuleFor(x => x.ResponsibleEmail)
			//	.EmailAddress();
		}
	}
}
