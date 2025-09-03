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
    public class DistrictValidator : AbstractValidator<District>
    {
        public DistrictValidator(IStringLocalizer<DistrictService> localizer)
        {
            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CityId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.DistrictNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.DistrictNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
