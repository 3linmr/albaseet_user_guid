using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Shared.CoreOne.Models.Domain.Modules
{
	public class PaymentMethod : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int PaymentMethodId { get; set; }

		[Column(Order = 2)]
		public int PaymentMethodCode { get; set; }

		[Column(Order = 3)]
		public byte PaymentTypeId { get; set; }

		[Column(Order = 4)]
		public int CompanyId { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 5)]
		public string? PaymentMethodNameAr { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 6)]
		public string? PaymentMethodNameEn { get; set; }

		[Column(Order = 7)]
		public int PaymentAccountId { get; set; }

		[Column(Order = 8, TypeName = "decimal(30,15)")]
		public decimal FixedCommissionValue { get; set; }

		[Column(Order = 9, TypeName = "decimal(30,15)")]
		public decimal CommissionPercent { get; set; }

		[Column(Order = 10, TypeName = "decimal(30,15)")]
		public decimal MinCommissionValue { get; set; }

		[Column(Order =11, TypeName = "decimal(30,15)")]
		public decimal MaxCommissionValue { get; set; }

		[Column(Order = 12)]
		public int? CommissionAccountId { get; set; }

		[Column(Order = 13)]
		public int? TaxId { get; set; }

		[Column(Order = 14)]
		public int? CommissionTaxAccountId { get; set; }

		[Column(Order = 15)]
		public bool IsActive { get; set; }

		[StringLength(500)]
		[Column(Order = 16)]
		public string? InActiveReasons { get; set; }

		[Column(Order = 17)]
		public bool IsPaymentMethod { get; set; }

		[Column(Order = 18)]
		public bool IsReceivingMethod { get; set; }

		[Column(Order = 19)]
		public bool FixedCommissionHasVat { get; set; }

		[Column(Order = 20)]
		public bool FixedCommissionVatInclusive { get; set; }

		[Column(Order = 21)]
		public bool CommissionHasVat { get; set; }

		[Column(Order = 22)]
		public bool CommissionVatInclusive { get; set; }


		[ForeignKey(nameof(PaymentTypeId))]
		public PaymentType? PaymentType { get; set; }

		[ForeignKey(nameof(CompanyId))]
		public Company? Company { get; set; }

		[ForeignKey(nameof(PaymentAccountId))]
		public Account? PaymentAccount { get; set; }

		[ForeignKey(nameof(CommissionAccountId))]
		public Account? CommissionAccount { get; set; }

		[ForeignKey(nameof(TaxId))]
		public Tax? Tax { get; set; }
		
		[ForeignKey(nameof(CommissionTaxAccountId))]
		public Account? CommissionTaxAccount { get; set; }
	}
}
