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
    public class StateValidator : AbstractValidator<State>
    {
        public StateValidator(IStringLocalizer<StateService> localizer)
        {
            RuleFor(x => x.StateId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.CountryId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.StateNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.StateNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
