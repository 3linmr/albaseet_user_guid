using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Shared.CoreOne.Models.Domain.Journal
{
    public class JournalDetail : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int JournalDetailId { get; set; }

        [Column(Order = 2)]
        public int JournalHeaderId { get; set; }
       
        [Column(Order = 3)]
        public int Serial { get; set; }
        
        [Column(Order = 4)]
        public int AccountId { get; set; }
        
        [Column(Order = 5)]
        public short CurrencyId { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 6)]
		public decimal CurrencyRate { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 7)]
		public decimal DebitValue { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 8)]
		public decimal DebitValueAccount { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 9)]
		public decimal CreditValue { get; set; }
		
		[Column(TypeName = "decimal(30,15)", Order = 10)]
		public decimal CreditValueAccount { get; set; }

		[Column(Order = 11)]
		public bool IsTax { get; set; }

		[Column(Order = 12)]
		public byte? TaxTypeId { get; set; }

		[Column(Order = 13)]
        public int? TaxId { get; set; }

        [Column(Order = 14)]
        public int? TaxParentId { get; set; } // JournalDetailId => 

        [Column(Order = 15, TypeName = "decimal(30,15)")]
		public decimal TaxPercent { get; set; }

		[StringLength(500)]
        [Column(Order = 16)]
        public string? RemarksAr { get; set; }

        [StringLength(500)]
        [Column(Order = 17)]
        public string? RemarksEn { get; set; }

        [StringLength(2000)]
        [Column(Order = 18)]
        public string? AutomaticRemarks { get; set; }

        [Column(Order = 19)]
        public byte? EntityTypeId { get; set; }

		[Column(Order = 20)]
        public int? EntityId { get; set; }

		[StringLength(300)]
		[Column(Order = 21)]
		public string? EntityNameAr { get; set; }

		[StringLength(300)]
		[Column(Order = 22)] 
		public string? EntityNameEn { get; set; }

		[StringLength(50)]
		[Column(Order = 23)] 
		public string? TaxCode { get; set; }

		[StringLength(100)]
		[Column(Order = 24)] 
		public string? EntityEmail { get; set; }

		[StringLength(50)]
		[Column(Order = 25)] 
		public string? TaxReference { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 26)] 
		public DateTime? TaxDate { get; set; }

		[Column(Order = 27)]
		public bool? IsLinkedToCostCenters { get; set; }

		[Column(Order = 28)]
		public bool? IsCostCenterDistributed { get; set; }

		[Column(Order = 29)]
		public bool IsSystematic { get; set; }

		[Column(Order = 30)]
		public bool IsCostRelated { get; set; }


		[ForeignKey(nameof(JournalHeaderId))]
        public JournalHeader? JournalHeader { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

		[ForeignKey(nameof(TaxTypeId))]
		public TaxType? TaxType { get; set; }

		[ForeignKey(nameof(TaxId))]
        public Tax? Tax { get; set; }   

        [ForeignKey(nameof(CurrencyId))]
        public Currency? Currency { get; set; } 
        
        [ForeignKey(nameof(EntityTypeId))]
        public EntityType? EntityType { get; set; }
    }
}
