using Shared.CoreOne.Models.Domain.Accounts;
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
    public class FixedAssetVoucherDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FixedAssetVoucherDetailId { get; set; }

        [Column(Order = 2)]
        public int FixedAssetVoucherHeaderId { get; set; }

        [Column(Order = 3)]
        public int FixedAssetId { get; set; }

        [Column(Order = 4)]
        [StringLength(500)]
        public string? RemarksAr { get; set; }

        [Column(Order = 5)]
        [StringLength(500)]
        public string? RemarksEn { get; set; }

        [Column(Order = 6)]
        public decimal? DetailValue { get; set; }


        [ForeignKey(nameof(FixedAssetVoucherHeaderId))]
        public FixedAssetVoucherHeader? FixedAssetVoucherHeader { get; set; }

        [ForeignKey(nameof(FixedAssetId))]
        public FixedAsset? FixedAsset { get; set; }
    }
}
