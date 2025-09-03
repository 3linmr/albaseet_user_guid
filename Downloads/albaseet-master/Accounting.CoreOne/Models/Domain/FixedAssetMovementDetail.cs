using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.FixedAssets;

namespace Accounting.CoreOne.Models.Domain
{
    public class FixedAssetMovementDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FixedAssetMovementDetailId { get; set; }

        [Column(Order = 2)]
        public int FixedAssetMovementHeaderId { get; set; }

		[Column(Order = 3)]
        public int FixedAssetId { get; set; }

        [Column(Order = 4)]
		[StringLength(2000)]
        public string? RemarksAr { get; set; }

        [Column(Order = 5)]
		[StringLength(2000)]
        public string? RemarksEn { get; set; }



        [ForeignKey(nameof(FixedAssetMovementHeaderId))]
        public FixedAssetMovementHeader? FixedAssetMovementHeader { get; set; }
        
        [ForeignKey(nameof(FixedAssetId))]
        public FixedAsset? FixedAsset { get; set; }
    }
}
