using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Service.Services.Basics;
using Shared.Service.Services.Modules;

namespace Shared.Service.Validators
{
    public class ShipmentTypeValidator : AbstractValidator<ShipmentType>
    {

        public ShipmentTypeValidator(IStringLocalizer<ShipmentTypeService> localizer)
        {
            RuleFor(x => x.ShipmentTypeId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.ShipmentTypeNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.ShipmentTypeNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
