using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class PaymentMethodDto
	{
		public int PaymentMethodId { get; set; }
		public int PaymentMethodCode { get; set; }

		public byte PaymentTypeId { get; set; }
		public string? PaymentTypeName { get; set; }

		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }

		public string? PaymentMethodName { get; set; }
		public string? PaymentMethodNameAr { get; set; }
		public string? PaymentMethodNameEn { get; set; }

		public int PaymentAccountId { get; set; }
		public string? PaymentAccountCode { get; set; }
		public string? PaymentAccountName { get; set; }
        public short CurrencyId { get; set; }
        public decimal CurrencyRate { get; set; }
        public string? CurrencyName { get; set; }

        public decimal FixedCommissionValue { get; set; }
		public decimal CommissionPercent { get; set; }
		public decimal MinCommissionValue { get; set; }
		public decimal MaxCommissionValue { get; set; }

		public int? CommissionAccountId { get; set; }
		public string? CommissionAccountCode { get; set; }
		public string? CommissionAccountName { get; set; }

		public int? TaxId { get; set; }
		public string? TaxName { get; set; }

		public int? CommissionTaxAccountId { get; set; }
		public string? CommissionTaxAccountCode { get; set; }
		public string? CommissionTaxAccountName { get; set; }

		public bool IsActive { get; set; }
		public string? InActiveReasons { get; set; }

		public bool IsPaymentMethod { get; set; }
		public bool IsReceivingMethod { get; set; }


		public bool FixedCommissionHasVat { get; set; }
		public bool FixedCommissionVatInclusive { get; set; }

		public bool CommissionHasVat { get; set; }
		public bool CommissionVatInclusive { get; set; }

	}
	public class PaymentMethodDropDownDto
	{
		public int PaymentMethodId { get; set; }
		public string? PaymentMethodName { get; set; }
	}

	public class VoucherPaymentMethodDto
	{
		public int PaymentMethodId { get; set; }
		public string? PaymentMethodName { get; set; }
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
		public int PaymentTypeId { get; set; }
		public int? CommissionAccountId { get; set; }
		public int? CommissionTaxAccountId { get; set; }
		public int? CommissionTaxId { get; set; }

		public short CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
		public decimal CurrencyRate { get; set; }
		public decimal FixedCommissionValue { get; set; }
		public decimal CommissionPercent { get; set; }
		public decimal MinCommissionValue { get; set; }
		public decimal MaxCommissionValue { get; set; }
		public bool IsPaymentMethod { get; set; }
		public bool IsReceivingMethod { get; set; }
	}
	public class PaymentMethodEntryDto
	{
		public int CommissionAccountId { get; set; }
		public string? CommissionAccountNameAr { get; set; }
		public string? CommissionAccountNameEn { get; set; }
		public int? CommissionTaxId { get; set; }
		public byte? CommissionTaxTypeId { get; set; }
		public int CommissionTaxAccountId { get; set; }
		public string? CommissionTaxAccountNameAr { get; set; }
		public string? CommissionTaxAccountNameEn { get; set; }
		public short CommissionCurrencyId { get; set; }
		public short CommissionTaxCurrencyId { get; set; }
		public decimal CommissionCurrencyRate { get; set; }
		public decimal CommissionTaxCurrencyRate { get; set; }
		public decimal FixedCommissionValue { get; set; }
		public decimal FixedCommissionTaxValue { get; set; }
		public decimal CommissionValue { get; set; }
		public decimal CommissionTaxValue { get; set; }
		public decimal BankValue { get; set; }
	}
}
