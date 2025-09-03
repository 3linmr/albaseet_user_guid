using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.FixedAssets;

namespace Accounting.CoreOne.Models.Domain
{
    public class FixedAssetVoucherHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FixedAssetVoucherHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int DocumentCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [Column(Order = 5)]
        public byte FixedAssetVoucherTypeId { get; set; }

        [StringLength(20)]
        [Column(Order = 6)]
        public string? DocumentReference { get; set; }

        [Column(Order = 7)]
        public int StoreId { get; set; }

        [Column(Order = 8)]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 9)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [StringLength(50)]
        [Column(Order = 10)]
        public string? PeerReference { get; set; }

        [Column(Order = 11)]
        [StringLength(50)]
        public string? FixedAssetReference { get; set; }

        [Column(Order = 12)]
        public int? SellerId { get; set; }

        [Column(Order = 13)]
        [StringLength(500)]
        public string? RemarksAr { get; set; }

        [Column(Order = 14)]
        [StringLength(500)]
        public string? RemarksEn { get; set; }

        [Column(Order = 15)]
        public int? JournalHeaderId { get; set; }

        [Column(Order = 16)]
        public int? ArchiveHeaderId { get; set; }

        
        [ForeignKey(nameof(FixedAssetVoucherTypeId))]
        public FixedAssetVoucherType? FixedAssetVoucherType { get; set; }   
        
        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller? Seller { get; set; }

        [ForeignKey(nameof(JournalHeaderId))]
        public JournalHeader? JournalHeader { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }

    }
}