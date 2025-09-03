using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Journal;

namespace Accounting.CoreOne.Models.Domain
{
    public class PaymentVoucherDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int PaymentVoucherDetailId { get; set; }

        [Column(Order = 2)]
        public int PaymentVoucherHeaderId { get; set; }

        [Column(Order = 3)]
        public int? PaymentMethodId { get; set; }

		[Column(Order = 4)]
        public int AccountId { get; set; }
        
        [Column(Order = 5)]
        public short CurrencyId { get; set; }
        
        [Column(Order = 6, TypeName = "decimal(30,15)")]
        public decimal CurrencyRate { get; set; }
        
        [Column(Order = 7, TypeName = "decimal(30,15)")]
        public decimal DebitValue { get; set; }
        
        [Column(Order = 8, TypeName = "decimal(30,15)")]
        public decimal CreditValue { get; set; }

        [Column(Order = 9, TypeName = "decimal(30,15)")]
        public decimal DebitValueAccount { get; set; }

        [Column(Order = 10, TypeName = "decimal(30,15)")]
        public decimal CreditValueAccount { get; set; }

		[Column(Order = 11)]
        [StringLength(500)]
		public string? RemarksAr { get; set; }

		[Column(Order = 12)]
        [StringLength(500)]
		public string? RemarksEn { get; set; }



		[ForeignKey(nameof(PaymentVoucherHeaderId))]
        public PaymentVoucherHeader? PaymentVoucherHeader { get; set; }

        [ForeignKey(nameof(PaymentMethodId))]
        public PaymentMethod? PaymentMethod { get; set; }

		[ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Currency? Currency { get; set; }
    }
}
