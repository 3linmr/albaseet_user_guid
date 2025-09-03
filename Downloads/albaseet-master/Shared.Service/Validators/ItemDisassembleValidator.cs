using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.Service.Services.Inventory;

namespace Shared.Service.Validators
{
	public class ItemDisassembleValidator : AbstractValidator<ItemDisassemble>
	{
		public ItemDisassembleValidator(IStringLocalizer<ItemDisassembleService> localizer)
		{
			RuleFor(x => x.ItemDisassembleId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.StoreId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.ItemPackageId)
				.NotEmpty()
				.NotNull();
		}
	}
}
