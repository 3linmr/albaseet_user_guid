using Shared.CoreOne.Models.Domain.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;

namespace Sales.CoreOne.Models.Dtos.ViewModels
{
	public class ClientCreditMemoVm
	{
		public ClientCreditMemoDto? ClientCreditMemo { get; set; }
		public JournalDto? Journal { get; set; }
		public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
	}

	public class ClientCreditMemoDto
	{
		public int ClientCreditMemoId { get; set; }
		public string? Prefix { get; set; }
		public int DocumentCode { get; set; }
		public string? Suffix { get; set; }
		public string? DocumentFullCode { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
		public string? DocumentReference { get; set; }
		public int SalesInvoiceHeaderId { get; set; }
		public int ClientId { get; set; }
		public int ClientCode { get; set; }
		public string? ClientName { get; set; }
		public int? SellerId { get; set; }
		public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public string? Reference { get; set; }
		public int DebitAccountId { get; set; }
		public int CreditAccountId { get; set; }
		public decimal MemoValue { get; set; }
		public int JournalHeaderId { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public bool IsClosed { get; set; }
		public int? ArchiveHeaderId { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }

		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
		public string? IpAddressModified { get; set; }
	}
}
