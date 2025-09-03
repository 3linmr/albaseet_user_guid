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
	public class SupplierValidator : AbstractValidator<Supplier>
	{
		public SupplierValidator(IStringLocalizer<SupplierService> localizer)
		{
			RuleFor(x => x.SupplierId)
				.NotEmpty()
				.NotNull();

			RuleFor(x => x.SupplierNameAr)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.SupplierNameEn)
				.NotEmpty()
				.NotNull()
				.Length(3, 100);

			RuleFor(x => x.ContractDate)
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
