using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Inventory
{
	public class StockType : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte StockTypeId { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 2)]
		public string? StockTypeNameAr { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 3)]
		public string? StockTypeNameEn { get; set; }
	}
}
