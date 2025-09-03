using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Menus;

namespace Shared.CoreOne.Models.Domain.Journal
{
    public class CostCenterJournalDetail : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int CostCenterJournalDetailId { get; set; }

		[Column(Order = 2)]
		public int? JournalDetailId { get; set; }

		[Column(Order = 3)]
		public int? ItemId { get; set; }

		[Column(Order = 4)]
        public int CostCenterId { get; set; }

        [Column(Order = 5)]
        public short? MenuCode { get; set; }

        [Column(Order = 6)]
        public int? ReferenceHeaderId { get; set; }

        [Column(Order = 7)]
        public int? ReferenceDetailId { get; set; }
        
        [Column(Order = 8)]
        public int Serial { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 9)]
		public decimal DebitValue { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 10)]
		public decimal CreditValue { get; set; }

		[Column(Order = 11)]
		public bool? IsCostCenterDistributed { get; set; }

		[StringLength(500)]
		[Column(Order = 12)]
		public string? RemarksAr { get; set; }

		[StringLength(500)]
		[Column(Order = 13)]
		public string? RemarksEn { get; set; }


		[ForeignKey(nameof(JournalDetailId))]
        public JournalDetail? JournalDetail { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

		[ForeignKey(nameof(CostCenterId))]
        public CostCenter? CostCenter { get; set; }

        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }
    }
}
