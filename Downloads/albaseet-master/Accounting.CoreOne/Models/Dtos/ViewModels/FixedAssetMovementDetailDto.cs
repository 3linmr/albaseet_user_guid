using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.FixedAssets;
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

namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
    public class FixedAssetMovementDetailDto
    {
        public int FixedAssetMovementDetailId { get; set; }
        public int FixedAssetMovementHeaderId { get; set; }
        public string? FixedAssetCode { get; set; }
        public int FixedAssetId { get; set; }
        public string? FixedAssetName { get; set; }
        public DateTime? MovementDate { get; set; }
        public int? CostCenterToId { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
    }
}
