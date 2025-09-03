using FluentValidation;
using Inventory.CoreOne.Models.Domain;
using Inventory.Service.Services;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Validators
{
    public class InternalTransferHeaderValidator: AbstractValidator<InternalTransferHeader>
    {
        public InternalTransferHeaderValidator(IStringLocalizer<InternalTransferHeaderService> localizer) 
        {
            RuleFor(x => x.FromStoreId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.ToStoreId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.DocumentDate)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.InternalTransferCode)
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
