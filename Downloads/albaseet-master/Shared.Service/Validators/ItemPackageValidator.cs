using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Service.Services.Items;
using Shared.CoreOne.Models.Domain.Items;

namespace Shared.Service.Validators
{
    public class ItemPackageValidator : AbstractValidator<ItemPackage>
	{

		public ItemPackageValidator(IStringLocalizer<ItemPackageService> localizer)
		{
			RuleFor(x => x.ItemPackageId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.PackageNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.PackageNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}