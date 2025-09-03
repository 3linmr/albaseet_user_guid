using FluentValidation;
using Sales.Service.Services;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Service.Validators
{
    public class ClientQuotationApprovalHeaderValidator: AbstractValidator<ClientQuotationApprovalHeader>
    {
        public ClientQuotationApprovalHeaderValidator(IStringLocalizer<ClientQuotationApprovalHeaderService> localizer)
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
