using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Basics;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Store : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int StoreId { get; set; }

        [Required, StringLength(1000)]
        [Column(Order = 2)]
        public string? StoreNameAr { get; set; }

        [Required, StringLength(1000)]
        [Column(Order = 3)]
        public string? StoreNameEn { get; set; }

        [Column(Order = 4)]
        public byte StoreClassificationId { get; set; }

        [Column(Order = 5)]
        public int BranchId { get; set; }

		[Column(Order = 6)]
        public int? CountryId { get; set; }

        [Column(Order = 7)]
        public int? StateId { get; set; }

        [Column(Order = 8)]
        public int? CityId { get; set; }

        [Column(Order = 9)]
        public int? DistrictId { get; set; }

        [StringLength(20)]
        [Column(Order = 10)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        [Column(Order = 11)]
        public string? BuildingNumber { get; set; }

        [StringLength(50)]
        [Column(Order = 12)]
        public string? CommercialRegister { get; set; } //السجل التجاري

        [StringLength(500)]
        [Column(Order = 13)]
        public string? Street1 { get; set; }

        [StringLength(500)]
        [Column(Order = 14)]
        public string? Street2 { get; set; }

        [StringLength(200)]
        [Column(Order = 15)]
        public string? AdditionalNumber { get; set; }

        [StringLength(200)]
        [Column(Order = 16)]
        public string? CountryCode { get; set; }
        
        [StringLength(2000)]
        [Column(Order = 17)]
        public string? Address1 { get; set; }

        [StringLength(2000)]
        [Column(Order = 18)]
        public string? Address2 { get; set; }

        [StringLength(2000)]
        [Column(Order = 19)]
        public string? Address3 { get; set; }

        [StringLength(2000)]
        [Column(Order = 20)]
        public string? Address4 { get; set; }

        [Column(Order = 21)]
        public int? StockDebitAccountId { get; set; } //Dr

        [Column(Order = 22)]
        public int? StockCreditAccountId { get; set; } //Cr

        [Column(Order = 23)]
        public int? ExpenseAccountId { get; set; } //Manufacturing

        [StringLength(300)]
        [Column(Order = 24)]
        public string? Long { get; set; }

        [StringLength(300)]
        [Column(Order = 25)]
        public string? Lat { get; set; }

        [StringLength(500)]
        [Column(Order = 26)]
        public string? GMap { get; set; }

        [Column(Order = 27)]
        public bool NoReplenishment { get; set; }

        [Column(Order = 28)]
        public bool IsActive { get; set; }

        [Column(Order = 29)]
        public bool IsReservedStore { get; set; }

        [Column(Order = 30)]
		public int? ReservedParentStoreId { get; set; }

		[StringLength(500)]
        [Column(Order = 31)]
        public string? InActiveReasons { get; set; }

		[Column(Order = 32)]
        public bool CanAcceptDirectInternalTransfer { get; set; }

        

		[ForeignKey(nameof(StoreClassificationId))]
        public StoreClassification? StoreClassification { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }

        [ForeignKey(nameof(StateId))]
        public State? State { get; set; }

        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }

        [ForeignKey(nameof(DistrictId))]
        public District? District { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }    
    }
}
