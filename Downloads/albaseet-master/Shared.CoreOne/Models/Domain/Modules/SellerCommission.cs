using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
	public class SellerCommission : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int SellerCommissionId { get; set; }

		[Column(Order = 2)]
		public short SellerCommissionMethodId { get; set; }

		[Column(Order = 3)]
		public int From { get; set; }

		[Column(Order = 4)]
		public int To { get; set; }

        [Column(Order = 5, TypeName = "decimal(5,2)")]
		public decimal CommissionPercent { get; set; }


		[ForeignKey(nameof(SellerCommissionMethodId))]
		public SellerCommissionMethod? SellerCommissionMethod { get; set; }
	}
}
