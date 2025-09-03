using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;

namespace Inventory.CoreOne.Models.Dtos.ViewModels
{
    public class InventoryOutDto
    {
        public InventoryOutHeaderDto? InventoryOutHeader { get; set; }
        public List<InventoryOutDetailDto> InventoryOutDetails { get; set; } = new List<InventoryOutDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

    public class InventoryOutHeaderDto
    {
        public int InventoryOutHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int InventoryOutCode { get; set; }
        public string? Suffix { get; set; }
        public string? InventoryOutFullCode { get; set; }
        public string? DocumentReference { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public string? Reference { get; set; }
        public decimal TotalConsumerValue { get; set; }
        public decimal TotalCostValue { get; set; }
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

    public class InventoryOutDetailDto
    {
        public int InventoryOutDetailId { get; set; }
        public int InventoryOutHeaderId { get; set; }
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
        public bool? IsLinkedToCostCenters { get; set; }
        public bool? IsCostCenterDistributed { get; set; }


        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public string? Packages { get; set; }

        public List<CostCenterJournalDetailDto> CostCenters { get; set; } = new List<CostCenterJournalDetailDto>();
    }
}
