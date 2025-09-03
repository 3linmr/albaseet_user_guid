using Sales.CoreOne.Contracts;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using Microsoft.Extensions.Localization;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Basics;

namespace Compound.Service.Services.Reports.FollowUpReports
{
	public class StockOutReturnFromStockOutFollowUpReportService : IStockOutReturnFromStockOutFollowUpReportService
	{
		private readonly IClientQuotationHeaderService _clientQuotationHeaderService;
		private readonly IClientQuotationApprovalHeaderService _clientQuotationApprovalHeaderService;
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ProductRequestFollowUpReportService> _localizer;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IMenuService _menuService;
		private readonly IClientService _clientService;
		private readonly IDocumentStatusService _documentStatusService;
		private readonly IInvoiceStockOutService _invoiceStockOutService;

		public StockOutReturnFromStockOutFollowUpReportService(IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IMenuService menuService, IClientService clientService, IDocumentStatusService documentStatusService, IInvoiceStockOutService invoiceStockOutService)
		{
			_clientQuotationHeaderService = clientQuotationHeaderService;
			_clientQuotationApprovalHeaderService = clientQuotationApprovalHeaderService;
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
			_stockOutHeaderService = stockOutHeaderService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_clientCreditMemoService = clientCreditMemoService;
			_clientDebitMemoService = clientDebitMemoService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_menuService = menuService;
			_clientService = clientService;
			_documentStatusService = documentStatusService;
			_invoiceStockOutService = invoiceStockOutService;
		}

		public IQueryable<StockOutReturnFromStockOutFollowUpReportDto> GetStockOutReturnFromStockOutFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var query = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == stockOutReturnHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockOutReturnHeader.MenuCode)
						from client in _clientService.GetAll().Where(x => x.ClientId == stockOutReturnHeader.ClientId)
						from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturnHeader.StockOutHeaderId).DefaultIfEmpty()
						from invoiceStockOut in _invoiceStockOutService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturnHeader.StockOutHeaderId).DefaultIfEmpty()
						from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == invoiceStockOut.SalesInvoiceHeaderId).DefaultIfEmpty()
						from stockOutReturnFromSalesInvoice in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						where stockOutHeader.ProformaInvoiceHeaderId != null && (salesInvoiceHeader == null || !salesInvoiceHeader.IsDirectInvoice)
						group new { stockOutReturnHeader, salesInvoiceHeader, stockOutReturnFromSalesInvoice, salesInvoiceReturnHeader, clientDebitMemo, clientCreditMemo, store, menu }
						by new { stockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnHeader.ClientId, client.ClientNameAr, client.ClientNameEn, menu.MenuCode, menu.MenuNameAr, menu.MenuNameEn, stockOutReturnHeader.Prefix, stockOutReturnHeader.DocumentCode, stockOutReturnHeader.Suffix, stockOutReturnHeader.DocumentDate, stockOutReturnHeader.NetValue, stockOutReturnHeader.Reference, stockOutReturnHeader.RemarksAr, stockOutReturnHeader.RemarksEn, store.StoreId, store.StoreNameAr, store.StoreNameEn, stockOutReturnHeader.CreatedAt, stockOutReturnHeader.UserNameCreated, stockOutReturnHeader.ModifiedAt, stockOutReturnHeader.UserNameModified }
						into g
						orderby g.Key.DocumentDate, g.Key.StockOutReturnHeaderId
						select new StockOutReturnFromStockOutFollowUpReportDto
						{
							StockOutReturnHeaderId = g.Key.StockOutReturnHeaderId,
							MenuCode = g.Key.MenuCode,
							MenuName = language == LanguageCode.Arabic ? g.Key.MenuNameAr : g.Key.MenuNameEn,
							DocumentFullCode = g.Key.Prefix + g.Key.DocumentCode + g.Key.Suffix,
							DocumentDate = g.Key.DocumentDate,
							Price = g.Key.NetValue,
							ClientId = g.Key.ClientId,
							ClientName = language == LanguageCode.Arabic ? g.Key.ClientNameAr : g.Key.ClientNameEn,
							StoreId = g.Key.StoreId,
							StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
							SalesInvoiceInterim = g.Count(x => x.salesInvoiceHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							ReceiptVoucher = g.Count(x => x.salesInvoiceHeader != null && x.salesInvoiceHeader.IsSettlementCompleted) > 0 ? _localizer["Done"].ToString() : null,
							StockOutReturnFromSalesInvoice = g.Count(x => x.stockOutReturnFromSalesInvoice != null) > 0 ? _localizer["Done"].ToString() : null,
							SalesInvoiceReturn = g.Count(x => x.salesInvoiceReturnHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							ClientDebitMemo = g.Count(x => x.clientDebitMemo != null) > 0 ? _localizer["Done"].ToString() : null,
							ClientCreditMemo = g.Count(x => x.clientCreditMemo != null) > 0 ? _localizer["Done"].ToString() : null,
							Reference = g.Key.Reference,
							RemarksAr = g.Key.RemarksAr,
							RemarksEn = g.Key.RemarksEn,

							CreatedAt = g.Key.CreatedAt,
							UserNameCreated = g.Key.UserNameCreated,
							ModifiedAt = g.Key.ModifiedAt,
							UserNameModified = g.Key.UserNameModified
						};

			return query;
		}
	}
}