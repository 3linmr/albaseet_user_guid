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
    public class ItemAttributeTypeValidator : AbstractValidator<ItemAttributeType>
	{

		public ItemAttributeTypeValidator(IStringLocalizer<ItemAttributeTypeService> localizer)
		{
			RuleFor(x => x.ItemAttributeTypeId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemAttributeTypeNameAr)
				.NotEmpty()
				.NotNull()
				.Length(1, 100);

			RuleFor(x => x.ItemAttributeTypeNameEn)
				.NotEmpty()
				.NotNull()
				.Length(1, 100);
		}
	}
}
