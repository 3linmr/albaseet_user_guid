using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class Item : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemId { get; set; }

        [StringLength(50)]
        [Column(Order = 2)]
        public string? ItemCode { get; set; }

        [Required, StringLength(2000)]
        [Column(Order = 3)]
        public string? ItemNameAr { get; set; }

        [Required, StringLength(2000)]
        [Column(Order = 4)]
        public string? ItemNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }

        [Column(Order = 6)]
        public int? ItemCategoryId { get; set; }

        [Column(Order = 7)]
        public int? ItemSubCategoryId { get; set; }

        [Column(Order = 8)]
        public int? ItemSectionId { get; set; }

        [Column(Order = 9)]
        public int? ItemSubSectionId { get; set; }

        [Column(Order = 10)]
        public int? MainItemId { get; set; }

        [Column(Order = 11)]
        public int? VendorId { get; set; }

        [Column(Order = 12)]
        public byte ItemTypeId { get; set; }

        [Column(Order = 13)]
        public byte TaxTypeId { get; set; }

		[Column(Order = 14)]
        public int SingularPackageId { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 15)]
        public decimal PurchasingPrice { get; set; }  //سعر الشراء

		[Column(TypeName = "decimal(30,15)", Order = 16)]
        public decimal ConsumerPrice { get; set; }  //سعر المستهلك لوحدة المفرد

		[Column(TypeName = "decimal(30,15)", Order = 17)]
        public decimal InternalPrice { get; set; }  //السعر الداخلي - Transfer Price

        [Column(TypeName = "decimal(4,2)", Order = 18)]
        public decimal MaxDiscountPercent { get; set; }

        [Column(Order = 19)]
        public int? SalesAccountId { get; set; }  //Sales

        [Column(Order = 20)]
        public int? PurchaseAccountId { get; set; }  //Purchases

        [Column(TypeName = "decimal(30,15)", Order = 21)]
        public decimal MinBuyQuantity { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 22)]
        public decimal MinSellQuantity { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 23)]
        public decimal MaxBuyQuantity { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 24)]
        public decimal MaxSellQuantity { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 25)]
        public decimal ReorderPointQuantity { get; set; } //حد الأمان

        [Column(TypeName = "decimal(30,15)", Order = 26)]
        public decimal CoverageQuantity { get; set; } // تغطية المخزون

        [Column(Order = 27)]
        public bool IsActive { get; set; }

        [Column(Order = 28)]
        [StringLength(200)]
        public string? InActiveReasons { get; set; }

        [Column(Order = 29)]
        public bool NoReplenishment { get; set; }

        [Column(Order = 30)]
        public bool IsUnderSelling { get; set; }

        [Column(Order = 31)]
        public bool IsNoStock { get; set; }

        [Column(Order = 32)]
        public bool IsUntradeable { get; set; }

        [Column(Order = 33)]
        public bool IsDeficit { get; set; }

        [Column(Order = 34)]
        public bool IsPos { get; set; }

        [Column(Order = 35)]
        public bool IsOnline { get; set; }

        [Column(Order = 36)]
        public bool IsPoints { get; set; }

        [Column(Order = 37)]
        public bool IsGifts { get; set; }

        [Column(Order = 38)]
        public bool IsPromoted { get; set; }

        [Column(Order = 39)]
        public bool IsExpired { get; set; }

        [Column(Order = 40)]
        public bool IsBatched { get; set; }

        [Column(Order = 41)]
        [StringLength(300)]
        public string? ItemLocation { get; set; }

        [Column(Order = 42)]
        public int? ArchiveHeaderId { get; set; }


		[ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }


        [ForeignKey(nameof(ItemCategoryId))]
        public ItemCategory? ItemCategory { get; set; }

        [ForeignKey(nameof(ItemSubCategoryId))]
        public ItemSubCategory? ItemSubCategory { get; set; }


        [ForeignKey(nameof(ItemSectionId))]
        public ItemSection? ItemSection { get; set; }

        [ForeignKey(nameof(ItemSubSectionId))]
        public ItemSubSection? ItemSubSection { get; set; }

        [ForeignKey(nameof(MainItemId))]
        public MainItem? MainItem { get; set; }

        [ForeignKey(nameof(VendorId))]
        public Vendor? Vendor { get; set; }

        [ForeignKey(nameof(ItemTypeId))]
        public ItemType? ItemType { get; set; }

        [ForeignKey(nameof(TaxTypeId))]
        public TaxType? TaxType { get; set; }

		[ForeignKey(nameof(SingularPackageId))]
        public ItemPackage? SingularPackage { get; set; }

		[ForeignKey(nameof(PurchaseAccountId))]
        public Account? PurchaseAccount { get; set; }

        [ForeignKey(nameof(SalesAccountId))]
        public Account? SalesAccount { get; set; }

		[ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
