using FluentValidation;
using Purchases.Service.Services;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.Service.Validators
{
    public class PurchaseInvoiceHeaderValidator: AbstractValidator<PurchaseInvoiceHeader>
    {
        public PurchaseInvoiceHeaderValidator(IStringLocalizer<PurchaseInvoiceHeaderService> localizer)
        {
            RuleFor(x => x.StoreId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.DocumentDate)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.DocumentCode)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.RemarksAr)
                .Length(0, 500);

            RuleFor(x => x.RemarksEn)
                .Length(0, 500);
        }
    }
}
