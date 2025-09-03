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
using Shared.Service.Services.Modules;

namespace Shared.Service.Validators
{
	public class StoreValidator : AbstractValidator<Store>
	{
		public StoreValidator(IStringLocalizer<StoreService> localizer)
		{
			RuleFor(x => x.StoreId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.StoreNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 1000);

			RuleFor(x => x.StoreNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 1000);

			RuleFor(x => x.StoreClassificationId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.BranchId)
				.NotEmpty()
				.NotNull();

			//RuleFor(x => x.CountryId)
			//	.NotEmpty()
			//	.NotNull();


			//RuleFor(x => x.StateId)
			//	.NotEmpty()
			//	.NotNull();


			//RuleFor(x => x.CityId)
			//	.NotEmpty()
			//	.NotNull();

			//RuleFor(x => x.DistrictId)
			//	.NotEmpty()
			//	.NotNull();

			//RuleFor(x => x.DistrictId)
			//	.NotEmpty()
			//	.NotNull();	
			
			//RuleFor(x => x.BuildingNumber)
			//	.NotEmpty()
			//	.NotNull();

			//RuleFor(x => x.Street1)
			//	.NotEmpty()
			//	.NotNull();


			//RuleFor(x => x.PostalCode)
			//	.NotEmpty()
			//	.NotNull();

			//RuleFor(x => x.CommercialRegister)
			//	.NotEmpty()
			//	.NotNull();
		}
	}
}
