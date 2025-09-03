using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Supplier : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SupplierId { get; set; }

        [Column(Order = 2)]
        public int SupplierCode { get; set; }

        [Required, StringLength(200)]
        [Column(Order = 3)]
        public string? SupplierNameAr { get; set; }

        [Required, StringLength(200)]
        [Column(Order = 4)]
        public string? SupplierNameEn { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 5)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ContractDate { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }

        [Column(Order = 7)]
        public int? AccountId { get; set; }

		[Column(Order = 8)]
        public int CreditLimitDays { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 9)]
        public decimal CreditLimitValues { get; set; }

        [Column(Order = 10)]
        public int DebitLimitDays { get; set; }

		[StringLength(50)]
        [Column(Order = 11)]
        public string? TaxCode { get; set; } //الرقم الضريبي

        [Column(Order = 12)]
        public int? ShipmentTypeId { get; set; }

        [Column(Order = 13)]
        public int? CountryId { get; set; }

        [Column(Order = 14)]
        public int? StateId { get; set; }

        [Column(Order = 15)]
        public int? CityId { get; set; }

        [Column(Order = 16)]
        public int? DistrictId { get; set; }

        [StringLength(20)]
        [Column(Order = 17)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        [Column(Order = 18)]
        public string? BuildingNumber { get; set; }

        [StringLength(50)]
        [Column(Order = 19)]
        public string? CommercialRegister { get; set; } //السجل التجاري

        [StringLength(200)]
        [Column(Order = 20)]
        public string? Street1 { get; set; }

        [StringLength(200)]
        [Column(Order = 21)]
        public string? Street2 { get; set; }

        [StringLength(200)]
        [Column(Order = 22)]
        public string? AdditionalNumber { get; set; }

        [StringLength(200)]
        [Column(Order = 23)]
        public string? CountryCode { get; set; }

        [StringLength(200)]
        [Column(Order = 24)]
        public string? Address1 { get; set; }

        [StringLength(200)]
        [Column(Order = 25)]
        public string? Address2 { get; set; }

        [StringLength(200)]
        [Column(Order = 26)]
        public string? Address3 { get; set; }

        [StringLength(200)]
        [Column(Order = 27)]
        public string? Address4 { get; set; }

        [StringLength(100)]
        [Column(Order = 28)]
        public string? FirstResponsibleName { get; set; }

        [StringLength(100)]
        [Column(Order = 29)]
        public string? FirstResponsiblePhone { get; set; }

        [StringLength(100)]
        [Column(Order = 30)]
        public string? FirstResponsibleEmail { get; set; }

        [StringLength(100)]
        [Column(Order = 31)]
        public string? SecondResponsibleName { get; set; }

        [StringLength(100)]
        [Column(Order = 32)]
        public string? SecondResponsiblePhone { get; set; }

        [StringLength(100)]
        [Column(Order = 33)]
        public string? SecondResponsibleEmail { get; set; }

		[Column(Order = 34)]
        public bool IsCredit { get; set; }   //تعامل آجل

        [Column(Order = 35)]
        public bool IsActive { get; set; }

        [StringLength(500)]
        [Column(Order = 36)]
        public string? InActiveReasons { get; set; }

		[Column(Order = 37)]
        public int? ArchiveHeaderId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

		[ForeignKey(nameof(ShipmentTypeId))]
        public ShipmentType? ShipmentType { get; set; }
        
        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
