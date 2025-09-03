using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;

namespace Compound.CoreOne.Models.Dtos.Print
{
	internal class PrintInvoiceDto
	{
	}

	public class SimplifiedInvoiceDto
	{
		public SimplifiedInvoiceHeaderDto SimplifiedInvoiceHeader { get; set; } = new();
		public List<SimplifiedInvoiceDetailDto> SimplifiedInvoiceDetail { get; set; } = [];
		public InvoiceSettingDto InvoiceSetting { get; set; } = new();
		public bool IsMemo { get; set; } // Indicates if this is a memo invoice
	}

	public class TaxInvoiceDto
	{
		public TaxInvoiceHeaderDto TaxInvoiceHeader { get; set; } = new();
		public TaxInvoiceEntityDto Seller { get; set; } = new();
		public TaxInvoiceEntityDto Buyer { get; set; } = new();
		public List<TaxInvoiceDetailDto> TaxInvoiceDetail { get; set; } = [];
		public InvoiceSettingDto InvoiceSetting { get; set; } = new();
		public bool IsMemo { get; set; } // Indicates if this is a memo invoice
	}

	public class SimplifiedInvoiceHeaderDto
	{
		public string? InvoiceType { get; set; } // Invoice type
		public string? InvoiceNumber { get; set; }
		public string? PaymentType { get; set; }
		public DateTime InvoiceDate { get; set; }
		public string? VatNumber { get; set; }
		public decimal TotalValue { get; set; }
		public decimal VatValue { get; set; }
		public decimal VatPercent { get; set; }
		public decimal DiscountValue { get; set; }
		public decimal NetValue { get; set; }
	}

	public class SimplifiedInvoiceDetailDto
	{
		public string? ItemName { get; set; }
		public string? Package { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal VatValue { get; set; }
		public decimal DiscountValue { get; set; }
		public decimal NetValue { get; set; }
	}

	public class TaxInvoiceHeaderDto
	{
		public string? SerialNumber { get; set; }
		public DateTime InvoiceDate { get; set; } // ISO or formatted
		public string? InvoiceType { get; set; } // Invoice type
		public DateTime? DueDate { get; set; } // Due date for the invoice (optional)
		public decimal TotalValue { get; set; }
		public decimal DiscountValue { get; set; } // Discount value for the invoice (before VAT)
		public decimal VatPercent { get; set; } // e.g. 15 for 15%
		public decimal VatValue { get; set; }
		public decimal NetValue { get; set; }
	}

	public class TaxInvoiceDetailDto
	{
		public string? ItemName { get; set; }
		public string? Package { get; set; } // Package name/label
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public decimal TotalValue { get; set; }
		public decimal? DiscountValue { get; set; } // Discount value for the item
		public decimal VatPercent { get; set; } // e.g. 15 for 15%
		public decimal VatValue { get; set; } // Calculated tax amount for the item
		public decimal? NetValue { get; set; } // Total including VAT for the item
	}

	public class TaxInvoiceEntityDto
	{
		public string? EntityName { get; set; }
		public string? EntityAddress { get; set; }
		public string? EntityContact { get; set; }
		public string? CommercialRegistration { get; set; }
		public string? VatRegistration { get; set; }
	}
}
