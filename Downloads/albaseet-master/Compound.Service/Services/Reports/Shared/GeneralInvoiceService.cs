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
	public class GeneralInvoiceService : IGeneralInvoiceService
	{
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
        private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly IInventoryInHeaderService _inventoryInHeaderService;
		private readonly IInventoryOutHeaderService _inventoryOutHeaderService;
		private readonly IFixedAssetVoucherHeaderService _fixedAssetVoucherHeaderService;

        public GeneralInvoiceService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService, IReceiptVoucherHeaderService receiptVoucherHeaderService, IPaymentVoucherHeaderService paymentVoucherHeaderService, IInventoryInHeaderService inventoryInHeaderService, IInventoryOutHeaderService inventoryOutHeaderService, IFixedAssetVoucherHeaderService fixedAssetVoucherHeaderService)
        {
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _clientDebitMemoService = clientDebitMemoService;
            _clientCreditMemoService = clientCreditMemoService;
            _receiptVoucherHeaderService = receiptVoucherHeaderService;
            _paymentVoucherHeaderService = paymentVoucherHeaderService;
			_inventoryInHeaderService = inventoryInHeaderService;
			_inventoryOutHeaderService = inventoryOutHeaderService;
			_fixedAssetVoucherHeaderService = fixedAssetVoucherHeaderService;
        }

        public IQueryable<GeneralInvoiceDto> GetGeneralInvoices()
        {
            var inventoryIns = _inventoryInHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.InventoryInHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = null,
                MenuCode = MenuCodeData.InventoryIn,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.InventoryInCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.InventoryInCode + x.Suffix,
                Reference = x.Reference,
                InvoiceTypeId = null,
                TaxTypeId = null,
                NetValue = -x.TotalConsumerValue,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });


            var inventoryOuts = _inventoryOutHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.InventoryOutHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = null,
                MenuCode = MenuCodeData.InventoryOut,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.InventoryOutCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.InventoryOutCode + x.Suffix,
                Reference = x.Reference,
                InvoiceTypeId = null,
                TaxTypeId = null,
                NetValue = x.TotalConsumerValue,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });

            var purchaseInvoices = _purchaseInvoiceHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.PurchaseInvoiceHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = x.JournalHeaderId,
                MenuCode = x.MenuCode,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.DocumentCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.DocumentCode + x.Suffix,
                Reference = x.Reference,
                InvoiceTypeId = x.InvoiceTypeId,
                TaxTypeId = x.TaxTypeId,
                NetValue = -x.NetValue,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });

            var purchaseInvoiceReturns = from purchaseInvoiceReturn in _purchaseInvoiceReturnHeaderService.GetAll()
                                         from purchaseInvoice in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturn.PurchaseInvoiceHeaderId)
                                         select new GeneralInvoiceDto
                                         {
                                             InvoiceId = purchaseInvoiceReturn.PurchaseInvoiceReturnHeaderId,
                                             StoreId = purchaseInvoiceReturn.StoreId,
                                             JournalHeaderId = purchaseInvoiceReturn.JournalHeaderId,
											 MenuCode = purchaseInvoiceReturn.MenuCode,
                                             DocumentDate = purchaseInvoiceReturn.DocumentDate,
                                             EntryDate = purchaseInvoiceReturn.EntryDate,
                                             InvoicePrefix = purchaseInvoiceReturn.Prefix,
                                             InvoiceCode = purchaseInvoiceReturn.DocumentCode,
                                             InvoiceSuffix = purchaseInvoiceReturn.Suffix,
                                             FullInvoiceCode = purchaseInvoiceReturn.Prefix + purchaseInvoiceReturn.DocumentCode + purchaseInvoiceReturn.Suffix,
                                             Reference = purchaseInvoiceReturn.Reference,
                                             InvoiceTypeId = purchaseInvoice.InvoiceTypeId,
                                             TaxTypeId = purchaseInvoiceReturn.TaxTypeId,
                                             NetValue = -purchaseInvoiceReturn.NetValue,
                                             RemarksAr = purchaseInvoiceReturn.RemarksAr,
                                             RemarksEn = purchaseInvoiceReturn.RemarksEn
                                         };

            var supplierCreditMemos = from supplierCreditMemo in _supplierCreditMemoService.GetAll()
                                      from purchaseInvoice in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
                                      select new GeneralInvoiceDto
                                      {
                                          InvoiceId = supplierCreditMemo.PurchaseInvoiceHeaderId,
                                          StoreId = supplierCreditMemo.StoreId,
                                          JournalHeaderId = supplierCreditMemo.JournalHeaderId,
										  MenuCode = MenuCodeData.SupplierCreditMemo,
                                          DocumentDate = supplierCreditMemo.DocumentDate,
                                          EntryDate = supplierCreditMemo.EntryDate,
                                          InvoicePrefix = supplierCreditMemo.Prefix,
                                          InvoiceCode = supplierCreditMemo.DocumentCode,
                                          InvoiceSuffix = supplierCreditMemo.Suffix,
                                          FullInvoiceCode = supplierCreditMemo.Prefix + supplierCreditMemo.DocumentCode + supplierCreditMemo.Suffix,
                                          Reference = supplierCreditMemo.Reference,
                                          InvoiceTypeId = purchaseInvoice.InvoiceTypeId,
                                          TaxTypeId = purchaseInvoice.TaxTypeId,
                                          NetValue = -supplierCreditMemo.MemoValue,
                                          RemarksAr = supplierCreditMemo.RemarksAr,
                                          RemarksEn = supplierCreditMemo.RemarksEn
                                      };

            var supplierDebitMemos = from supplierDebitMemo in _supplierDebitMemoService.GetAll()
                                     from purchaseInvoice in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
                                     select new GeneralInvoiceDto
                                     {
                                         InvoiceId = supplierDebitMemo.PurchaseInvoiceHeaderId,
                                         StoreId = supplierDebitMemo.StoreId,
                                         JournalHeaderId = supplierDebitMemo.JournalHeaderId,
										 MenuCode = MenuCodeData.SupplierDebitMemo,
                                         DocumentDate = supplierDebitMemo.DocumentDate,
                                         EntryDate = supplierDebitMemo.EntryDate,
                                         InvoicePrefix = supplierDebitMemo.Prefix,
                                         InvoiceCode = supplierDebitMemo.DocumentCode,
                                         InvoiceSuffix = supplierDebitMemo.Suffix,
                                         FullInvoiceCode = supplierDebitMemo.Prefix + supplierDebitMemo.DocumentCode + supplierDebitMemo.Suffix,
                                         Reference = supplierDebitMemo.Reference,
                                         InvoiceTypeId = purchaseInvoice.InvoiceTypeId,
                                         TaxTypeId = purchaseInvoice.TaxTypeId,
                                         NetValue = supplierDebitMemo.MemoValue,
                                         RemarksAr = supplierDebitMemo.RemarksAr,
                                         RemarksEn = supplierDebitMemo.RemarksEn
                                     };

            var salesInvoices = _salesInvoiceHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.SalesInvoiceHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = x.JournalHeaderId,
                MenuCode = x.MenuCode,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.DocumentCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.DocumentCode + x.Suffix,
                Reference = x.Reference,
                InvoiceTypeId = x.InvoiceTypeId,
                TaxTypeId = x.TaxTypeId,
                NetValue = x.NetValue,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });

            var salesInvoiceReturns = from salesInvoiceReturn in _salesInvoiceReturnHeaderService.GetAll()
                                      from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceReturn.SalesInvoiceHeaderId)
                                      select new GeneralInvoiceDto
                                      {
                                          InvoiceId = salesInvoiceReturn.SalesInvoiceReturnHeaderId,
                                          StoreId = salesInvoiceReturn.StoreId,
                                          JournalHeaderId = salesInvoiceReturn.JournalHeaderId,
                                          MenuCode = salesInvoiceReturn.MenuCode,
                                          DocumentDate = salesInvoiceReturn.DocumentDate,
                                          EntryDate = salesInvoiceReturn.EntryDate,
                                          InvoicePrefix = salesInvoiceReturn.Prefix,
                                          InvoiceCode = salesInvoiceReturn.DocumentCode,
                                          InvoiceSuffix = salesInvoiceReturn.Suffix,
                                          FullInvoiceCode = salesInvoiceReturn.Prefix + salesInvoiceReturn.DocumentCode + salesInvoiceReturn.Suffix,
                                          Reference = salesInvoiceReturn.Reference,
                                          InvoiceTypeId = salesInvoice.InvoiceTypeId,
                                          TaxTypeId = salesInvoice.TaxTypeId,
                                          NetValue = -salesInvoiceReturn.NetValue,
                                          RemarksAr = salesInvoiceReturn.RemarksAr,
                                          RemarksEn = salesInvoiceReturn.RemarksEn
                                      };

            var clientCreditMemos = from clientCreditMemo in _clientCreditMemoService.GetAll()
                                    from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
                                    select new GeneralInvoiceDto
                                    {
                                        InvoiceId = clientCreditMemo.SalesInvoiceHeaderId,
                                        StoreId = clientCreditMemo.StoreId,
                                        JournalHeaderId = clientCreditMemo.JournalHeaderId,
										MenuCode = MenuCodeData.ClientCreditMemo,
                                        DocumentDate = clientCreditMemo.DocumentDate,
                                        EntryDate = clientCreditMemo.EntryDate,
                                        InvoicePrefix = clientCreditMemo.Prefix,
                                        InvoiceCode = clientCreditMemo.DocumentCode,
                                        InvoiceSuffix = clientCreditMemo.Suffix,
                                        FullInvoiceCode = clientCreditMemo.Prefix + clientCreditMemo.DocumentCode + clientCreditMemo.Suffix,
                                        Reference = clientCreditMemo.Reference,
                                        InvoiceTypeId = salesInvoice.InvoiceTypeId,
                                        TaxTypeId = salesInvoice.TaxTypeId,
                                        NetValue = -clientCreditMemo.MemoValue,
                                        RemarksAr = clientCreditMemo.RemarksAr,
                                        RemarksEn = clientCreditMemo.RemarksEn
                                    };

            var clientDebitMemos = from clientDebitMemo in _clientDebitMemoService.GetAll()
                                   from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
                                   select new GeneralInvoiceDto
                                   {
                                       InvoiceId = clientDebitMemo.SalesInvoiceHeaderId,
                                       StoreId = clientDebitMemo.StoreId,
                                       JournalHeaderId = clientDebitMemo.JournalHeaderId,
									   MenuCode = MenuCodeData.ClientDebitMemo,
                                       DocumentDate = clientDebitMemo.DocumentDate,
                                       EntryDate = clientDebitMemo.EntryDate,
                                       InvoicePrefix = clientDebitMemo.Prefix,
                                       InvoiceCode = clientDebitMemo.DocumentCode,
                                       InvoiceSuffix = clientDebitMemo.Suffix,
                                       FullInvoiceCode = clientDebitMemo.Prefix + clientDebitMemo.DocumentCode + clientDebitMemo.Suffix,
                                       Reference = clientDebitMemo.Reference,
                                       InvoiceTypeId = salesInvoice.InvoiceTypeId,
                                       TaxTypeId = salesInvoice.TaxTypeId,
                                       NetValue = clientDebitMemo.MemoValue,
                                       RemarksAr = clientDebitMemo.RemarksAr,
                                       RemarksEn = clientDebitMemo.RemarksEn
                                   };

            var receiptVouchers = _receiptVoucherHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.ReceiptVoucherHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = x.JournalHeaderId,
                MenuCode = MenuCodeData.ReceiptVoucher,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.ReceiptVoucherCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.ReceiptVoucherCode + x.Suffix,
                Reference = x.PeerReference,
                InvoiceTypeId = null,
                TaxTypeId = null,
                NetValue = x.TotalDebitValue,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });

            var paymentVouchers = _paymentVoucherHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.PaymentVoucherHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = x.JournalHeaderId,
                MenuCode = MenuCodeData.PaymentVoucher,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.PaymentVoucherCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.PaymentVoucherCode + x.Suffix,
                Reference = x.PeerReference,
                InvoiceTypeId = null,
                TaxTypeId = null,
                NetValue = -x.TotalCreditValue,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });

            var fixedAssetVouchers = _fixedAssetVoucherHeaderService.GetAll().Select(x => new GeneralInvoiceDto
            {
                InvoiceId = x.FixedAssetVoucherHeaderId,
                StoreId = x.StoreId,
                JournalHeaderId = x.JournalHeaderId,
                MenuCode = MenuCodeData.ReceiptVoucher,
                DocumentDate = x.DocumentDate,
                EntryDate = x.EntryDate,
                InvoicePrefix = x.Prefix,
                InvoiceCode = x.DocumentCode,
                InvoiceSuffix = x.Suffix,
                FullInvoiceCode = x.Prefix + x.DocumentCode + x.Suffix,
                Reference = x.PeerReference,
                InvoiceTypeId = null,
                TaxTypeId = null,
                NetValue = null,
                RemarksAr = x.RemarksAr,
                RemarksEn = x.RemarksEn
            });

            return inventoryIns.Concat(inventoryOuts).Concat(purchaseInvoices).Concat(purchaseInvoiceReturns).Concat(supplierCreditMemos).Concat(supplierDebitMemos).Concat(salesInvoices).Concat(salesInvoiceReturns).Concat(clientCreditMemos).Concat(clientDebitMemos).Concat(receiptVouchers).Concat(paymentVouchers).Concat(fixedAssetVouchers);
        }
	}
}
