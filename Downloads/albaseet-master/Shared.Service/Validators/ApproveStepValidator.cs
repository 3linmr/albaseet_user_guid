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
	internal class ApproveStepValidator : AbstractValidator<ApproveStep>
	{
		public ApproveStepValidator(IStringLocalizer<ApproveStepService> localizer)
		{
			RuleFor(x => x.ApproveStepId)
				.NotEmpty()
				.NotNull();
			RuleFor(x => x.ApproveId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.StepNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.StepNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
