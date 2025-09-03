using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Menus;

namespace Shared.CoreOne.Models.Domain.Journal
{
    public class JournalHeader : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public int JournalHeaderId { get; set; }

        [Column(Order = 2)]
        public int JournalId { get; set; }
     
        [Column(Order = 3)]
        public byte JournalTypeId { get; set; }
        
        [Column(Order = 4)]
        public int TicketId { get; set; }

        [Column(Order = 5)]
        public int StoreId { get; set; }

        [StringLength(10)]
        [Column(Order = 6)]
        public string? Prefix { get; set; }

        [Column(Order = 7)]
        public int JournalCode { get; set; }

        [StringLength(10)]
        [Column(Order = 8)]
        public string? Suffix { get; set; }

        [StringLength(20)]
        [Column(Order = 9)]
        public string? DocumentReference { get; set; }

		[Column(Order = 10)]
        public DateTime EntryDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 11)] 
        public DateTime TicketDate { get; set; }

        [StringLength(50)]
        [Column(Order = 12)]
        public string? PeerReference { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 13)]
        public decimal TotalDebitValue { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 14)]
        public decimal TotalDebitValueAccount { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 15)]
        public decimal TotalCreditValue { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 16)]
        public decimal TotalCreditValueAccount { get; set; }

        [StringLength(2000)]
        [Column(Order = 17)]
        public string? RemarksAr { get; set; }
        
        [StringLength(2000)]
        [Column(Order = 18)]
        public string? RemarksEn { get; set; }

        [Column(Order = 19)]
        public bool IsClosed { get; set; }

        [Column(Order = 20)]
        public bool IsCancelled { get; set; }

		[Column(Order = 21)]
        public bool IsSystematic { get; set; }

		[Column(Order = 22)]
        public short? MenuCode { get; set; }

		[Column(Order = 23)]
        public int? MenuReferenceId { get; set; }

        [Column(Order = 24)]
        public int? ArchiveHeaderId { get; set; }



        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(JournalTypeId))]
        public JournalType? JournalType { get; set; }

        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
