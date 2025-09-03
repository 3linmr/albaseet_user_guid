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
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.Service.Services.Accounts;
using Shared.Service.Services.CostCenters;

namespace Shared.Service.Validators
{
	public class CostCenterValidator : AbstractValidator<CostCenter>
	{

		public CostCenterValidator(IStringLocalizer<CostCenterService> localizer)
		{
			RuleFor(x => x.CostCenterId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CostCenterCode)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CompanyId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CostCenterNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 500);

			RuleFor(x => x.CostCenterNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 500);
		}
	}
}
