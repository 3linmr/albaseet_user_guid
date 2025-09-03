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
    internal class MainItemValidator : AbstractValidator<MainItem>
	{
		public MainItemValidator(IStringLocalizer<MainItemService> localizer)
		{
			RuleFor(x => x.MainItemId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemSubSectionId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.MainItemNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.MainItemNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}