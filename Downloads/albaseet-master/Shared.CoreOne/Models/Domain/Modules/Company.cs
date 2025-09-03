using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Company : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)] 
        public int CompanyId { get; set; }

        [Required, StringLength(1000)]
        [Column(Order = 2)]
        public string? CompanyNameAr { get; set; }

        [Required, StringLength(1000)]
        [Column(Order = 3)]
        public string? CompanyNameEn { get; set; }

        [StringLength(50)]
        [Column(Order = 4)]
        public string? TaxCode { get; set; }

        [Column(Order = 5)]
        public short CurrencyId { get; set; }

        [StringLength(50)]
        [Column(Order = 6)]
        public string? Phone { get; set; }

        [StringLength(20)]
        [Column(Order = 7)] 
        public string? WhatsApp { get; set; }

        [StringLength(100)]
        [Column(Order = 8)]
        public string? Email { get; set; }

        [StringLength(100)]
        [Column(Order = 9)]
        public string? Website { get; set; }

        [StringLength(2000)]
        [Column(Order = 10)]
        public string? Address { get; set; }

        [StringLength(500)]
        [Column(Order = 11)]
        public string? LogoUrl { get; set; }

        [StringLength(500)]
        [Column(Order = 12)]
        public string? HeaderUrl { get; set; }

        [StringLength(500)]
        [Column(Order = 13)]
        public string? FooterUrl { get; set; }
        
        [Column(Order = 14)]
        public bool IsActive { get; set; }

        [StringLength(500)]
        [Column(Order = 15)]
        public string? InActiveReasons { get; set; }


        [ForeignKey(nameof(CurrencyId))]
        public Currency? Currency { get; set; }
    }
}
