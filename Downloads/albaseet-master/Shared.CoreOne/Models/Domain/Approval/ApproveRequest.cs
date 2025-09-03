using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Approval
{
    public class ApproveRequest : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }

        [Column(Order = 2)]
        public int RequestCode { get; set; }
        
        [Column(Order = 3)]
        public short MenuCode { get; set; }
        
		[DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 4)]
        public DateTime RequestDate { get; set; }

        [Column(Order = 5)]
        public byte ApproveRequestTypeId { get; set; }

		[Column(Order = 6)]
        public int ReferenceId { get; set; }//BaseIdFromScreen

        [StringLength(50)]
        [Column(Order = 7)]
		public string? ReferenceCode { get; set; }//CodeBasedOnIdFromScreen
        
        [StringLength(50)]
        [Column(Order = 8)]
        [Required]
        public string? FromUserName { get; set; }

        [Column(Order = 9)]
        public bool? IsApproved { get; set; }

        [Column(Order = 10)]
        public int ApproveId { get; set; }

        [Column(Order = 11)]
        public int CurrentStepId { get; set; }

        [Column(Order = 12)]
        public int CurrentStatusId { get; set; }

        [Column(Order = 13)]
        public int LastStepId { get; set; }

        [Column(Order = 14)]
        public int LastStatusId { get; set; }

        [Column(Order = 15)]
        public byte CurrentStepCount { get; set; }

        [Column(Order = 16)]
        public int CompanyId { get; set; }

        [Column(Order = 17)]
        public int BranchId { get; set; }

        [Column(Order = 18)]
        public int StoreId { get; set; }


      
        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }

		[ForeignKey(nameof(ApproveId))]
        public Approve? Approve { get; set; }

        [ForeignKey(nameof(ApproveRequestTypeId))]
        public ApproveRequestType? ApproveRequestType { get; set; }

		[ForeignKey(nameof(CurrentStepId))]
        public ApproveStep? CurrentApproveStep { get; set; }

        [ForeignKey(nameof(CurrentStatusId))]
        public ApproveStatus? CurrentApproveStatus { get; set; }

        [ForeignKey(nameof(LastStepId))]
        public ApproveStep? LastApproveStep { get; set; }

        [ForeignKey(nameof(LastStatusId))]
        public ApproveStatus? LastApproveStatus { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }
    }
}
