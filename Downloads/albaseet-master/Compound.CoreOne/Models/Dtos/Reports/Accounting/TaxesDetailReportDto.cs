using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class TaxesDetailReportDto
    {
		public int JournalHeaderId { get; set; }
		public int TicketId { get; set; }
		public int Serial { get; set; }
		public DateTime? TicketDate { get; set; }
		public DateTime? EntryDate { get; set; }
		public int? InvoiceId { get; set; }
		public string? InvoiceFullCode { get; set; }
		public string? JournalFullCode { get; set; }
		public int JournalTypeId { get; set; }
		public string? JournalTypeName { get; set; }
		public string? InvoiceTypeName { get; set; }

		public string? AccountNameAr { get; set; }
		public string? AccountNameEn { get; set; }

		public decimal? GrossValue { get; set; } // قيمة السند قبل الضرائب
		public decimal TaxValue { get; set; }
		public decimal TaxPaid { get; set; }
		public decimal TaxCollected { get; set; }

		public bool IsTax { get; set; }
		public int? TaxId { get; set; }
		public decimal TaxPercent { get; set; }
		public string? TaxName { get; set; }
		public string? TaxCode { get; set; }
		public string? TaxReference { get; set; }
		public bool TaxAfterVatInclusive { get; set; }
		public DateTime? TaxDate { get; set; }

		public string? PeerReference { get; set; }
		public string? InvoiceReference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public string? PostalCode { get; set; }
		public string? BuildingNumber { get; set; }
		public string? Street1 { get; set; }
		public string? Street2 { get; set; }
		public string? AdditionalNumber { get; set; }
		public string? Address1 { get; set; }
		public string? Address2 { get; set; }
		public string? Address3 { get; set; }
		public string? Address4 { get; set; }
		public string? FirstResponsibleName { get; set; }
		public string? FirstResponsiblePhone { get; set; }
		public string? FirstResponsibleEmail { get; set; }
		public string? SecondResponsibleName { get; set; }
		public string? SecondResponsiblePhone { get; set; }
		public string? SecondResponsibleEmail { get; set; }

		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int BranchId { get; set; }
		public string? BranchName { get; set; }
		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
	}
}
