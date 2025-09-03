using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Accounts;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Accounting.CoreOne.Contracts;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Modules;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Shared.CoreOne.Contracts.Menus;

namespace Compound.Service.Services.Reports.Accounting
{
	public class StoreIncomeReportService : IStoreIncomeReportService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly IReceiptVoucherDetailService _receiptVoucherDetailService;
		private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly IPaymentVoucherDetailService _paymentVoucherDetailService;
		private readonly IClientService _clientService;
		private readonly IMenuService _menuService;
		private readonly ISellerService _sellerService;
		private readonly IPaymentMethodService _paymentMethodService;

		public StoreIncomeReportService(IHttpContextAccessor httpContextAccessor, IStoreService storeService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IReceiptVoucherHeaderService receiptVoucherHeaderService, IReceiptVoucherDetailService receiptVoucherDetailService, IPaymentVoucherHeaderService paymentVoucherHeaderService, IPaymentVoucherDetailService paymentVoucherDetailService, IClientService clientService, IMenuService menuService, ISellerService sellerService, IPaymentMethodService paymentMethodService)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_receiptVoucherDetailService = receiptVoucherDetailService;
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_paymentVoucherDetailService = paymentVoucherDetailService;
			_clientService = clientService;
			_menuService = menuService;
			_sellerService = sellerService;
			_paymentMethodService = paymentMethodService;
		}

		public IQueryable<StoreIncomeReportDto> GetStoreIncomeReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<short> menuCodes)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var invoiceDocuments = GetCashIncomeData(storeIds, fromDate, toDate);

			return from invoiceDocument in invoiceDocuments
				   from store in _storeService.GetAll().Where(x => x.StoreId == invoiceDocument.StoreId)
				   from client in _clientService.GetAll().Where(x => x.ClientId == invoiceDocument.ClientId).DefaultIfEmpty()
				   from seller in _sellerService.GetAll().Where(x => x.SellerId == invoiceDocument.SellerId).DefaultIfEmpty()
				   from menu in _menuService.GetAll().Where(x => x.MenuCode == invoiceDocument.MenuCode).DefaultIfEmpty()
				   where (menuCodes.Count == 0 || menuCodes.Contains(invoiceDocument.MenuCode))
				   orderby invoiceDocument.DocumentDate, invoiceDocument.EntryDate, invoiceDocument.HeaderId, invoiceDocument.MenuCode
				   select new StoreIncomeReportDto
				   {
					   MenuCode = invoiceDocument.MenuCode,
					   MenuName = menu != null ? (language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn) : null,
					   HeaderId = invoiceDocument.HeaderId,
					   DocumentFullCode = invoiceDocument.DocumentFullCode,
					   DocumentDate = invoiceDocument.DocumentDate,
					   EntryDate = invoiceDocument.EntryDate,
					   NetValue = invoiceDocument.NetValue,
					   ClientId = invoiceDocument.ClientId,
					   ClientCode = client.ClientCode,
					   ClientName = client != null ? (language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn) : null,
					   SellerId = invoiceDocument.SellerId,
					   SellerCode = seller.SellerCode,
					   SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : null,
					   StoreId = invoiceDocument.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

					   CreatedAt = invoiceDocument.CreatedAt,
					   UserNameCreated = invoiceDocument.UserNameCreated,
					   ModifiedAt = invoiceDocument.ModifiedAt,
					   UserNameModified = invoiceDocument.UserNameModified,
				   };
		}

		private IQueryable<StoreIncomeReportDataDto> GetCashIncomeData(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var salesInvoiceHeaders = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(
										  x => storeIds.Contains(x.StoreId) &&
										  !x.CreditPayment &&
										  (fromDate == null || x.DocumentDate >= fromDate) &&
										  (toDate == null || x.DocumentDate <= toDate)
									  )
									  select new StoreIncomeReportDataDto
									  {
										  HeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
										  MenuCode = salesInvoiceHeader.MenuCode ?? 0,
										  DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
										  DocumentDate = salesInvoiceHeader.DocumentDate,
										  EntryDate = salesInvoiceHeader.EntryDate,
										  NetValue = salesInvoiceHeader.NetValue,
										  ClientId = salesInvoiceHeader.ClientId,
										  SellerId = salesInvoiceHeader.SellerId,
										  StoreId = salesInvoiceHeader.StoreId,
										  CreatedAt = salesInvoiceHeader.CreatedAt,
										  UserNameCreated = salesInvoiceHeader.UserNameCreated,
										  ModifiedAt = salesInvoiceHeader.ModifiedAt,
										  UserNameModified = salesInvoiceHeader.UserNameModified,
									  };

			var salesInvoiceReturnHeaders = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(
												x => storeIds.Contains(x.StoreId) &&
												!x.CreditPayment &&
												(fromDate == null || x.DocumentDate >= fromDate) &&
												(toDate == null || x.DocumentDate <= toDate)
											)
											select new StoreIncomeReportDataDto
											{
												HeaderId = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
												MenuCode = salesInvoiceReturnHeader.MenuCode ?? 0,
												DocumentFullCode = salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix,
												DocumentDate = salesInvoiceReturnHeader.DocumentDate,
												EntryDate = salesInvoiceReturnHeader.EntryDate,
												NetValue = -salesInvoiceReturnHeader.NetValue,
												ClientId = salesInvoiceReturnHeader.ClientId,
												SellerId = salesInvoiceReturnHeader.SellerId,
												StoreId = salesInvoiceReturnHeader.StoreId,
										        CreatedAt = salesInvoiceReturnHeader.CreatedAt,
										        UserNameCreated = salesInvoiceReturnHeader.UserNameCreated,
										        ModifiedAt = salesInvoiceReturnHeader.ModifiedAt,
										        UserNameModified = salesInvoiceReturnHeader.UserNameModified,
											};

			var purchaseInvoiceHeaders = from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(
											 x => storeIds.Contains(x.StoreId) &&
											 !x.CreditPayment &&
											 (fromDate == null || x.DocumentDate >= fromDate) &&
											 (toDate == null || x.DocumentDate <= toDate)
										 )
										 select new StoreIncomeReportDataDto
										 {
											 HeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
											 MenuCode = purchaseInvoiceHeader.MenuCode ?? 0,
											 DocumentFullCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
											 DocumentDate = purchaseInvoiceHeader.DocumentDate,
											 EntryDate = purchaseInvoiceHeader.EntryDate,
											 NetValue = -purchaseInvoiceHeader.NetValue,
											 ClientId = null,
											 SellerId = null,
											 StoreId = purchaseInvoiceHeader.StoreId,
										     CreatedAt = purchaseInvoiceHeader.CreatedAt,
										     UserNameCreated = purchaseInvoiceHeader.UserNameCreated,
										     ModifiedAt = purchaseInvoiceHeader.ModifiedAt,
										     UserNameModified = purchaseInvoiceHeader.UserNameModified,
										 };

			var purchaseInvoiceReturnHeaders = from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(
												   x => storeIds.Contains(x.StoreId) &&
												   !x.CreditPayment &&
												   (fromDate == null || x.DocumentDate >= fromDate) &&
												   (toDate == null || x.DocumentDate <= toDate)
											   )
											   select new StoreIncomeReportDataDto
											   {
												   HeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId,
												   MenuCode = purchaseInvoiceReturnHeader.MenuCode ?? 0,
												   DocumentFullCode = purchaseInvoiceReturnHeader.Prefix + purchaseInvoiceReturnHeader.DocumentCode + purchaseInvoiceReturnHeader.Suffix,
												   DocumentDate = purchaseInvoiceReturnHeader.DocumentDate,
												   EntryDate = purchaseInvoiceReturnHeader.EntryDate,
												   NetValue = purchaseInvoiceReturnHeader.NetValue,
												   ClientId = null,
												   SellerId = null,
												   StoreId = purchaseInvoiceReturnHeader.StoreId,
										           CreatedAt = purchaseInvoiceReturnHeader.CreatedAt,
										           UserNameCreated = purchaseInvoiceReturnHeader.UserNameCreated,
										           ModifiedAt = purchaseInvoiceReturnHeader.ModifiedAt,
										           UserNameModified = purchaseInvoiceReturnHeader.UserNameModified,
											   };

			var receiptVoucherHeaders = from receiptVoucherHeader in _receiptVoucherHeaderService.GetAll().Where(
											x => storeIds.Contains(x.StoreId) &&
											(fromDate == null || x.DocumentDate >= fromDate) &&
											(toDate == null || x.DocumentDate <= toDate)
										)
										from receiptVoucherDetail in _receiptVoucherDetailService.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeader.ReceiptVoucherHeaderId)
										from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == receiptVoucherDetail.PaymentMethodId)
										where paymentMethod.PaymentTypeId == PaymentTypeData.Cash
										group receiptVoucherDetail by new { receiptVoucherHeader.ReceiptVoucherHeaderId, receiptVoucherHeader.Prefix, receiptVoucherHeader.ReceiptVoucherCode, receiptVoucherHeader.Suffix, receiptVoucherHeader.DocumentDate, receiptVoucherHeader.EntryDate, receiptVoucherHeader.ClientId, receiptVoucherHeader.SellerId, receiptVoucherHeader.StoreId, receiptVoucherHeader.CreatedAt, receiptVoucherHeader.UserNameCreated, receiptVoucherHeader.ModifiedAt, receiptVoucherHeader.UserNameModified } into g
										select new StoreIncomeReportDataDto
										{
											HeaderId = g.Key.ReceiptVoucherHeaderId,
											MenuCode = MenuCodeData.ReceiptVoucher,
											DocumentFullCode = g.Key.Prefix + g.Key.ReceiptVoucherCode + g.Key.Suffix,
											DocumentDate = g.Key.DocumentDate,
											EntryDate = g.Key.EntryDate,
											NetValue = g.Sum(x => x.DebitValue),
											ClientId = g.Key.ClientId,
											SellerId = g.Key.SellerId,
											StoreId = g.Key.StoreId,
											CreatedAt = g.Key.CreatedAt,
											UserNameCreated = g.Key.UserNameCreated,
											ModifiedAt = g.Key.ModifiedAt,
											UserNameModified = g.Key.UserNameModified
										};

			var paymentVoucherHeaders = from paymentVoucherHeader in _paymentVoucherHeaderService.GetAll().Where(
											x => storeIds.Contains(x.StoreId) &&
											(fromDate == null || x.DocumentDate >= fromDate) &&
											(toDate == null || x.DocumentDate <= toDate)
										)
										from paymentVoucherDetail in _paymentVoucherDetailService.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeader.PaymentVoucherHeaderId)
										from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == paymentVoucherDetail.PaymentMethodId)
										where paymentMethod.PaymentTypeId == PaymentTypeData.Cash
										group paymentVoucherDetail by new { paymentVoucherHeader.PaymentVoucherHeaderId, paymentVoucherHeader.Prefix, paymentVoucherHeader.PaymentVoucherCode, paymentVoucherHeader.Suffix, paymentVoucherHeader.DocumentDate, paymentVoucherHeader.EntryDate, paymentVoucherHeader.SellerId, paymentVoucherHeader.StoreId, paymentVoucherHeader.CreatedAt, paymentVoucherHeader.UserNameCreated, paymentVoucherHeader.ModifiedAt, paymentVoucherHeader.UserNameModified } into g
										select new StoreIncomeReportDataDto
										{
											HeaderId = g.Key.PaymentVoucherHeaderId,
											MenuCode = MenuCodeData.PaymentVoucher,
											DocumentFullCode = g.Key.Prefix + g.Key.PaymentVoucherCode + g.Key.Suffix,
											DocumentDate = g.Key.DocumentDate,
											EntryDate = g.Key.EntryDate,
											NetValue = -g.Sum(x => x.CreditValue),
											ClientId = null,
											SellerId = g.Key.SellerId,
											StoreId = g.Key.StoreId,
											CreatedAt = g.Key.CreatedAt,
											UserNameCreated = g.Key.UserNameCreated,
											ModifiedAt = g.Key.ModifiedAt,
											UserNameModified = g.Key.UserNameModified
										};


			return salesInvoiceHeaders
				.Concat(salesInvoiceReturnHeaders)
				.Concat(purchaseInvoiceHeaders)
				.Concat(purchaseInvoiceReturnHeaders)
				.Concat(receiptVoucherHeaders)
				.Concat(paymentVoucherHeaders);
		}
	}
}
