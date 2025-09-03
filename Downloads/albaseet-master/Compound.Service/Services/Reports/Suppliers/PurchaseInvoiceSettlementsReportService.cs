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

namespace Compound.Service.Services.Reports.Suppliers
{
	public class PurchaseInvoiceSettlementsReportService: IPurchaseInvoiceSettlementsReportService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly ISupplierService _supplierService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPurchaseValueService _purchaseValueService;
		private readonly IStoreService _storeService;
		private readonly IPurchaseInvoiceSettlementService _purchaseInvoiceSettlementService;
		private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly ISellerService _sellerService;
		private readonly IMenuService _menuService;

		public PurchaseInvoiceSettlementsReportService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, ISupplierService supplierService, IHttpContextAccessor httpContextAccessor, IPurchaseValueService purchaseValueService, IStoreService storeService, IPurchaseInvoiceSettlementService purchaseInvoiceSettlementService, IPaymentVoucherHeaderService paymentVoucherHeaderService, ISellerService sellerService, IMenuService menuService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_supplierService = supplierService;
			_httpContextAccessor = httpContextAccessor;
			_purchaseValueService = purchaseValueService;
			_storeService = storeService;
			_purchaseInvoiceSettlementService = purchaseInvoiceSettlementService;
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_sellerService = sellerService;
			_menuService = menuService;
		}

		public IQueryable<PurchaseInvoiceSettlementsReportDto> GetPurchaseInvoiceSettlementsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId, int? sellerId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow().Date;

			return from purchaseInvoiceSettlement in _purchaseInvoiceSettlementService.GetAll()
				   from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceSettlement.PurchaseInvoiceHeaderId && storeIds.Contains(x.StoreId) && (supplierId == null || x.SupplierId == supplierId))
				   from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceHeader.MenuCode).DefaultIfEmpty()
				   from purchaseInvoiceValue in _purchaseValueService.GetOverallValueOfPurchaseInvoices().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
				   from purchaseInvoiceSettleValue in _purchaseInvoiceSettlementService.GetPurchaseInvoiceSettledValues().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
				   from paymentVoucherHeader in _paymentVoucherHeaderService.GetAll().Where(x => x.PaymentVoucherHeaderId == purchaseInvoiceSettlement.PaymentVoucherHeaderId && (toDate == null || x.DocumentDate <= toDate) && (fromDate == null || x.DocumentDate >= fromDate) && (sellerId == null || x.SellerId == sellerId))
				   from supplier in _supplierService.GetAll().Where(x => x.SupplierId == purchaseInvoiceHeader.SupplierId)
				   from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceHeader.StoreId)
				   from seller in _sellerService.GetAll().Where(x => x.SellerId == paymentVoucherHeader.SellerId).DefaultIfEmpty()
				   where purchaseInvoiceValue.OverallValue > 0
				   orderby paymentVoucherHeader.DocumentDate, paymentVoucherHeader.EntryDate
				   select new PurchaseInvoiceSettlementsReportDto
				   {
					   PurchaseInvoiceSettlementId = purchaseInvoiceSettlement.PurchaseInvoiceSettlementId,
					   PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
					   DocumentFullCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
					   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
					   DocumentDate = purchaseInvoiceHeader.DocumentDate,
					   EntryDate = purchaseInvoiceHeader.EntryDate,
					   SupplierId = purchaseInvoiceHeader.SupplierId,
					   SupplierCode = supplier.SupplierCode,
					   SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
					   InvoiceValue = purchaseInvoiceValue.OverallValue,
					   SettledValue = purchaseInvoiceSettleValue.SettledValue ?? 0,
					   RemainingValue = purchaseInvoiceValue.OverallValue - (purchaseInvoiceSettleValue.SettledValue ?? 0),
					   StoreId = purchaseInvoiceHeader.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					   DueDate = purchaseInvoiceHeader.DueDate,
					   Age = Math.Max(EF.Functions.DateDiffDay(purchaseInvoiceHeader.DueDate, today) ?? 0, 0),
					   SellerId = paymentVoucherHeader.SellerId,
					   SellerCode = seller != null ? seller.SellerCode : null,
					   SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : null,
					   Reference = purchaseInvoiceHeader.Reference,
					   RemarksAr = purchaseInvoiceHeader.RemarksAr,
					   RemarksEn = purchaseInvoiceHeader.RemarksEn,
					   RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, purchaseInvoiceHeader.DueDate) ?? 0, 0),
					   InvoiceDuration = EF.Functions.DateDiffDay(purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.DueDate) ?? 0,
					   PaymentVoucherHeaderId = paymentVoucherHeader.PaymentVoucherHeaderId,
					   VoucherFullCode = paymentVoucherHeader.Prefix + paymentVoucherHeader.PaymentVoucherCode + paymentVoucherHeader.Suffix,
					   VoucherAmount = purchaseInvoiceSettlement.SettleValue,
					   VoucherDate = paymentVoucherHeader.DocumentDate,
					   VoucherAge = Math.Max(EF.Functions.DateDiffDay(purchaseInvoiceHeader.DueDate, paymentVoucherHeader.DocumentDate) ?? 0, 0),

					   CreatedAt = paymentVoucherHeader.CreatedAt,
					   UserNameCreated = paymentVoucherHeader.UserNameCreated,
					   ModifiedAt = paymentVoucherHeader.ModifiedAt,
					   UserNameModified = paymentVoucherHeader.UserNameModified,
				   };
		}
	}
}
