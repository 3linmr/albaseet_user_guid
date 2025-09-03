using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Approval
{
	public class ApproveRequestType : BaseObject
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(Order = 1)] 
		public byte ApproveRequestTypeId { get; set; }

        [StringLength(20)]
		public string? ApproveRequestTypeNameAr { get; set; }

        [StringLength(20)]
		public string? ApproveRequestTypeNameEn { get; set; }
	}
}
