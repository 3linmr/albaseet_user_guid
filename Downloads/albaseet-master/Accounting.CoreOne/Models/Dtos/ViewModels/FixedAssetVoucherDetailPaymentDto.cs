using Accounting.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
    public class FixedAssetVoucherDetailPaymentDto
    {
        public int FixedAssetVoucherDetailPaymentId { get; set; }
        public int FixedAssetVoucherDetailId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int AccountId { get; set; }
        public short CurrencyId { get; set; }
        public int? FixedAssetVoucherHeaderId { get; set; }
        public int? FixedAssetId { get; set; }
        //public decimal CurrencyRate { get; set; }
        public decimal DebitValue { get; set; }
        public decimal CreditValue { get; set; }
        //public decimal DebitValueAccount { get; set; }
        //public decimal CreditValueAccount { get; set; }
        public string? PaymentMethodName { get; set; }
        //public string? PaymentMethodNameAr { get; set; }
        //public string? PaymentMethodNameEn { get; set; }
        //public string? AccountName { get; set; }
        //public string? AccountNameAr { get; set; }
        //public string? AccountNameEn { get; set; }
        //public string? CurrencyName { get; set; }
        //public string? CurrencyNameAr { get; set; }
        //public string? CurrencyNameEn { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? IpAddressCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public string? UserNameModified { get; set; }
    }
}
