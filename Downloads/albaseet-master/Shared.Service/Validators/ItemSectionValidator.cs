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
    internal class ItemSectionValidator : AbstractValidator<ItemSection>
	{
		public ItemSectionValidator(IStringLocalizer<ItemSectionService> localizer)
		{
			RuleFor(x => x.ItemSectionId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemSubCategoryId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.SectionNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.SectionNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
