using FluentValidation;
using Microsoft.Extensions.Localization;
using Inventory.CoreOne.Models.Domain;
using Inventory.Service.Services;

namespace Inventory.Service.Validators
{
    public class StockTakingCarryOverHeaderValidator : AbstractValidator<StockTakingCarryOverHeader>
    {
        public StockTakingCarryOverHeaderValidator(IStringLocalizer<StockTakingCarryOverHeaderService> localizer)
        {
            RuleFor(x => x.StoreId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.DocumentDate)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.StockTakingCarryOverCode)
	            .NotEmpty()
	            .NotNull()
                .GreaterThan(0);

			RuleFor(x => x.StockTakingCarryOverNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.StockTakingCarryOverNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
