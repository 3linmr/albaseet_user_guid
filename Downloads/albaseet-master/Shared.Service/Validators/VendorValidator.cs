using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Items;

namespace Shared.Service.Validators
{
    public class VendorValidator : AbstractValidator<Vendor>
	{

		public VendorValidator(IStringLocalizer<VendorService> localizer)
		{
			RuleFor(x => x.VendorId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.VendorNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.VendorNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);
		}
	}
}