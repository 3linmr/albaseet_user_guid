using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Inventory
{
    public class ItemDisassembleDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int ItemDisassembleDetailId { get; set; }

        [Column(Order = 2)]
		public int ItemDisassembleHeaderId { get; set; }

        [Column(Order = 3)]
		public bool IsSerialConversion { get; set; }

		[Column(Order = 4)]
		public int ItemId { get; set; }

		[Column(Order = 5)]
		public int FromPackageId { get; set; }

		[Column(Order = 6)]
		public int ToPackageId { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 7)]
		public DateTime? ExpireDate { get; set; }

		[Column(Order = 8)]
		[StringLength(50)]
		public string? BatchNumber { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 9)]
		public decimal Packing { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 10)]
		public decimal Quantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 11)]
		public decimal FromPackageQuantityBefore { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 12)]
		public decimal ToPackageQuantityBefore { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 13)]
		public decimal FromPackageQuantityAfter { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 14)]
		public decimal ToPackageQuantityAfter { get; set; }



		[ForeignKey(nameof(ItemDisassembleHeaderId))]
		public ItemDisassembleHeader? ItemDisassembleHeader { get; set; }
		
		[ForeignKey(nameof(ItemId))]
		public Item? Item { get; set; }

		[ForeignKey(nameof(FromPackageId))]
		public ItemPackage? FromPackage { get; set; }

		[ForeignKey(nameof(ToPackageId))]
		public ItemPackage? ToPackage { get; set; }
	}
}
