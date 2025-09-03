using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Inventory
{
	public class ItemNegativeSalesHeader : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int ItemNegativeSalesHeaderId { get; set; }

		[Column(Order = 2)]
		public int StoreId { get; set; }

		[Column(TypeName = "Date", Order = 3)]
		public DateTime DocumentDate { get; set; }

		[Column(Order = 4)]
		public DateTime EntryDate { get; set; }

		[Column(Order = 5)]
		public int ItemId { get; set; }

		[Column(Order = 6)]
		public int ItemPackageId { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 7)]
		public DateTime? ExpireDate { get; set; }

		[Column(Order = 8)]
		[StringLength(50)]
		public string? BatchNumber { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 9)]
		public decimal ItemPreviousQuantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 10)]
		public decimal NegativeSalesQuantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 11)]
		public decimal SettledQuantity { get; set; }

		[Column(Order = 12)]
		public bool IsSettled { get; set; }

		[StringLength(2000)]
		[Column(Order = 13)]
		public string? RemarksAr { get; set; }

		[StringLength(2000)]
		[Column(Order = 14)]
		public string? RemarksEn { get; set; }



		[ForeignKey(nameof(StoreId))]
		public Store? Store { get; set; }

		[ForeignKey(nameof(ItemId))]
		public Item? Item { get; set; }

		[ForeignKey(nameof(ItemPackageId))]
		public ItemPackage? ItemPackage { get; set; }
	}
}
