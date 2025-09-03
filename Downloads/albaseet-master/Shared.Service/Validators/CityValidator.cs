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
    public class CityValidator : AbstractValidator<City>
    {
        public CityValidator(IStringLocalizer<CityService> localizer)
        {
            RuleFor(x => x.CityId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.StateId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CityNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.CityNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
