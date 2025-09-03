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
    public class InvoiceExpenseTypeValidator : AbstractValidator<InvoiceExpenseType>
    {

        public InvoiceExpenseTypeValidator(IStringLocalizer<InvoiceExpenseTypeService> localizer)
        {
            RuleFor(x => x.InvoiceExpenseTypeId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.InvoiceExpenseTypeNameAr)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);

            RuleFor(x => x.InvoiceExpenseTypeNameEn)
                .NotEmpty()
                .NotNull()
                .Length(3, 100);
        }
    }
}
