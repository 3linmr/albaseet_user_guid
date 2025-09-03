using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Menus;

namespace Inventory.CoreOne.Models.Domain
{
    public class InternalTransferReceiveHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InternalTransferReceiveHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int InternalTransferReceiveCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [StringLength(20)]
        [Column(Order = 5)]
        public string? DocumentReference { get; set; }

		[Column(Order = 6)]
        public int InternalTransferHeaderId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 7)]
        public DateTime DocumentDate { get; set; }

        [Column(Order = 8)]
        public DateTime EntryDate { get; set; }

		[Column(Order = 9)]
        public int FromStoreId { get; set; }

        [Column(Order = 10)]
        public int ToStoreId { get; set; }

        [Column(Order = 11)]
        [StringLength(50)]
        public string? Reference { get; set; }

        [Column(Order = 12, TypeName = "decimal(30,15)")]
        public decimal TotalConsumerValue { get; set; }

        [Column(Order = 13, TypeName = "decimal(30,15)")]
        public decimal TotalCostValue { get; set; }

        [Column(Order = 14)]
        [StringLength(500)]
        public string? RemarksAr { get; set; }

        [Column(Order = 15)]
        [StringLength(500)]
        public string? RemarksEn { get; set; }

        [Column(Order = 16)]
        public short? MenuCode { get; set; }

        [Column(Order = 17)]
        public int? ReferenceId { get; set; }

		[Column(Order = 18)]
        public bool IsClosed { get; set; }

        [Column(Order = 19)]
        public int? ArchiveHeaderId { get; set; }


		[ForeignKey(nameof(InternalTransferHeaderId))]
        public InternalTransferHeader? InternalTransferHeader { get; set; }

        [ForeignKey(nameof(FromStoreId))]
        public Store? FromStore { get; set; }

        [ForeignKey(nameof(ToStoreId))]
        public Store? ToStore { get; set; }

        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }

		[ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
