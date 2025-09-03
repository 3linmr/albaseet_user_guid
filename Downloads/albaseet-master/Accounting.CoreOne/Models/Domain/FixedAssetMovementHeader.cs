using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Models.Domain
{
    public class FixedAssetMovementHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FixedAssetMovementHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int DocumentCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 5)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [Column(Order = 6)]
        public DateTime EntryDate { get; set; }

        [StringLength(20)]
        [Column(Order = 7)]
        public string? DocumentReference { get; set; }

        [Column(Order = 8)]
        public int StoreId { get; set; }

        [Column(Order = 9)]
        public int CostCenterToId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 10)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime MovementDate { get; set; }

        [Column(Order = 11)]
        [StringLength(2000)]
        public string? RemarksAr { get; set; }

        [Column(Order = 12)]
        [StringLength(2000)]
        public string? RemarksEn { get; set; }

        [Column(Order = 13)]
        public int? ArchiveHeaderId { get; set; }


        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(CostCenterToId))]
        public CostCenter? CostCenterTo { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
