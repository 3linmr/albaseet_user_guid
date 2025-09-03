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
    internal class ItemSubSectionValidator : AbstractValidator<ItemSubSection>
	{
		public ItemSubSectionValidator(IStringLocalizer<ItemSubSectionService> localizer)
		{

			RuleFor(x => x.ItemSubSectionId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemSectionId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.SubSectionNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.SubSectionNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}
