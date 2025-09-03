using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Inventory
{
	public class ItemDisassembleHeader : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ItemDisassembleHeaderId { get; set; }

		[Column(Order = 2)]
		public int ItemDisassembleCode { get; set; }

		[Column(Order = 3)]
		public int StoreId { get; set; }

		[Column(Order = 4)]
		public DateTime EntryDate { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 5)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime DocumentDate { get; set; }

		[Column(Order = 6)]
		public bool IsAutomatic { get; set; }

		[Column(Order = 7)]
		public short? MenuCode { get; set; }

		[Column(Order = 8)]
		public int? ReferenceHeaderId { get; set; }

		[Column(Order = 9)]
		public int? ReferenceDetailId { get; set; }

		[StringLength(2000)]
		[Column(Order = 10)]
		public string? RemarksAr { get; set; }

		[StringLength(2000)]
		[Column(Order = 11)]
		public string? RemarksEn { get; set; }


		[ForeignKey(nameof(StoreId))]
		public Store? Store { get; set; }

		[ForeignKey(nameof(MenuCode))]
		public Menu? Menu { get; set; }
	}
}
