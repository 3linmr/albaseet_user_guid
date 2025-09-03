using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Taxes
{
	public class TaxDto
	{
		public int TaxId { get; set; }
		public int TaxCode { get; set; }
		public string? TaxNameAr { get; set; }
		public string? TaxNameEn { get; set; }
		public string? TaxName { get; set; }
		public int CompanyId { get; set; }
		public int StoreId { get; set; }
		public string? CompanyName { get; set; }
		public string? StoreName { get; set; }
		public byte TaxTypeId { get; set; }
		public string? TaxTypeName { get; set; }
		public int? DrAccount { get; set; }
		public string? DrAccountCode { get; set; }
		public string? DrAccountName { get; set; }
		public int? CrAccount { get; set; }
		public string? CrAccountCode { get; set; }
		public string? CrAccountName { get; set; }

		public bool IsVatTax { get; set; }

		public bool TaxAfterVatInclusive { get; set; } //احتساب الضريبة علي المبلغ شامل الضرائب (Value + VAT)

		public decimal Percent { get; set; }

	}

	public class TaxDropDownDto
	{
		public int TaxId { get; set; }
		public string? TaxName { get; set; }
		public bool IsVatTax { get; set; }
	}

	public class TaxVm
	{
		public TaxDto? Taxes { get; set; }
		public List<TaxPercentDto>? TaxPercents { get; set; }
	}
}
