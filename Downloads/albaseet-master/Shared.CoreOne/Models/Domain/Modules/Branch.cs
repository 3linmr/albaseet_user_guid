using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Branch : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BranchId { get; set; }

        [Column(Order = 2)]
        public int BranchCode { get; set; }

        [Required, StringLength(1000)]
        [Column(Order = 3)]
        public string? BranchNameAr { get; set; }

        [Required, StringLength(1000)]
        [Column(Order = 4)]
        public string? BranchNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

		[StringLength(50)]
        [Column(Order = 6)]
        public string? BranchPhone { get; set; }

        [StringLength(20)]
        [Column(Order = 7)]
        public string? BranchWhatsApp { get; set; }

        [StringLength(50)]
        [Column(Order = 8)]
        public string? BranchEmail { get; set; }

        [StringLength(100)]
        [Column(Order = 9)]
        public string? BranchWebsite { get; set; }

        [StringLength(2000)]
        [Column(Order = 10)]
        public string? BranchAddress { get; set; }


        [StringLength(500)]
        [Column(Order = 11)]
        public string? ResponsibleNameAr { get; set; }


        [StringLength(500)]
        [Column(Order = 12)]
        public string? ResponsibleNameEn { get; set; }


        [StringLength(20)]
        [Column(Order = 13)]
        public string? ResponsiblePhone { get; set; }

        [StringLength(20)]
        [Column(Order = 14)]
        public string? ResponsibleWhatsApp { get; set; }

        [StringLength(100)]
        [Column(Order = 15)]
        public string? ResponsibleEmail { get; set; }

        [StringLength(2000)]
        [Column(Order = 16)]
        public string? ResponsibleAddress { get; set; }

        [Column(Order = 17)]
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        [Column(Order = 18)]
        public string? InActiveReasons { get; set; }



        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
	}
}
