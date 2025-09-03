using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Menus
{
    public class MenuEncoding : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MenuEncodingId { get; set; }

        [Column(Order = 2)]
        public int StoreId { get; set; }

        [Column(Order = 3)]
        public short MenuCode { get; set; }

        [Column(Order = 4)]
		[StringLength(10)]
		public string? Prefix { get; set; }

        [Column(Order = 5)]
        [StringLength(10)]
		public string? Suffix { get; set; }



        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }
    }
}



//FixedAsset
//CashReceipt
//PaymentReceipt
//FixedAssetTransaction
//Account
//JournalHeader
//InternalTransferHeader
//InventoryInHeader
//InventoryOutHeader
//ItemCostUpdateHeader
//StockInInternalTransferHeader
//StockTakingHeader (Has Encoding But No Approval)
//ProductRequestHeader
//ProductRequestPriceHeader
//PurchaseOrderHeader
//StockInHeader
//StockInReturnHeader
//SupplierCreditNoticeHeader
//SupplierDebitNoticeHeader
//SupplierInvoiceHeader
//ClientCreditNoticeHeader
//ClientDebitNoticeHeader
//ClientInvoiceHeader
//ProductReceiveHeader
//ProductReceivePriceHeader
//SellingOrderHeader
//StockOutHeader
//StockOutReturnHeader


