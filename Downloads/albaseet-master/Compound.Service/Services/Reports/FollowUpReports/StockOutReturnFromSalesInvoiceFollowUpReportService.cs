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
	public class StockOutReturnFromSalesInvoiceFollowUpReportService : IStockOutReturnFromSalesInvoiceFollowUpReportService
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
		private readonly IInvoiceStockOutReturnService _invoiceStockOutReturnService;

		public StockOutReturnFromSalesInvoiceFollowUpReportService(IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IMenuService menuService, IClientService clientService, IDocumentStatusService documentStatusService, IInvoiceStockOutReturnService invoiceStockOutReturnService)
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
			_invoiceStockOutReturnService = invoiceStockOutReturnService;
		}

		public IQueryable<StockOutReturnFromSalesInvoiceFollowUpReportDto> GetStockOutReturnFromSalesInvoiceFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var query = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == stockOutReturnHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockOutReturnHeader.MenuCode)
						from client in _clientService.GetAll().Where(x => x.ClientId == stockOutReturnHeader.ClientId)
						from invoiceStockOutReturn in _invoiceStockOutReturnService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId).DefaultIfEmpty()
						from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == invoiceStockOutReturn.SalesInvoiceReturnHeaderId).DefaultIfEmpty()
						from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutReturnHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == stockOutReturnHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						where stockOutReturnHeader.SalesInvoiceHeaderId != null && (salesInvoiceReturnHeader == null || !salesInvoiceReturnHeader.IsDirectInvoice)
						group new { stockOutReturnHeader, salesInvoiceReturnHeader, clientDebitMemo, clientCreditMemo, store, menu }
						by new { stockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnHeader.ClientId, client.ClientNameAr, client.ClientNameEn, menu.MenuCode, menu.MenuNameAr, menu.MenuNameEn, stockOutReturnHeader.Prefix, stockOutReturnHeader.DocumentCode, stockOutReturnHeader.Suffix, stockOutReturnHeader.DocumentDate, stockOutReturnHeader.NetValue, stockOutReturnHeader.Reference, stockOutReturnHeader.RemarksAr, stockOutReturnHeader.RemarksEn, store.StoreId, store.StoreNameAr, store.StoreNameEn, stockOutReturnHeader.CreatedAt, stockOutReturnHeader.UserNameCreated, stockOutReturnHeader.ModifiedAt, stockOutReturnHeader.UserNameModified }
						into g
						orderby g.Key.DocumentDate, g.Key.StockOutReturnHeaderId
						select new StockOutReturnFromSalesInvoiceFollowUpReportDto
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