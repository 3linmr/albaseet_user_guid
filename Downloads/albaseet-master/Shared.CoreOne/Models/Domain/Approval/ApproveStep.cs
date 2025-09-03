using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Approval
{
    public class ApproveStep : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int ApproveStepId { get; set; }

        [Column(Order = 2)]
        public int ApproveId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? StepNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? StepNameEn { get; set; }

        [Column(Order = 5)]
        public byte ApproveOrder { get; set; }

        [Column(Order = 6)]
        public  byte ApproveCount { get; set; }

        [Column(Order = 7)]
        public bool AllCountShouldApprove { get; set; }

        [Column(Order = 8)]
        public bool IsDeleted { get; set; }

        [Column(Order = 9)]
        public bool IsLastStep { get; set; }

        
        [ForeignKey(nameof(ApproveId))]
        public Approve? Approve { get; set; }
    }
}
