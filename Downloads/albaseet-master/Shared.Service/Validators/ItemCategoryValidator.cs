using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Service.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Validators
{
    public class ItemCategoryValidator : AbstractValidator<ItemCategory>
	{

		public ItemCategoryValidator(IStringLocalizer<ItemCategoryService> localizer)
		{
			RuleFor(x => x.ItemCategoryId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.CategoryNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.CategoryNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
