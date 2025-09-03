using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.Service.Services.FixedAssets;

namespace Shared.Service.Validators
{
    internal class FixedAssetValidator : AbstractValidator<FixedAsset>
    {

        public FixedAssetValidator(IStringLocalizer<FixedAssetService> localizer)
        {
            RuleFor(x => x.FixedAssetId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.AccountId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.FixedAssetNameAr)
                .NotEmpty()
                .NotNull()
                .Length(1, 250);

            RuleFor(x => x.FixedAssetNameEn)
                .NotEmpty()
                .NotNull()
                .Length(1, 250);
        }
    }
}
