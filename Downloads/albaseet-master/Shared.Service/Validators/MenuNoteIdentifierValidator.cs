using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.Service.Services.Basics;
using Shared.Service.Services.Menus;

namespace Shared.Service.Validators
{
    public class MenuNoteIdentifierValidator : AbstractValidator<MenuNoteIdentifier>
    {

        public MenuNoteIdentifierValidator(IStringLocalizer<MenuNoteIdentifierService> localizer)
        {
            RuleFor(x => x.MenuNoteIdentifierId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.MenuNoteIdentifierNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.MenuNoteIdentifierNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
