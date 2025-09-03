using FluentValidation;
using Microsoft.Extensions.Localization;
using Inventory.CoreOne.Models.Domain;
using Inventory.Service.Services;

namespace Inventory.Service.Validators
{
    public class StockTakingHeaderValidator : AbstractValidator<StockTakingHeader>
    {
        public StockTakingHeaderValidator(IStringLocalizer<StockTakingHeaderService> localizer)
        {
            RuleFor(x => x.StoreId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.StockDate)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.StockTakingCode)
	            .NotEmpty()
	            .NotNull()
                .GreaterThan(0);

			RuleFor(x => x.StockTakingNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.StockTakingNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
