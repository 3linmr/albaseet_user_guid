using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Service.Services.Items;

namespace Shared.Service.Validators
{
	public class ItemValidator : AbstractValidator<Item>
	{
		public ItemValidator(IStringLocalizer<ItemService> localizer)
		{
			RuleFor(x => x.ItemId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 2000);

			RuleFor(x => x.ItemNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 2000);
		}
	}
}
