using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.CostCenters
{
    public class CostCenter : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CostCenterId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)]
        public string? CostCenterCode { get; set; }

		[Column(Order = 3)]
        [Required, StringLength(500)]
        public string? CostCenterNameAr { get; set; }

        [Column(Order = 4)]
        [Required, StringLength(500)]
        public string? CostCenterNameEn { get; set; }

        [Column(Order = 5)]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }

        [Column(Order = 7)]
        public bool IsMainCostCenter { get; set; }

        [Column(Order = 8)]
        public int? MainCostCenterId { get; set; }

        [Column(Order = 9)]
        public byte CostCenterLevel { get; set; }

        [Column(Order = 10)]
        public int Order { get; set; }

        [Column(Order = 11)]
        public bool IsLastLevel { get; set; }

        [Column(Order = 12)]
        public bool IsPrivate { get; set; }

		[Column(Order = 13)]
        public bool IsActive { get; set; }

        [StringLength(200)]
        [Column(Order = 14)]
        public string? InActiveReasons { get; set; }

		[Column(Order = 15)]
		public bool HasRemarks { get; set; }

		[Column(Order = 16)]
		[StringLength(500)]
		public string? RemarksAr { get; set; }

		[Column(Order = 17)]
		[StringLength(500)]
		public string? RemarksEn { get; set; }

		[Column(Order = 18)]
		public bool IsNonEditable { get; set; }

		[Column(Order = 19)]
		[StringLength(500)]
		public string? NotesAr { get; set; }

		[Column(Order = 20)]
		[StringLength(500)]
		public string? NotesEn { get; set; }

		[Column(Order = 21)]
		public int? ArchiveHeaderId { get; set; }



		[ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }


        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
	}
}
