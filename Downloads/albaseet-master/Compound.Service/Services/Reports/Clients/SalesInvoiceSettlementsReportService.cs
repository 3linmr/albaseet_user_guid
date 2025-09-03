using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Contracts.Reports.Suppliers;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Logic;
using Compound.CoreOne.Models.Dtos.Reports.Suppliers;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Accounting.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Menus;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.CoreOne.Contracts.Reports.Clients;
using Sales.CoreOne.Contracts;

namespace Compound.Service.Services.Reports.Clients
{
	public class SalesInvoiceSettlementsReportService: ISalesInvoiceSettlementsReportService
    {
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IClientService _clientService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesValueService _salesValueService;
		private readonly IStoreService _storeService;
		private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;
		private readonly IReceiptVoucherHeaderService  _receiptVoucherHeaderService;
		private readonly ISellerService _sellerService;
		private readonly IMenuService _menuService;

		public SalesInvoiceSettlementsReportService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IClientService clientService, IHttpContextAccessor httpContextAccessor, ISalesValueService salesValueService, IStoreService storeService, ISalesInvoiceSettlementService salesInvoiceSettlementService, IReceiptVoucherHeaderService receiptVoucherHeaderService, ISellerService sellerService, IMenuService menuService)
		{
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_clientService = clientService;
			_httpContextAccessor = httpContextAccessor;
			_salesValueService = salesValueService;
			_storeService = storeService;
			_salesInvoiceSettlementService = salesInvoiceSettlementService;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_sellerService = sellerService;
			_menuService = menuService;
		}

		public IQueryable<SalesInvoiceSettlementsReportDto> GetSalesInvoiceSettlementsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow().Date;

			return from salesInvoiceSettlement in _salesInvoiceSettlementService.GetAll()
				   from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceSettlement.SalesInvoiceHeaderId && storeIds.Contains(x.StoreId) && (clientId == null || x.ClientId == clientId))
				   from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
				   from salesInvoiceValue in _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
				   from salesInvoiceSettleValue in _salesInvoiceSettlementService.GetSalesInvoiceSettledValues().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
				   from receiptVoucherHeader in _receiptVoucherHeaderService.GetAll().Where(x => x.ReceiptVoucherHeaderId == salesInvoiceSettlement.ReceiptVoucherHeaderId && (toDate == null || x.DocumentDate <= toDate) && (fromDate == null || x.DocumentDate >= fromDate) && (sellerId == null || x.SellerId == sellerId))
				   from client in _clientService.GetAll().Where(x => x.ClientId == salesInvoiceHeader.ClientId)
				   from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
				   from seller in _sellerService.GetAll().Where(x => x.SellerId == receiptVoucherHeader.SellerId).DefaultIfEmpty()
				   where salesInvoiceValue.OverallNetValue > 0
				   orderby receiptVoucherHeader.DocumentDate, receiptVoucherHeader.EntryDate
				   select new SalesInvoiceSettlementsReportDto
				   {
					   SalesInvoiceSettlementId = salesInvoiceSettlement.SalesInvoiceSettlementId,
					   SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
					   DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
					   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
					   DocumentDate = salesInvoiceHeader.DocumentDate,
					   EntryDate = salesInvoiceHeader.EntryDate,
					   ClientId = salesInvoiceHeader.ClientId,
					   ClientCode = client.ClientCode,
					   ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
                       InvoiceValue = salesInvoiceValue.OverallNetValue,
					   SettledValue = salesInvoiceSettleValue.SettledValue ?? 0,
					   RemainingValue = salesInvoiceValue.OverallNetValue - (salesInvoiceSettleValue.SettledValue ?? 0),
					   StoreId = salesInvoiceHeader.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					   DueDate = salesInvoiceHeader.DueDate,
					   Age = Math.Max(EF.Functions.DateDiffDay(salesInvoiceHeader.DueDate, today) ?? 0, 0),
					   SellerId = receiptVoucherHeader.SellerId,
					   SellerCode = seller != null ? seller.SellerCode : null,
					   SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : null,
					   Reference = salesInvoiceHeader.Reference,
					   RemarksAr = salesInvoiceHeader.RemarksAr,
					   RemarksEn = salesInvoiceHeader.RemarksEn,
					   RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, salesInvoiceHeader.DueDate) ?? 0, 0),
					   InvoiceDuration = EF.Functions.DateDiffDay(salesInvoiceHeader.DocumentDate, salesInvoiceHeader.DueDate) ?? 0,
					   ReceiptVoucherHeaderId = receiptVoucherHeader.ReceiptVoucherHeaderId,
					   VoucherFullCode = receiptVoucherHeader.Prefix + receiptVoucherHeader.ReceiptVoucherCode + receiptVoucherHeader.Suffix,
					   VoucherAmount = salesInvoiceSettlement.SettleValue,
					   VoucherDate = receiptVoucherHeader.DocumentDate,
					   VoucherAge = Math.Max(EF.Functions.DateDiffDay(salesInvoiceHeader.DueDate, receiptVoucherHeader.DocumentDate) ?? 0, 0),

					   CreatedAt = receiptVoucherHeader.CreatedAt,
					   UserNameCreated = receiptVoucherHeader.UserNameCreated,
					   ModifiedAt = receiptVoucherHeader.ModifiedAt,
					   UserNameModified = receiptVoucherHeader.UserNameModified,
				   };
		}

       
    }
}
