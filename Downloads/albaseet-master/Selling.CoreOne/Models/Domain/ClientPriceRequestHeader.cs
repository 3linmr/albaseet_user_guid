using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;

namespace Sales.CoreOne.Models.Domain
{
    public class ClientPriceRequestHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClientPriceRequestHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int DocumentCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [StringLength(20)]
        [Column(Order = 5)]
        public string? DocumentReference { get; set; }

        [Column(Order = 6)]
        public int StoreId { get; set; }  
        
        [Column(Order = 7)]
        public int ClientId { get; set; }

        [Column(Order = 8)]
        public int? SellerId { get; set; }

		[DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 9)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [Column(Order = 10)]
        public DateTime EntryDate { get; set; }

        [StringLength(50)]
        [Column(Order = 11)]
        public string? Reference { get; set; }

        [Column(Order = 12, TypeName = "decimal(30,15)")]
        public decimal ConsumerValue { get; set; }

        [Column(Order = 13, TypeName = "decimal(30,15)")]
        public decimal CostValue { get; set; }

        [StringLength(2000)]
        [Column(Order = 14)]
        public string? RemarksAr { get; set; }

        [StringLength(2000)]
        [Column(Order = 15)]
        public string? RemarksEn { get; set; }

        [Column(Order = 16)]
        public bool IsClosed { get; set; }

        [Column(Order = 17)]
        public int? ArchiveHeaderId { get; set; }



        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller? Seller { get; set; }

		[ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
