using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Approval
{
    public class ApproveStatus : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int ApproveStatusId { get; set; }

        [Column(Order = 2)]
        public int ApproveStepId { get; set; }

        [Required, StringLength(10)]
        [Column(Order = 3)]
        public string? StatusNameAr { get; set; }

        [Required, StringLength(10)]
        [Column(Order = 4)]
        public string? StatusNameEn { get; set; }

        [Column(Order = 5)]
        public  bool Approved { get; set; }
        
        [Column(Order = 6)]
        public bool Pending { get; set; }
        
        [Column(Order = 7)]
        public bool Rejected { get; set; }
        

        [ForeignKey(nameof(ApproveStepId))]
        public new ApproveStep? ApproveStep { get; set; }
    }
}
