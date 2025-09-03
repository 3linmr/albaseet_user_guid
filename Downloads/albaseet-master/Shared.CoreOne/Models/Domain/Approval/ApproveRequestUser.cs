using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Approval
{
	public class ApproveRequestUser : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int ApproveRequestUserId { get; set; }

		[Column(Order = 2)]
		public int RequestId { get; set; }

		[Column(Order = 3)]
		public int StepId { get; set; }

		[Column(Order = 4)]
		public int StatusId { get; set; }

		[StringLength(50)]
		[Required]
		[Column(Order = 5)]
		public string? UserName { get; set; }

		[StringLength(500)]
		[Column(Order = 6)]
		public string? Remarks { get; set; }


		[ForeignKey(nameof(RequestId))]
		public ApproveRequest? ApproveRequest { get; set; }

		[ForeignKey(nameof(StepId))]
		public ApproveStep? ApproveStep { get; set; }

		[ForeignKey(nameof(StatusId))]
		public ApproveStatus? ApproveStatus { get; set; }
	}
}
