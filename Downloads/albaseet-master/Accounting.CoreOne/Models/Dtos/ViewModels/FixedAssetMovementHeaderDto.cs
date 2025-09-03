using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
    public class FixedAssetMovementHeaderDto
    {
        public int FixedAssetMovementHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; }
        public string? DocumentReference { get; set; }
        public int StoreId { get; set; }
        public int CostCenterToId { get; set; }
        public string? CostCenterToName { get; set; }
        public DateTime MovementDate { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public int? ArchiveHeaderId { get; set; }
    }
}
