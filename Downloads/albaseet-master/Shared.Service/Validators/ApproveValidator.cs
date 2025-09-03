using FluentValidation;
using Shared.CoreOne.Models.Domain.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Microsoft.Extensions.Localization;
using Shared.Service.Services.Approval;
using Shared.Service.Services.Basics;

namespace Shared.Service.Validators
{
	internal class ApproveValidator : AbstractValidator<Approve>
	{
		public ApproveValidator(IStringLocalizer<ApproveService> localizer)
		{
			RuleFor(x => x.ApproveId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.MenuCode)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ApproveNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.ApproveNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
