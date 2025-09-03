using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.FixedAssets
{
    public class FixedAsset : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FixedAssetId { get; set; }

        [StringLength(50)]
        [Column(Order = 2)]
        public string? FixedAssetCode { get; set; }

        [Column(Order = 3)]
        [StringLength(1000)]
        public string? FixedAssetNameAr { get; set; }

        [Column(Order = 4)]
        [StringLength(1000)]
        public string? FixedAssetNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

        [Column(Order = 6)]
        public bool IsMainFixedAsset { get; set; }

        [Column(Order = 7)]
        public int? MainFixedAssetId { get; set; }

        [Column(Order = 8)]
        public byte FixedAssetLevel { get; set; }

        [Column(Order = 9)]
        public int Order { get; set; }

        [Column(Order = 10)]
        public bool IsLastLevel { get; set; }

        [Column(Order = 11)]
        public int? AccountId { get; set; }

        [Column(Order = 12)]
        public int? DepreciationAccountId { get; set; }

        [Column(Order = 13)]
        public int? CumulativeDepreciationAccountId { get; set; }

        [Column(Order = 14, TypeName = "decimal(30,15)")]
        public decimal AnnualDepreciationPercent { get; set; }

        [Column(Order = 15)]
        public bool IsPrivate { get; set; }

        [Column(Order = 16)]
        public bool IsActive { get; set; }

        [StringLength(2000)]
        [Column(Order = 17)]
        public string? InActiveReasonsAr { get; set; }

        [StringLength(2000)]
        [Column(Order = 18)]
        public string? InActiveReasonsEn { get; set; }

        [Column(Order = 19)]
        public bool HasRemarks { get; set; }

        [Column(Order = 20)]
        [StringLength(2000)]
        public string? RemarksAr { get; set; }

        [Column(Order = 21)]
        [StringLength(2000)]
        public string? RemarksEn { get; set; }

        [Column(Order = 22)]
        public bool IsNonEditable { get; set; }

        [Column(Order = 23)]
        [StringLength(2000)]
        public string? NotesAr { get; set; }

        [Column(Order = 24)]
        [StringLength(2000)]
        public string? NotesEn { get; set; }

        [Column(Order = 25)]
        public int? ArchiveHeaderId { get; set; }



        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        
        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

        [ForeignKey(nameof(DepreciationAccountId))]
        public Account? DepreciationAccount { get; set; }

        [ForeignKey(nameof(CumulativeDepreciationAccountId))]
        public Account? CumulativeDepreciationAccount { get; set; }      
        
		[ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }    
    }
}
