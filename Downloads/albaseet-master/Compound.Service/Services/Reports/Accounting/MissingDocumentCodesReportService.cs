using Accounting.CoreOne.Contracts;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Inventory.CoreOne.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Compound.Service.Services.Reports.Accounting
{
	public class MissingDocumentCodesReportService : IMissingDocumentCodesReportService
	{
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IJournalHeaderService _journalHeaderService;
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly IInventoryInHeaderService _inventoryInHeaderService;
		private readonly IInventoryOutHeaderService _inventoryOutHeaderService;
		private readonly IInternalTransferHeaderService _internalTransferHeaderService;
		private readonly IInternalTransferReceiveHeaderService _internalTransferReceiveHeaderService;
		private readonly IStockTakingHeaderService _stockTakingHeaderService;
		private readonly IStockTakingCarryOverHeaderService _stockTakingCarryOverHeaderService;
		private readonly IProductRequestHeaderService _productRequestHeaderService;
		private readonly IProductRequestPriceHeaderService _productRequestPriceHeaderService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
		private readonly IStockInHeaderService _stockInHeaderService;
		private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
		private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
		private readonly IClientPriceRequestHeaderService _clientPriceRequestHeaderService;
		private readonly IClientQuotationApprovalHeaderService _clientQuotationApprovalHeaderService;
		private readonly IClientQuotationHeaderService _clientQuotationHeaderService;
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;
		private readonly IMenuService _menuService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IStoreService _storeService;
		private readonly IStringLocalizer<MissingDocumentCodesReportService> _localizer;

		public MissingDocumentCodesReportService( ISalesInvoiceHeaderService salesInvoiceHeaderService, IJournalHeaderService journalHeaderService, IReceiptVoucherHeaderService receiptVoucherHeaderService, IPaymentVoucherHeaderService paymentVoucherHeaderService, IInventoryInHeaderService inventoryInHeaderService, IInventoryOutHeaderService inventoryOutHeaderService, IInternalTransferHeaderService internalTransferHeaderService, IInternalTransferReceiveHeaderService internalTransferReceiveHeaderService, IStockTakingHeaderService stockTakingHeaderService, IStockTakingCarryOverHeaderService stockTakingCarryOverHeaderService, IProductRequestHeaderService productRequestHeaderService, IProductRequestPriceHeaderService productRequestPriceHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, ISupplierQuotationHeaderService supplierQuotationHeaderService, IClientPriceRequestHeaderService clientPriceRequestHeaderService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IClientQuotationHeaderService clientQuotationHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, IMenuService menuService, IHttpContextAccessor httpContextAccessor, IApplicationSettingService applicationSettingService, IStoreService storeService, IStringLocalizer<MissingDocumentCodesReportService> localizer)
		{
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_journalHeaderService = journalHeaderService;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_inventoryInHeaderService = inventoryInHeaderService;
			_inventoryOutHeaderService = inventoryOutHeaderService;
			_internalTransferHeaderService = internalTransferHeaderService;
			_internalTransferReceiveHeaderService = internalTransferReceiveHeaderService;
			_stockTakingHeaderService = stockTakingHeaderService;
			_stockTakingCarryOverHeaderService = stockTakingCarryOverHeaderService;
			_productRequestHeaderService = productRequestHeaderService;
			_productRequestPriceHeaderService = productRequestPriceHeaderService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseOrderHeaderService = purchaseOrderHeaderService;
			_stockInHeaderService = stockInHeaderService;
			_stockInReturnHeaderService = stockInReturnHeaderService;
			_supplierQuotationHeaderService = supplierQuotationHeaderService;
			_clientPriceRequestHeaderService = clientPriceRequestHeaderService;
			_clientQuotationApprovalHeaderService = clientQuotationApprovalHeaderService;
			_clientQuotationHeaderService = clientQuotationHeaderService;
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_clientDebitMemoService = clientDebitMemoService;
			_clientCreditMemoService = clientCreditMemoService;
			_supplierDebitMemoService = supplierDebitMemoService;
			_supplierCreditMemoService = supplierCreditMemoService;
			_menuService = menuService;
			_httpContextAccessor = httpContextAccessor;
			_applicationSettingService = applicationSettingService;
			_storeService = storeService;
			_localizer = localizer;
		}

		public async Task<IEnumerable<MissingDocumentCodesReportDto>> GetMissingDocumentCodesReport(List<int> storeIds, List<int> menuCodes)
		{
			var storeIdsWithoutSeparateYears = new List<int> { };
			foreach (var storeId in storeIds)
			{
				var separateYears = await _applicationSettingService.SeparateYears(storeId);
				if (!separateYears)
				{
					storeIdsWithoutSeparateYears.Add(storeId);
				}
			}

			var allHeadersWithDocumentCode = GetAllDocumentHeaders().Where(x => storeIds.Contains(x.StoreId) && (menuCodes.Count == 0 || menuCodes.Contains(x.MenuCode)));

			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			//this query will return the missing document codes in exclusive ranges,
			//for example a range of 0 to 3 means that the document codes 1 and 2 are missing
			var missingDocumentCodeRanges = await (from document1 in allHeadersWithDocumentCode
												   from menu in _menuService.GetAll().Where(x => x.MenuCode == document1.MenuCode)
												   from store in _storeService.GetAll().Where(x => x.StoreId == document1.StoreId)
												   from document2 in allHeadersWithDocumentCode.Where(x => x.DocumentCode < document1.DocumentCode && x.StoreId == document1.StoreId && x.Prefix == document1.Prefix && x.Suffix == document1.Suffix && x.MenuCode == document1.MenuCode && (storeIdsWithoutSeparateYears.Contains(x.StoreId) || x.Year == document1.Year)).OrderByDescending(x => x.DocumentCode).Take(1).DefaultIfEmpty()
												   from documentMin in allHeadersWithDocumentCode.Where(x => x.StoreId == document1.StoreId && x.Prefix == document1.Prefix && x.Suffix == document1.Suffix && x.MenuCode == document1.MenuCode && (storeIdsWithoutSeparateYears.Contains(x.StoreId) || x.Year == document1.Year)).OrderBy(x => x.DocumentCode).Take(1).DefaultIfEmpty()
												   from documentMax in allHeadersWithDocumentCode.Where(x => x.StoreId == document1.StoreId && x.Prefix == document1.Prefix && x.Suffix == document1.Suffix && x.MenuCode == document1.MenuCode && (storeIdsWithoutSeparateYears.Contains(x.StoreId) || x.Year == document1.Year)).OrderByDescending(x => x.DocumentCode).Take(1).DefaultIfEmpty()
												   where (document2.DocumentCode == null || document2.DocumentCode != document1.DocumentCode - 1) && document1.DocumentCode > 1
												   orderby document1.StoreId, document1.MenuCode, (storeIdsWithoutSeparateYears.Contains(document1.StoreId) ? (int?)null :document1.Year), document1.Prefix, document1.Suffix, document1.DocumentCode
												   select new
												   {
													   StoreId = document1.StoreId,
													   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
													   MenuCode = document1.MenuCode,
													   MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
													   Year = (storeIdsWithoutSeparateYears.Contains(document1.StoreId) ? (int?)null :document1.Year), //if separate year is not used, no need to show the year
													   Prefix = document1.Prefix,
													   Suffix = document1.Suffix,
													   DocumentCodeRangeStart = (int?)document2.DocumentCode ?? 0,
													   DocumentCodeRangeEnd = document1.DocumentCode,
													   Min = documentMin.DocumentCode,
													   Max = documentMax.DocumentCode,
												   }).ToListAsync();

			var individualMissingDocumentCodes = missingDocumentCodeRanges.SelectMany(x =>
				Enumerable.Range(x.DocumentCodeRangeStart + 1, x.DocumentCodeRangeEnd - x.DocumentCodeRangeStart - 1).Select(y => new MissingDocumentCodesReportDto
					{
						StoreId = x.StoreId,
						StoreName = x.StoreName,
						MenuCode = x.MenuCode,
						MenuName = x.MenuName,
						Year = x.Year,
						Prefix = x.Prefix,
						DocumentCode = y,
						Suffix = x.Suffix,
						DocumentFullCode = x.Prefix + y + x.Suffix,
						Min = x.Min,
						Max = x.Max,
						Count = 1,
					}
				)
			);

			var addCountRowsAndSerial = individualMissingDocumentCodes.GroupBy(x => new { x.StoreId, x.StoreName, x.MenuCode, x.MenuName, x.Year, x.Prefix, x.Suffix, x.Min, x.Max })
				.SelectMany(group => group.Select(x => new MissingDocumentCodesReportDto
				{
					StoreId = x.StoreId,
					StoreName = x.StoreName,
					MenuCode = x.MenuCode,
					MenuName = x.MenuName,
					Year = x.Year,
					Prefix = x.Prefix,
					DocumentCode = x.DocumentCode,
					Suffix = x.Suffix,
					DocumentFullCode = x.DocumentFullCode,
					Min = x.Min,
					Max = x.Max,
					Count = 1,
				}).Append(new MissingDocumentCodesReportDto
				{
					StoreId = null,
					StoreName = null,
					MenuCode = null,
					MenuName = _localizer["Count"],
					Year = null,
					Prefix = null,
					DocumentCode = null,
					Suffix = null,
					DocumentFullCode = group.Count().ToString(),
					Min = null,
					Max = null,
					Count = null,
				}
				)).Select((x, index) => new MissingDocumentCodesReportDto
				{
					Serial = index + 1,
					StoreId = x.StoreId,
					StoreName = x.StoreName,
					MenuCode = x.MenuCode,
					MenuName = x.MenuName,
					Year = x.Year,
					Prefix = x.Prefix,
					DocumentCode = x.DocumentCode,
					Suffix = x.Suffix,
					DocumentFullCode = x.DocumentFullCode,
					Min = x.Min,
					Max = x.Max,
					Count = x.Count
				});

			return addCountRowsAndSerial;
		}

		private IQueryable<AllDocumentCodesDto> GetAllDocumentHeaders()
		{
			var journalHeaders = _journalHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.JournalEntry,
					StoreId = x.StoreId,
					Year = x.TicketDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.JournalCode
				});

			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.MenuCode ?? 0,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var receiptVoucherHeaders = _receiptVoucherHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ReceiptVoucher,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.ReceiptVoucherCode,
				});

			var paymentVoucherHeaders = _paymentVoucherHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.PaymentVoucher,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.PaymentVoucherCode
				});

			var inventoryInHeaders = _inventoryInHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.InventoryIn,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.InventoryInCode
				});

			var inventoryOutHeaders = _inventoryOutHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.InventoryOut,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.InventoryOutCode
				});

			var internalTransferHeaders = _internalTransferHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.InternalTransferItems,
					StoreId = x.FromStoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.InternalTransferCode,
				});

			var internalTransferReceiveHeaders = _internalTransferReceiveHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.InternalReceiveItems,
					StoreId = x.ToStoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.InternalTransferReceiveCode
				});

			var stockTakingHeaders = _stockTakingHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.IsOpenBalance? (short)MenuCodeData.StockTakingOpenBalance : (short)MenuCodeData.StockTakingCurrentBalance,
					StoreId = x.StoreId,
					Year = x.StockDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.StockTakingCode
				});

			var stockTakingCarryOverHeaders = _stockTakingCarryOverHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.IsOpenBalance? (short)MenuCodeData.ApprovalStockTakingAsOpenBalance : (short)MenuCodeData.ApprovalStockTakingAsCurrentBalance,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.StockTakingCarryOverCode
				});

			var productRequestHeaders = _productRequestHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ProductRequest,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var productRequestPriceHeaders = _productRequestPriceHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ProductRequestPrice,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.MenuCode ?? 0,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var purchaseInvoiceReturnHeaders = _purchaseInvoiceReturnHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.MenuCode ?? 0,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var purchaseOrderHeaders = _purchaseOrderHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.PurchaseOrder,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var stockInHeaders = _stockInHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.MenuCode ?? 0,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var stockInReturnHeaders = _stockInReturnHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.MenuCode ?? 0,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var supplierQuotationHeaders = _supplierQuotationHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.SupplierQuotation,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var clientPriceRequestHeaders = _clientPriceRequestHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ClientPriceRequest,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var clientQuotationApprovalHeaders = _clientQuotationApprovalHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ClientQuotationApproval,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var clientQuotationHeaders = _clientQuotationHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ClientQuotation,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var proformaInvoiceHeaders = _proformaInvoiceHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.ProformaInvoice,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var salesInvoiceReturnHeaders = _salesInvoiceReturnHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = MenuCodeData.SalesInvoiceReturn,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var stockOutHeaders = _stockOutHeaderService.GetAll().Select(x =>
				new AllDocumentCodesDto
				{
					MenuCode = x.MenuCode ?? 0,
					StoreId = x.StoreId,
					Year = x.DocumentDate.Year,
					Prefix = x.Prefix,
					Suffix = x.Suffix,
					DocumentCode = x.DocumentCode
				});

			var stockOutReturnHeaders = _stockOutReturnHeaderService.GetAll().Select(x =>
			new AllDocumentCodesDto
			{
				MenuCode = x.MenuCode ?? 0,
				StoreId = x.StoreId,
				Year = x.DocumentDate.Year,
				Prefix = x.Prefix,
				Suffix = x.Suffix,
				DocumentCode = x.DocumentCode
			});

			var supplierCreditMemos = _supplierCreditMemoService.GetAll().Select(x =>
			new AllDocumentCodesDto
			{
				MenuCode = MenuCodeData.SupplierCreditMemo,
				StoreId = x.StoreId,
				Year = x.DocumentDate.Year,
				Prefix = x.Prefix,
				Suffix = x.Suffix,
				DocumentCode = x.DocumentCode
			});

			var supplierDebitMemos = _supplierDebitMemoService.GetAll().Select(x =>
			new AllDocumentCodesDto
			{
				MenuCode = MenuCodeData.SupplierDebitMemo,
				StoreId = x.StoreId,
				Year = x.DocumentDate.Year,
				Prefix = x.Prefix,
				Suffix = x.Suffix,
				DocumentCode = x.DocumentCode
			});

			var clientDebitMemos = _clientDebitMemoService.GetAll().Select(x =>
			new AllDocumentCodesDto
			{
				MenuCode = MenuCodeData.ClientDebitMemo,
				StoreId = x.StoreId,
				Year = x.DocumentDate.Year,
				Prefix = x.Prefix,
				Suffix = x.Suffix,
				DocumentCode = x.DocumentCode
			});

			var clientCreditMemos = _clientCreditMemoService.GetAll().Select(x =>
			new AllDocumentCodesDto
			{
				MenuCode = MenuCodeData.ClientCreditMemo,
				StoreId = x.StoreId,
				Year = x.DocumentDate.Year,
				Prefix = x.Prefix,
				Suffix = x.Suffix,
				DocumentCode = x.DocumentCode
			});

			return journalHeaders
				.Concat(salesInvoiceHeaders)
				.Concat(receiptVoucherHeaders)
				.Concat(paymentVoucherHeaders)
				.Concat(inventoryInHeaders)
				.Concat(inventoryOutHeaders)
				.Concat(internalTransferHeaders)
				.Concat(internalTransferReceiveHeaders)
				.Concat(stockTakingHeaders)
				.Concat(stockTakingCarryOverHeaders)
				.Concat(productRequestHeaders)
				.Concat(productRequestPriceHeaders)
				.Concat(purchaseInvoiceHeaders)
				.Concat(purchaseInvoiceReturnHeaders)
				.Concat(purchaseOrderHeaders)
				.Concat(stockInHeaders)
				.Concat(stockInReturnHeaders)
				.Concat(supplierQuotationHeaders)
				.Concat(clientPriceRequestHeaders)
				.Concat(clientQuotationApprovalHeaders)
				.Concat(clientQuotationHeaders)
				.Concat(proformaInvoiceHeaders)
				.Concat(salesInvoiceReturnHeaders)
				.Concat(stockOutHeaders)
				.Concat(stockOutReturnHeaders)
				.Concat(supplierDebitMemos)
				.Concat(supplierCreditMemos)
				.Concat(clientDebitMemos)
				.Concat(clientCreditMemos);
		}
	}
}
