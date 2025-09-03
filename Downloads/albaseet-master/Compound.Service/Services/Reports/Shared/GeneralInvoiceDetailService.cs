using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Shared;
using Compound.CoreOne.Contracts.Reports.Shared;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Inventory.CoreOne.Contracts;
using Accounting.CoreOne.Contracts;
using Shared.CoreOne.Models.StaticData;

namespace Compound.Service.Services.Reports.Shared
{
    public class GeneralInvoiceDetailService : IGeneralInvoiceDetailService
    {
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IInventoryInHeaderService _inventoryInHeaderService;
        private readonly IInventoryOutHeaderService _inventoryOutHeaderService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
        private readonly IInventoryInDetailService _inventoryInDetailService;
        private readonly IInventoryOutDetailService _inventoryOutDetailService;

        public GeneralInvoiceDetailService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IInventoryInHeaderService inventoryInHeaderService, IInventoryOutHeaderService inventoryOutHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IInventoryInDetailService inventoryInDetailService, IInventoryOutDetailService inventoryOutDetailService)
        {
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _inventoryInHeaderService = inventoryInHeaderService;
            _inventoryOutHeaderService = inventoryOutHeaderService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
            _inventoryInDetailService = inventoryInDetailService;
            _inventoryOutDetailService = inventoryOutDetailService;
        }

        public IQueryable<GeneralInvoiceDetailDto> GetGeneralInvoiceDetails()
        {
			var inventoryIns = from inventoryInHeader in _inventoryInHeaderService.GetAll()
							   from inventoryInDetail in _inventoryInDetailService.GetAll().Where(x => x.InventoryInHeaderId == inventoryInHeader.InventoryInHeaderId)
							   select new GeneralInvoiceDetailDto
							   {
								   InvoiceId = inventoryInHeader.InventoryInHeaderId,
								   StoreId = inventoryInHeader.StoreId,
								   JournalHeaderId = null,
								   MenuCode = MenuCodeData.InventoryIn,
								   DocumentDate = inventoryInHeader.DocumentDate,
								   EntryDate = inventoryInHeader.EntryDate,
								   InvoicePrefix = inventoryInHeader.Prefix,
								   InvoiceCode = inventoryInHeader.InventoryInCode,
								   InvoiceSuffix = inventoryInHeader.Suffix,
								   FullInvoiceCode = inventoryInHeader.Prefix + inventoryInHeader.InventoryInCode + inventoryInHeader.Suffix,
								   Reference = inventoryInHeader.Reference,
								   InvoiceTypeId = null,
								   TaxTypeId = null,
                                   InvoiceDetailId = inventoryInDetail.InventoryInDetailId,
								   ItemId = inventoryInDetail.ItemId,
                                   ItemNote = null,
                                   Quantity = inventoryInDetail.Quantity,
                                   Price = inventoryInDetail.ConsumerPrice,
							   };

            var inventoryOuts = from inventoryOutHeader in _inventoryOutHeaderService.GetAll()
                                from inventoryOutDetail in _inventoryOutDetailService.GetAll().Where(x => x.InventoryOutHeaderId == inventoryOutHeader.InventoryOutHeaderId)
                                select new GeneralInvoiceDetailDto
                                {
                                    InvoiceId = inventoryOutHeader.InventoryOutHeaderId,
                                    StoreId = inventoryOutHeader.StoreId,
                                    JournalHeaderId = null,
                                    MenuCode = MenuCodeData.InventoryOut,
                                    DocumentDate = inventoryOutHeader.DocumentDate,
                                    EntryDate = inventoryOutHeader.EntryDate,
                                    InvoicePrefix = inventoryOutHeader.Prefix,
                                    InvoiceCode = inventoryOutHeader.InventoryOutCode,
                                    InvoiceSuffix = inventoryOutHeader.Suffix,
                                    FullInvoiceCode = inventoryOutHeader.Prefix + inventoryOutHeader.InventoryOutCode + inventoryOutHeader.Suffix,
                                    Reference = inventoryOutHeader.Reference,
                                    InvoiceTypeId = null,
                                    TaxTypeId = null,
                                    InvoiceDetailId = inventoryOutDetail.InventoryOutDetailId,
									ItemId = inventoryOutDetail.ItemId,
                                    ItemNote = null,
                                    Quantity = inventoryOutDetail.Quantity,
                                    Price = inventoryOutDetail.ConsumerPrice
                                };

            var purchaseInvoices = from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll()
                                   from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                                   select new GeneralInvoiceDetailDto
                                   {
                                       InvoiceId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
                                       StoreId = purchaseInvoiceHeader.StoreId,
                                       JournalHeaderId = purchaseInvoiceHeader.JournalHeaderId,
                                       MenuCode = purchaseInvoiceHeader.MenuCode,
                                       DocumentDate = purchaseInvoiceHeader.DocumentDate,
                                       EntryDate = purchaseInvoiceHeader.EntryDate,
                                       InvoicePrefix = purchaseInvoiceHeader.Prefix,
                                       InvoiceCode = purchaseInvoiceHeader.DocumentCode,
                                       InvoiceSuffix = purchaseInvoiceHeader.Suffix,
                                       FullInvoiceCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
                                       Reference = purchaseInvoiceHeader.Reference,
                                       InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
                                       TaxTypeId = purchaseInvoiceHeader.TaxTypeId,
                                       InvoiceDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId,
									   ItemId = purchaseInvoiceDetail.ItemId,
                                       ItemNote = purchaseInvoiceDetail.ItemNote,
                                       Quantity = purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity,
                                       Price = purchaseInvoiceDetail.PurchasePrice
                                   };

            var purchaseInvoiceReturns = from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll()
                                         from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
                                         select new GeneralInvoiceDetailDto
                                         {
                                             InvoiceId = purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId,
                                             StoreId = purchaseInvoiceReturnHeader.StoreId,
                                             JournalHeaderId = purchaseInvoiceReturnHeader.JournalHeaderId,
                                             MenuCode = purchaseInvoiceReturnHeader.MenuCode,
                                             DocumentDate = purchaseInvoiceReturnHeader.DocumentDate,
                                             EntryDate = purchaseInvoiceReturnHeader.EntryDate,
                                             InvoicePrefix = purchaseInvoiceReturnHeader.Prefix,
                                             InvoiceCode = purchaseInvoiceReturnHeader.DocumentCode,
                                             InvoiceSuffix = purchaseInvoiceReturnHeader.Suffix,
                                             FullInvoiceCode = purchaseInvoiceReturnHeader.Prefix + purchaseInvoiceReturnHeader.DocumentCode + purchaseInvoiceReturnHeader.Suffix,
                                             Reference = purchaseInvoiceReturnHeader.Reference,
                                             InvoiceTypeId = null,
                                             TaxTypeId = purchaseInvoiceReturnHeader.TaxTypeId,
											 InvoiceDetailId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId,
											 ItemId = purchaseInvoiceReturnDetail.ItemId,
                                             ItemNote = purchaseInvoiceReturnDetail.ItemNote,
                                             Quantity = purchaseInvoiceReturnDetail.Quantity + purchaseInvoiceReturnDetail.BonusQuantity,
                                             Price = purchaseInvoiceReturnDetail.PurchasePrice
										 };

            var salesInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll()
                                from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
                                select new GeneralInvoiceDetailDto
                                {
                                    InvoiceId = salesInvoiceHeader.SalesInvoiceHeaderId,
                                    StoreId = salesInvoiceHeader.StoreId,
                                    JournalHeaderId = salesInvoiceHeader.JournalHeaderId,
                                    MenuCode = salesInvoiceHeader.MenuCode,
                                    DocumentDate = salesInvoiceHeader.DocumentDate,
                                    EntryDate = salesInvoiceHeader.EntryDate,
                                    InvoicePrefix = salesInvoiceHeader.Prefix,
                                    InvoiceCode = salesInvoiceHeader.DocumentCode,
                                    InvoiceSuffix = salesInvoiceHeader.Suffix,
                                    FullInvoiceCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
                                    Reference = salesInvoiceHeader.Reference,
                                    InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
                                    TaxTypeId = salesInvoiceHeader.TaxTypeId,
									InvoiceDetailId = salesInvoiceDetail.SalesInvoiceDetailId,
									ItemId = salesInvoiceDetail.ItemId,
                                    ItemNote = salesInvoiceDetail.ItemNote,
                                    Quantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
                                    Price = salesInvoiceDetail.SellingPrice
								};

            var salesInvoiceReturns = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll()
                                      from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
                                      select new GeneralInvoiceDetailDto
                                      {
                                          InvoiceId = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
                                          StoreId = salesInvoiceReturnHeader.StoreId,
                                          JournalHeaderId = salesInvoiceReturnHeader.JournalHeaderId,
                                          MenuCode = salesInvoiceReturnHeader.MenuCode,
                                          DocumentDate = salesInvoiceReturnHeader.DocumentDate,
                                          EntryDate = salesInvoiceReturnHeader.EntryDate,
                                          InvoicePrefix = salesInvoiceReturnHeader.Prefix,
                                          InvoiceCode = salesInvoiceReturnHeader.DocumentCode,
                                          InvoiceSuffix = salesInvoiceReturnHeader.Suffix,
                                          FullInvoiceCode = salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix,
                                          Reference = salesInvoiceReturnHeader.Reference,
                                          InvoiceTypeId = null,
                                          TaxTypeId = salesInvoiceReturnHeader.TaxTypeId,
                                          InvoiceDetailId = salesInvoiceReturnDetail.SalesInvoiceReturnDetailId,
										  ItemId = salesInvoiceReturnDetail.ItemId,
                                          ItemNote = salesInvoiceReturnDetail.ItemNote,
                                          Quantity = salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity,
                                          Price = salesInvoiceReturnDetail.SellingPrice
									  };

            return inventoryIns.Concat(inventoryOuts)
                               .Concat(purchaseInvoices)
                               .Concat(purchaseInvoiceReturns)
                               .Concat(salesInvoices)
                               .Concat(salesInvoiceReturns);
        }
    }
}
