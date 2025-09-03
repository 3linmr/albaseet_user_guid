using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;

namespace Purchases.CoreOne.Models.Dtos.ViewModels
{
    public class ProductRequestDto
    {
        public ProductRequestHeaderDto? ProductRequestHeader { get; set; }
        public List<ProductRequestDetailDto> ProductRequestDetails { get; set; } = new List<ProductRequestDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

    public class ProductRequestHeaderDto
    {
        public int ProductRequestHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public string? DocumentReference { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public string? Reference { get; set; }
        public decimal ConsumerValue { get; set; }
        public decimal CostValue { get; set; }
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

    public class ProductRequestDetailDto
    {
        public int ProductRequestDetailId { get; set; }
        public int ProductRequestHeaderId { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameReport { get; set; }
        public byte TaxTypeId { get; set; }
		public byte? ItemTypeId { get; set; }
		public int ItemPackageId { get; set; }
        public bool IsItemVatInclusive { get; set; }
        public string? ItemPackageName { get; set; }
        public string? BarCode { get; set; }
        public decimal Packing { get; set; }
        public decimal Quantity { get; set; }
        public decimal ConsumerPrice { get; set; }
        public decimal ConsumerValue { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
        public decimal CostValue { get; set; }
        public string? Notes { get; set; }
        public string? ItemNote { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public string? Packages { get; set; }
    }
}
