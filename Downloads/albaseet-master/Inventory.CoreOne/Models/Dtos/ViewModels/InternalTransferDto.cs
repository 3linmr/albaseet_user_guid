using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;


namespace Inventory.CoreOne.Models.Dtos.ViewModels
{
    public class InternalTransferDto
    {
        public InternalTransferHeaderDto? InternalTransferHeader { get; set; }
        public List<InternalTransferDetailDto> InternalTransferDetails { get; set; } = new List<InternalTransferDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();

    }

    public class InternalTransferHeaderDto
    {
        public int InternalTransferHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int InternalTransferCode { get; set; }
        public string? Suffix { get; set; }
        public string? InternalTransferFullCode { get; set; }
        public string? DocumentReference { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public int FromStoreId { get; set; }
        public string? FromStoreName { get; set; }
        public int ToStoreId { get; set; }
        public string? ToStoreName { get; set; }
        public string? Reference { get; set; }
        public decimal TotalConsumerValue { get; set; }
        public decimal TotalCostValue { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public bool IsReturned { get; set; }
        public string? ReturnReason { get; set; }
        public short? MenuCode { get; set; }
        public int? ReferenceId { get; set; }
		public bool IsClosed { get; set; }
        public int? ArchiveHeaderId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public string? IpAddressModified { get; set; }
    }

    public class InternalTransferDetailDto
    {
        public int InternalTransferDetailId { get; set; }
        public int InternalTransferHeaderId { get; set; }
        public int StoreId { get; set; }
        public decimal AvailableBalance { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public byte? ItemTypeId { get; set; }
        public int ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
        public string? BarCode { get; set; }
        public decimal Packing { get; set; }
        public decimal Quantity { get; set; }
        public decimal ConsumerPrice { get; set; }
        public decimal ConsumerValue { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
        public decimal CostValue { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? BatchNumber { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public string? Packages { get; set; }
    }

    public class FromAndToStoreItemCurrentBalanceDto
    {
        public List<ItemCurrentBalanceDto> FromStoreCurrentBalances { get; set; } = [];
        public List<ItemCurrentBalanceDto> ToStoreCurrentBalances { get; set; } = [];

	}
}
