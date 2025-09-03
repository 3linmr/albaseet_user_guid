using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Seller : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerId { get; set; }

        [Column(Order = 2)]
        public int SellerCode { get; set; }
 
        [Column(Order = 3)]
        public byte SellerTypeId { get; set; }

        [Column(Order = 4)]
        [StringLength(100)]
        public string? SellerNameAr { get; set; }

        [Column(Order = 5)]
        [StringLength(100)]
        public string? SellerNameEn { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 6)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")] 
        public DateTime ContractDate { get; set; }

        [Column(Order = 7)]
        public int CompanyId { get; set; }

		[Column(Order = 8)]
        public int? EmployeeId { get; set; }

        [Column(Order = 9)]
        [StringLength(100)]
        public string? OutSourcingCompany { get; set; }

        [Column(Order = 10)]
        public short? SellerCommissionMethodId { get; set; }

        [Column(Order = 11)]
        [StringLength(50)]
        public string? Phone { get; set; }

        [Column(Order = 12)]
        [StringLength(20)]
        public string? WhatsApp { get; set; }

        [Column(Order = 13)]
        [StringLength(200)]
        public string? Address { get; set; }

        [Column(Order = 14)]
        [StringLength(100)]
        public string? Email { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 15)]
        public decimal ClientsDebitLimit { get; set; } // الحد الائتماني لمديونية عملاؤه

		[Column(Order = 16)]
        public bool IsActive { get; set; }

        [StringLength(500)]
        [Column(Order = 17)]
        public string? InActiveReasons { get; set; }

		[Column(Order = 18)]
        public int? ArchiveHeaderId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
        
        [ForeignKey(nameof(SellerTypeId))]
        public SellerType? SellerType { get; set; }

        [ForeignKey(nameof(SellerCommissionMethodId))]
        public SellerCommissionMethod? SellerCommissionMethod { get; set; }
    }
}
