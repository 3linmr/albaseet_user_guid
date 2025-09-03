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

namespace Compound.Service.Services.Reports.FollowUpReports
{
	public class ClientPriceRequestFollowUpReportService : IClientPriceRequestFollowUpReportService
	{
		private readonly IClientPriceRequestHeaderService _clientPriceRequestHeaderService;
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

		public ClientPriceRequestFollowUpReportService(IClientPriceRequestHeaderService clientPriceRequestHeaderService, IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IMenuService menuService, IClientService clientService)
		{
			_clientPriceRequestHeaderService = clientPriceRequestHeaderService;
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
		}

		public IQueryable<ClientPriceRequestFollowUpReportDto> GetClientPriceRequestFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var query = from clientPriceRequestHeader in _clientPriceRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == clientPriceRequestHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ClientPriceRequest)
						from client in _clientService.GetAll().Where(x => x.ClientId == clientPriceRequestHeader.ClientId)
						from clientQuotationHeader in _clientQuotationHeaderService.GetAll().Where(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeader.ClientPriceRequestHeaderId).DefaultIfEmpty()
						from clientQuotationApprovalHeader in _clientQuotationApprovalHeaderService.GetAll().Where(x => x.ClientQuotationHeaderId == clientQuotationHeader.ClientQuotationHeaderId).DefaultIfEmpty()
						from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeader.ClientQuotationHeaderApprovalId).DefaultIfEmpty()
						from stockOutFromProformaInvoice in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId).DefaultIfEmpty()
						from stockOutReturnFromStockOut in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoice.StockOutHeaderId).DefaultIfEmpty()
						from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId).DefaultIfEmpty()
						from stockOutReturnFromSalesInvoice in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						group new { clientPriceRequestHeader, clientQuotationHeader, clientQuotationApprovalHeader, proformaInvoiceHeader, stockOutFromProformaInvoice, stockOutReturnFromStockOut, salesInvoiceHeader, stockOutReturnFromSalesInvoice, salesInvoiceReturnHeader, clientDebitMemo, clientCreditMemo, store, menu }
						by new { clientPriceRequestHeader.ClientPriceRequestHeaderId, clientPriceRequestHeader.ClientId, client.ClientNameAr, client.ClientNameEn, menu.MenuCode, menu.MenuNameAr, menu.MenuNameEn, clientPriceRequestHeader.Prefix, clientPriceRequestHeader.DocumentCode, clientPriceRequestHeader.Suffix, clientPriceRequestHeader.DocumentDate, clientPriceRequestHeader.ConsumerValue, clientPriceRequestHeader.Reference, clientPriceRequestHeader.RemarksAr, clientPriceRequestHeader.RemarksEn, store.StoreId, store.StoreNameAr, store.StoreNameEn, clientPriceRequestHeader.CreatedAt, clientPriceRequestHeader.UserNameCreated, clientPriceRequestHeader.ModifiedAt, clientPriceRequestHeader.UserNameModified }
						into g
						orderby g.Key.DocumentDate, g.Key.ClientPriceRequestHeaderId
						select new ClientPriceRequestFollowUpReportDto
						{
							ClientPriceRequestHeaderId = g.Key.ClientPriceRequestHeaderId,
							MenuCode = g.Key.MenuCode,
							MenuName = language == LanguageCode.Arabic ? g.Key.MenuNameAr : g.Key.MenuNameEn,
							DocumentFullCode = g.Key.Prefix + g.Key.DocumentCode + g.Key.Suffix,
							DocumentDate = g.Key.DocumentDate,
							Price = g.Key.ConsumerValue,
							ClientId = g.Key.ClientId,
							ClientName = language == LanguageCode.Arabic ? g.Key.ClientNameAr : g.Key.ClientNameEn,
							StoreId = g.Key.StoreId,
							StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
							ClientQuotation = g.Count(x => x.clientQuotationHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							ClientQuotationApproval = g.Count(x => x.clientQuotationApprovalHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							ProformaInvoice = g.Count(x => x.proformaInvoiceHeader != null && (x.salesInvoiceHeader == null || !x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							StockOutFromProformaInvoice = g.Count(x => x.stockOutFromProformaInvoice != null && (x.salesInvoiceHeader == null || !x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							StockOutReturnFromStockOut = g.Count(x => x.stockOutReturnFromStockOut != null && (x.salesInvoiceHeader == null || !x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							SalesInvoiceInterim = g.Count(x => x.salesInvoiceHeader != null && (!x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							ReceiptVoucher = g.Count(x => x.salesInvoiceHeader != null && (!x.salesInvoiceHeader.IsDirectInvoice) && x.salesInvoiceHeader.IsSettlementCompleted) > 0 ? _localizer["Done"].ToString() : null,
							StockOutReturnFromSalesInvoice = g.Count(x => x.stockOutReturnFromSalesInvoice != null && (!x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							SalesInvoiceReturn = g.Count(x => x.salesInvoiceReturnHeader != null && (!x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							ClientDebitMemo = g.Count(x => x.clientDebitMemo != null && (!x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							ClientCreditMemo = g.Count(x => x.clientCreditMemo != null && (!x.salesInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
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