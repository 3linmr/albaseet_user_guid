using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;

namespace Inventory.CoreOne.Models.Domain
{
    public class InventoryInHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InventoryInHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int InventoryInCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [StringLength(20)]
        [Column(Order = 5)]
        public string? DocumentReference { get; set; }

		[Column(Order = 6)]
        public int StoreId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 7)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [Column(Order = 8)]
        public DateTime EntryDate { get; set; }

		[Column(Order = 9)]
        [StringLength(20)]
        public string? Reference { get; set; }

        [Column(Order = 10, TypeName = "decimal(30,15)")]
        public decimal TotalConsumerValue { get; set; }

        [Column(Order = 11, TypeName = "decimal(30,15)")]
        public decimal TotalCostValue { get; set; }

        [Column(Order = 12)]
        [StringLength(300)]
        public string? RemarksAr { get; set; }

        [Column(Order = 13)]
        [StringLength(300)]
        public string? RemarksEn { get; set; }

        [Column(Order = 14)]
        public bool IsClosed { get; set; }

        [Column(Order = 15)]
        public int? ArchiveHeaderId { get; set; }


        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }

    }
}
