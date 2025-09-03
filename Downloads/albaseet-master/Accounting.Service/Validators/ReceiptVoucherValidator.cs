using FluentValidation;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Service.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Service.Services.Journal;
using Shared.CoreOne.Models.Domain.Journal;
using Accounting.CoreOne.Models.Domain;
using Accounting.Service.Services;

namespace Accounting.Service.Validators
{
    internal class ReceiptVoucherValidator : AbstractValidator<ReceiptVoucherHeader>
    {
        public ReceiptVoucherValidator(IStringLocalizer<ReceiptVoucherHeaderService> localizer)
        {
            RuleFor(x => x.ReceiptVoucherHeaderId)
                .NotEmpty()
                .NotNull()
                .NotEqual(0);

            RuleFor(x => x.ReceiptVoucherCode)
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
