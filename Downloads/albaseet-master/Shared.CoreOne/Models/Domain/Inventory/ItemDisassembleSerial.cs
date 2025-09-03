using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;

namespace Shared.CoreOne.Models.Domain.Inventory
{
	public class ItemDisassembleSerial : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ItemDisassembleSerialId { get; set; }

		[Column(Order = 2)]
		public int ItemDisassembleHeaderId { get; set; }

		[Column(Order = 3)]
		public int ItemId { get; set; }

		[Column(Order = 4)]
		public int ItemPackageId { get; set; }

		[Column(Order = 5)]
		public int? MainItemPackageId { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 6)]
		public decimal ItemPackageBalanceBefore { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 7)]
		public decimal ItemPackageBalanceAfter { get; set; }


		[ForeignKey(nameof(ItemDisassembleHeaderId))]
		public ItemDisassembleHeader? ItemDisassembleHeader { get; set; }

		[ForeignKey(nameof(ItemId))]
		public Item? Item { get; set; }

		[ForeignKey(nameof(ItemPackageId))]
		public ItemPackage? ItemPackage { get; set; }

		[ForeignKey(nameof(MainItemPackageId))]
		public ItemPackage? MainItemPackage { get; set; }
	}
}
