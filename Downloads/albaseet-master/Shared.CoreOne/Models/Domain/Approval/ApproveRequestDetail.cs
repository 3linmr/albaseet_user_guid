using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Approval
{
	public class ApproveRequestDetail : BaseObject
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(Order = 1)] 
		public int ApproveRequestDetailId { get; set; }

		[Column(Order = 2)]
		public int ApproveRequestId { get; set; }

		[Column(Order = 3)]
		public string? OldValue { get; set; }

		[Column(Order = 4)]
		public string? NewValue { get; set; }

		[Column(Order = 5)]
		public string? Changes { get; set; }


		[ForeignKey(nameof(ApproveRequestId))]
		public ApproveRequest? ApproveRequest { get; set; }
	}
}
