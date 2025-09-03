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

namespace Inventory.CoreOne.Models.Dtos.ViewModels
{
    public class InternalTransferReceiveDto
    {
        public InternalTransferReceiveHeaderDto? InternalTransferReceiveHeader { get; set; }
        public List<InternalTransferReceiveDetailDto> InternalTransferReceiveDetails { get; set; } = new List<InternalTransferReceiveDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();

    }

    public class InternalTransferReceiveHeaderDto
    {
        public int InternalTransferReceiveHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int InternalTransferReceiveCode { get; set; }
        public string? Suffix { get; set; }
        public string? InternalTransferReceiveFullCode { get; set; }
        public string? DocumentReference { get; set; }
        public int InternalTransferHeaderId { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow(); //like in the stockTaking Header Dto
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
		public int? ArchiveHeaderId { get; set; }
        public bool IsClosed { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }
    }

    public class InternalTransferReceiveDetailDto
    {
        public int InternalTransferReceiveDetailId { get; set; }
        public int InternalTransferReceiveHeaderId { get; set; }
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
}
