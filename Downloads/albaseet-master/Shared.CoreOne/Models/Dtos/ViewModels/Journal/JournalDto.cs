using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;


namespace Shared.CoreOne.Models.Dtos.ViewModels.Journal
{
	public class JournalDto
	{
		public JournalHeaderDto? JournalHeader { get; set; }
		public List<JournalDetailDto>? JournalDetails { get; set; }
		public List<CostCenterJournalDetailDto>? CostCenterJournalDetails { get; set; }
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }
	public class JournalVm
	{
		public JournalHeaderDto? JournalHeader { get; set; }
		public List<JournalDetailVm>? JournalDetails { get; set; }
	}
	public class JournalDetailVm
	{
		public JournalDetailDto? JournalDetail { get; set; }
		public List<CostCenterJournalDetailDto>? CostCenterJournalDetails { get; set; }
	}

	public class JournalDetailReturnDto
	{
		public string? Mode { get; set; }
		public JournalDetail? JournalDetail { get; set; }
	}

	public class CostCenterJournalDetailReturnDto
	{
		public List<CostCenterJournalDetail>? JournalDetailCreate { get; set; }
		public List<CostCenterJournalDetail>? JournalDetailUpdate { get; set; }
	}

	public class JournalHeaderDto
	{
		public int JournalHeaderId { get; set; }
		public int JournalId { get; set; }
		public byte JournalTypeId { get; set; }
		public string? JournalTypeName { get; set; }
		public int TicketId { get; set; }
		public int StoreId { get; set; }
		public int StoreCurrencyId { get; set; }
		public string? StoreName { get; set; }
		public string? Prefix { get; set; }
		public int JournalCode { get; set; }
		public string? JournalCodeFull { get; set; }
		public string? Suffix { get; set; }
		public string? DocumentReference { get; set; }
        public decimal TotalDebitValue { get; set; }
        public decimal TotalDebitValueAccount { get; set; }
        public decimal TotalCreditValue { get; set; }
        public decimal TotalCreditValueAccount { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
		public DateTime TicketDate { get; set; }
		public string? PeerReference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public bool IsClosed { get; set; }
		public bool IsCancelled { get; set; }
		public bool IsSystematic { get; set; }
		public short? MenuCode { get; set; }
		public int? MenuReferenceId { get; set; }
		public int? ArchiveHeaderId { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? IpAddressCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}

	public class JournalDetailDto
	{
		public int JournalDetailId { get; set; }
		public int JournalHeaderId { get; set; }
		public int Serial { get; set; }
		public int AccountId { get; set; }
		public short CurrencyId { get; set; }
		public decimal CurrencyRate { get; set; }
		public decimal DebitValue { get; set; }
		public decimal DebitValueAccount { get; set; }
		public decimal CreditValue { get; set; }
		public decimal CreditValueAccount { get; set; }
		public bool IsTax { get; set; }
		public int? TaxId { get; set; }
		public byte? TaxTypeId { get; set; }
		public int? TaxParentId { get; set; } // JournalDetailId => 
		public decimal TaxPercent { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public string? AutomaticRemarks { get; set; }
		public byte? EntityTypeId { get; set; }
		public string? EntityTypeName { get; set; }
		public int? EntityId { get; set; }
		public string? EntityNameAr { get; set; }
		public string? EntityNameEn { get; set; }
		public string? TaxCode { get; set; }
		public string? EntityEmail { get; set; }
		public string? TaxReference { get; set; }
		public DateTime? TaxDate { get; set; }
		public bool? IsLinkedToCostCenters { get; set; }
		public bool? IsCostCenterDistributed { get; set; }
		public bool IsSystematic { get; set; }
		public bool IsCostRelated { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class CostCenterJournalDetailDto
	{
		public int CostCenterJournalDetailId { get; set; }
		public int? JournalDetailId { get; set; }
		public int? ItemId { get; set; }
		public short? MenuCode { get; set; }
		public int? ReferenceHeaderId { get; set; }
		public int? ReferenceDetailId { get; set; }
		public int CostCenterId { get; set; }
		public bool Distributed { get; set; }
		public int Serial { get; set; }
		public decimal DebitValue { get; set; }
		public decimal CreditValue { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class JournalDetailCalculationVm
	{
		public ResponseDto? Response { get; set; }
		public List<JournalDetailCalculationDto>? JournalDetailCalculations { get; set; }
	}

	public class JournalDetailCalculationDto
	{
		public int AccountId { get; set; }
		public int? CommissionTaxId { get; set; }
		public byte? CommissionTaxTypeId { get; set; }
		public short CurrencyId { get; set; }
		public decimal CurrencyRate { get; set; }
		public decimal DebitValue { get; set; }
		public decimal DebitValueAccount { get; set; }
		public decimal CreditValue { get; set; }
		public decimal CreditValueAccount { get; set; }
		public string? AutomaticRemarks { get; set; }

	}

	public class JournalCompanyDto
	{
		public int JournalHeaderId { get; set; }
		public int JournalDetailId { get; set; }
		public int JournalCode { get; set; }
		public int CompanyId { get; set; }
		public DateTime TicketDate { get; set; }
		public string? TaxReference { get; set; }
		public string? EntityName { get; set; }
	}

	public class JournalDetailTaxReferenceDto
	{
		public List<string> TaxReferences { get; set; } = new List<string>();
		public DateTime TicketDate { get; set; }
	}
}
