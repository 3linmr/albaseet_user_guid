using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Modules;

namespace Inventory.CoreOne.Models.Domain
{
    public class StockTakingHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int StockTakingHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int StockTakingCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [StringLength(20)]
        [Column(Order = 5)]
        public string? DocumentReference { get; set; }

		[Column(Order = 6)]
        public int StoreId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 7)]
        public string? StockTakingNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 8)]
        public string? StockTakingNameEn { get; set; }

        [Column(Order = 9)]
        public bool IsOpenBalance { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order =10)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StockDate { get; set; }

        [Column(Order = 11)]
        public DateTime EntryDate { get; set; }

        [StringLength(50)]
        [Column(Order = 12)]
        public string? Reference { get; set; }

        [Column(Order = 13, TypeName = "decimal(30,15)")]
        public decimal TotalConsumerValue { get; set; }

        [Column(Order = 14, TypeName = "decimal(30,15)")]
        public decimal TotalCostValue { get; set; }

		[Column(Order = 15)]
        [StringLength(500)]
        public string? RemarksAr { get; set; }

        [Column(Order = 16)]
        [StringLength(500)]
        public string? RemarksEn { get; set; }

		[Column(Order = 17)]
        public bool IsClosed { get; set; }

        [Column(Order = 18)]
        public bool IsDeleted { get; set; }

        [Column(Order = 19)]
        public bool IsCarriedOver { get; set; }


        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }
    }
}
