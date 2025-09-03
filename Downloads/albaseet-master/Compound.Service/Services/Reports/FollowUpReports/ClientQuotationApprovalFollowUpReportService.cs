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
	public class ClientQuotationApprovalFollowUpReportService : IClientQuotationApprovalFollowUpReportService
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

		public ClientQuotationApprovalFollowUpReportService(IClientQuotationHeaderService clientQuotationHeaderService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IMenuService menuService, IClientService clientService)
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
		}

		public IQueryable<ClientQuotationApprovalFollowUpReportDto> GetClientQuotationApprovalFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var query = from clientQuotationApprovalHeader in _clientQuotationApprovalHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == clientQuotationApprovalHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ClientQuotationApproval)
						from client in _clientService.GetAll().Where(x => x.ClientId == clientQuotationApprovalHeader.ClientId)
						from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeader.ClientQuotationHeaderApprovalId).DefaultIfEmpty()
						from stockOutFromProformaInvoice in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId).DefaultIfEmpty()
						from stockOutReturnFromStockOut in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoice.StockOutHeaderId).DefaultIfEmpty()
						from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId).DefaultIfEmpty()
						from stockOutReturnFromSalesInvoice in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
						group new { clientQuotationApprovalHeader, proformaInvoiceHeader, stockOutFromProformaInvoice, stockOutReturnFromStockOut, salesInvoiceHeader, stockOutReturnFromSalesInvoice, salesInvoiceReturnHeader, clientDebitMemo, clientCreditMemo, store, menu }
						by new { clientQuotationApprovalHeader.ClientQuotationHeaderApprovalId, clientQuotationApprovalHeader.ClientId, client.ClientNameAr, client.ClientNameEn, menu.MenuCode, menu.MenuNameAr, menu.MenuNameEn, clientQuotationApprovalHeader.Prefix, clientQuotationApprovalHeader.DocumentCode, clientQuotationApprovalHeader.Suffix, clientQuotationApprovalHeader.DocumentDate, clientQuotationApprovalHeader.NetValue, clientQuotationApprovalHeader.Reference, clientQuotationApprovalHeader.RemarksAr, clientQuotationApprovalHeader.RemarksEn, store.StoreId, store.StoreNameAr, store.StoreNameEn, clientQuotationApprovalHeader.CreatedAt, clientQuotationApprovalHeader.UserNameCreated, clientQuotationApprovalHeader.ModifiedAt, clientQuotationApprovalHeader.UserNameModified }
						into g
						orderby g.Key.DocumentDate, g.Key.ClientQuotationHeaderApprovalId
						select new ClientQuotationApprovalFollowUpReportDto
						{
							ClientQuotationApprovalHeaderId = g.Key.ClientQuotationHeaderApprovalId,
							MenuCode = g.Key.MenuCode,
							MenuName = language == LanguageCode.Arabic ? g.Key.MenuNameAr : g.Key.MenuNameEn,
							DocumentFullCode = g.Key.Prefix + g.Key.DocumentCode + g.Key.Suffix,
							DocumentDate = g.Key.DocumentDate,
							Price = g.Key.NetValue,
							ClientId = g.Key.ClientId,
							ClientName = language == LanguageCode.Arabic ? g.Key.ClientNameAr : g.Key.ClientNameEn,
							StoreId = g.Key.StoreId,
							StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
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