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
    internal class FixedAssetVoucherValidator : AbstractValidator<FixedAssetVoucherHeader>
    {
        public FixedAssetVoucherValidator(IStringLocalizer<FixedAssetVoucherHeaderService> localizer)
        {
            RuleFor(x => x.FixedAssetVoucherHeaderId)
                .NotEmpty()
                .NotNull()
                .NotEqual(0);

            RuleFor(x => x.DocumentCode)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.DocumentDate)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.RemarksAr)
                .Length(0, 500);

            RuleFor(x => x.RemarksEn)
                .Length(0, 500);
        }
    }
}

