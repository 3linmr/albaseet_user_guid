using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Items
{
	public class ItemBarCodeDetail : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int ItemBarCodeDetailId { get; set; }

		[Column(Order = 2)]
		public int ItemBarCodeId { get; set; }

		[StringLength(200)]
		[Column(Order = 3)]
		public string? BarCode { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 4)]
		public decimal ConsumerPrice { get; set; }


		[ForeignKey(nameof(ItemBarCodeId))]
		public ItemBarCode? ItemBarCode { get; set; }
	}
}
