using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Validators
{
    public class ShippingStatusValidator: AbstractValidator<ShippingStatus>
    {
        public ShippingStatusValidator(IStringLocalizer<ShippingStatusService> localizer)
        {
            RuleFor(x => x.ShippingStatusId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.ShippingStatusNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.ShippingStatusNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
