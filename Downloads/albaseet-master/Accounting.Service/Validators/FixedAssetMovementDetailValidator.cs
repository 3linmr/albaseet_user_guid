using Accounting.CoreOne.Models.Domain;
using Accounting.Service.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Service.Validators
{
    internal class FixedAssetMovementDetailValidator : AbstractValidator<FixedAssetMovementDetail>
    {
        public FixedAssetMovementDetailValidator(IStringLocalizer<FixedAssetMovementDetailService> localizer)
        {
            RuleFor(x => x.FixedAssetMovementDetailId)
                .NotEmpty()
                .NotNull()
                .NotEqual(0);

            RuleFor(x => x.FixedAssetMovementHeaderId)
                .NotEmpty()
                .NotNull()
                .NotEqual(0);

            RuleFor(x => x.FixedAssetId)
                .NotEmpty()
                .NotNull()
                .NotEqual(0);
        }
    }
}
