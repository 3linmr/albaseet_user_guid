using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.Service.Services.Basics;

namespace Shared.Service.Validators
{
    public class CountryValidator : AbstractValidator<Country>
    {

        public CountryValidator(IStringLocalizer<CountryService> localizer)
        {
            RuleFor(x => x.CountryId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CountryNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.CountryNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
