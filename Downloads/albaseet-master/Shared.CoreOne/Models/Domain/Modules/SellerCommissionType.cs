using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
	public class SellerCommissionType : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte SellerCommissionTypeId { get; set; }

		[Column(Order = 2)]
		[StringLength(50)]
		public string? SellerCommissionTypeNameAr { get; set; }

		[Column(Order = 3)]
		[StringLength(50)]
		public string? SellerCommissionTypeNameEn { get; set; }
	}
}
