using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;

namespace Shared.CoreOne.Models.Domain.Inventory
{
	public class ItemNegativeSalesDetail : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ItemNegativeSalesDetailId { get; set; }

		[Column(Order = 2)]
		public int ItemNegativeSalesHeaderId { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 3)]
		public decimal SettledQuantity { get; set; }

		[Column(Order = 4)]
		public short? MenuCode { get; set; }

		[Column(Order = 5)]
		public int? ReferenceHeaderId { get; set; }

		[Column(Order = 6)]
		public int? ReferenceDetailId { get; set; }

		[StringLength(2000)]
		[Column(Order = 7)]
		public string? RemarksAr { get; set; }

		[StringLength(2000)]
		[Column(Order = 8)]
		public string? RemarksEn { get; set; }


		[ForeignKey(nameof(ItemNegativeSalesHeaderId))]
		public ItemNegativeSalesHeader? ItemNegativeSalesHeader { get; set; }

		[ForeignKey(nameof(MenuCode))]
		public Menu? Menu { get; set; }
	}
}
