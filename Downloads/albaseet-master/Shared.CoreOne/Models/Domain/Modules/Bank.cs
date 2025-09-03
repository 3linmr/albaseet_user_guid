using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Archive;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Bank : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BankId { get; set; }

        [Column(Order = 2)]
        public int BankCode { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? BankNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? BankNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

        [StringLength(100)]
        [Column(Order = 6)]
        public string? AccountNumber { get; set; }

        [StringLength(100)]
        [Column(Order = 7)]
        public string? IBAN { get; set; }

        [StringLength(100)]
        [Column(Order = 8)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [Column(Order = 9)]
        public string? Fax { get; set; }

        [StringLength(100)]
        [Column(Order = 10)]
        public string? Website { get; set; }

        [StringLength(100)]
        [Column(Order = 11)]
        public string? Email { get; set; }

        [StringLength(300)]
        [Column(Order = 12)]
        public string? Address { get; set; }

        [Column(Order = 13)]
        [StringLength(50)]
        public string? TaxCode { get; set; }

		[StringLength(100)]
        [Column(Order = 14)]
        public string? ResponsibleName { get; set; }

        [StringLength(100)]
        [Column(Order = 15)]
        public string? ResponsiblePhone { get; set; }

        [StringLength(100)]
        [Column(Order = 16)]
        public string? ResponsibleFax { get; set; }

        [StringLength(100)]
        [Column(Order = 17)]
        public string? ResponsibleEmail { get; set; }

        [Column(Order = 18)]
        public int? AccountId { get; set; }

		[Column(TypeName = "decimal(4,2)",Order = 19)]
        public decimal VisaFees { get; set; }

        [Column(Order = 20)]
        public bool IsActive { get; set; }

        [StringLength(500)]
        [Column(Order = 21)]
        public string? InActiveReasons { get; set; }

        [Column(Order = 22)]
        public int? ArchiveHeaderId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

		[ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
	}
}
