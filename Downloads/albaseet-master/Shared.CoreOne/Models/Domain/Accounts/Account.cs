using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.CostCenters;

namespace Shared.CoreOne.Models.Domain.Accounts
{
    public class Account : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)]
        public string? AccountCode { get; set; }

        [Column(Order = 3)]
        public int CompanyId { get; set; }
        
        [Required, Column(Order = 4)]
        [StringLength(500)]
        public string? AccountNameAr { get; set; }
        
        [Required, Column(Order = 5)]
        [StringLength(500)]
        public string? AccountNameEn { get; set; }

		[Column(Order = 6)]
		public byte AccountCategoryId { get; set; }

		[Column(Order = 7)]
		public byte? AccountTypeId { get; set; }

		[Column(Order = 8)]
        public bool IsMainAccount { get; set; }
        
        [Column(Order = 9)]
        public int? MainAccountId { get; set; }
        
        [Column(Order = 10)]
        public byte AccountLevel { get; set; }

        [Column(Order = 11)]
        public int Order { get; set; }

        [Column(Order = 12)]
        public bool IsLastLevel { get; set; }

		[Column(Order = 13)]
        public short CurrencyId { get; set; }
        
        [Column(Order = 14)]
        public bool HasCostCenter { get; set; }
        
        [Column(Order = 15)]
        public int? CostCenterId { get; set; }
        
        [Column(Order = 16)]
        public bool IsPrivate { get; set; }
        
        [Column(Order = 17)]
        public bool IsActive { get; set; }

        [Column(Order = 18)]
        [StringLength(200)]
        public string? InActiveReasons { get; set; }

		[Column(Order = 19)]
        public bool HasRemarks { get; set; }

		[Column(Order = 20)]
		[StringLength(500)]
		public string? RemarksAr { get; set; }

        [Column(Order = 21)]
		[StringLength(500)]
		public string? RemarksEn { get; set; }

		[Column(Order = 22)]
        public bool IsNonEditable { get; set; }
        
        [Column(Order = 23)]
        public bool IsNonDeletable { get; set; }

        [Column(Order = 24)]
        public bool IsCreatedAutomatically { get; set; }

		[Column(Order = 25)]
        [StringLength(500)]
        public string? NotesAr { get; set; }

        [Column(Order = 26)]
        [StringLength(500)]
        public string? NotesEn { get; set; }

        [Column(Order = 27)]
        public int? InternalReferenceAccountId { get; set; } //Like Reference On Fixed Assets(Mirroring)

		[Column(Order = 28)]
        public int? ArchiveHeaderId { get; set; }


		[ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
       
        [ForeignKey(nameof(AccountCategoryId))]
        public AccountCategory? AccountCategory { get; set; } 
        
        [ForeignKey(nameof(AccountTypeId))]
        public AccountType? AccountType { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Currency? Currency { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter? CostCenter { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
