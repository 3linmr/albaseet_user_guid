using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Menus
{
	public class MenuService : BaseService<Menu>,IMenuService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public MenuService(IRepository<Menu> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<List<DocumentDto>> GetDocuments()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Select(s => new DocumentDto()
			{
				MenuCode = s.MenuCode,
				MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
				MenuNameAr = s.MenuNameAr,
				MenuNameEn = s.MenuNameEn
			}).ToListAsync();
		}

		public async Task<List<MenuCodeDto>> GetAllMenus()
		{
			var userMenus = await _httpContextAccessor.GetUserMenus();
			var allMenus =await _repository.GetAll().ToListAsync();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var menus =
			(from menu in allMenus
			 from userMenu in userMenus.Where(x=>x.MenuCode == menu.MenuCode)	
			select new MenuCodeDto
			{
				MenuCode = menu.MenuCode,
				MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
				MenuNameAr = menu.MenuNameAr,
				MenuNameEn = menu.MenuNameEn,
				HasApprove = menu.HasApprove,
				HasNotes = menu.HasNotes,
				HasEncoding = menu.HasEncoding,
				IsFavorite = menu.IsFavorite
			}).ToList();
			return menus;
		}

		public async Task<MenuCodeDto?> GetMenuByMenuCode(int menuCode)
		{
			return (await GetAllMenus()).FirstOrDefault(x =>  x.MenuCode == menuCode);
		}

		public async Task<List<MenuCodeDropDownDto>> GetAllMenusDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var allMenus = await GetAllMenus();
			return allMenus.Select(s => new MenuCodeDropDownDto()
			{
				MenuCode = s.MenuCode,
				MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
			}).ToList();

		}

		public async Task<List<MenuCodeDropDownDto>> GetMenusHasApprovesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var allMenus = await GetAllMenus();
			return allMenus.Where(x=>x.HasApprove).Select(s => new MenuCodeDropDownDto()
			{
				MenuCode = s.MenuCode,
				MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
			}).ToList();
		}

		public async Task<List<MenuCodeDropDownDto>> GetMenusHasNotesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var allMenus = await GetAllMenus();
			return allMenus.Where(x => x.HasNotes).Select(s => new MenuCodeDropDownDto()
			{
				MenuCode = s.MenuCode,
				MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
			}).ToList();
		}

		public async Task<List<MenuCodeDropDownDto>> GetMenusHasEncodingDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var allMenus = await GetAllMenus();
			return allMenus.Where(x => x.HasEncoding).Select(s => new MenuCodeDropDownDto()
			{
				MenuCode = s.MenuCode,
				MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
			}).ToList();
		}

        public async Task<List<MenuCodeDropDownDto>> GetMenusShippingStatusDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => x.MenuCode == MenuCodeData.ProformaInvoice).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }

		private static readonly List<short> inventoryDocuments = [
			MenuCodeData.InventoryIn,
			MenuCodeData.InventoryOut,
			MenuCodeData.InternalTransferItems,
			MenuCodeData.InternalReceiveItems,
			MenuCodeData.StockTakingOpenBalance,
			MenuCodeData.StockTakingCurrentBalance,
			MenuCodeData.ApprovalStockTakingAsOpenBalance,
			MenuCodeData.ApprovalStockTakingAsCurrentBalance,

			MenuCodeData.ReceiptStatement,
			MenuCodeData.ReceiptStatementReturn,
			MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay,
			MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWayReturn,
			MenuCodeData.ReturnFromPurchaseInvoice,

			MenuCodeData.CashPurchaseInvoice,
			MenuCodeData.CashPurchaseInvoiceReturn,
			MenuCodeData.CreditPurchaseInvoice,
			MenuCodeData.CreditPurchaseInvoiceReturn,

			MenuCodeData.StockOutFromProformaInvoice,
			MenuCodeData.StockOutReturnFromStockOut,
			MenuCodeData.StockOutFromReservation,
			MenuCodeData.StockOutReturnFromReservation,
			MenuCodeData.StockOutReturnFromInvoice,

			MenuCodeData.CashSalesInvoice,
			MenuCodeData.CashSalesInvoiceReturn,
			MenuCodeData.CreditSalesInvoice,
			MenuCodeData.CreditSalesInvoiceReturn,
			MenuCodeData.CashReservationInvoice,
			MenuCodeData.CreditReservationInvoice,
			MenuCodeData.ReservationInvoiceCloseOut,

			MenuCodeData.DisassembleItemPackages
		];

		private static readonly List<short> itemTradingMovementReport = [
			MenuCodeData.CashPurchaseInvoice,
			MenuCodeData.CreditPurchaseInvoice,
			MenuCodeData.PurchaseInvoiceOnTheWayCash,
			MenuCodeData.PurchaseInvoiceOnTheWayCredit,
			MenuCodeData.PurchaseInvoiceInterim,

			MenuCodeData.CashPurchaseInvoiceReturn,
			MenuCodeData.CreditPurchaseInvoiceReturn,
			MenuCodeData.PurchaseInvoiceReturn,
			MenuCodeData.PurchaseInvoiceReturnOnTheWay,

			//MenuCodeData.SupplierDebitMemo,
			//MenuCodeData.SupplierCreditMemo,

			MenuCodeData.CashSalesInvoice,
			MenuCodeData.CreditSalesInvoice,
			MenuCodeData.CashReservationInvoice,
			MenuCodeData.CreditReservationInvoice,
			MenuCodeData.SalesInvoiceInterim,

			MenuCodeData.CashSalesInvoiceReturn,
			MenuCodeData.CreditSalesInvoiceReturn,
			MenuCodeData.SalesInvoiceReturn,
			MenuCodeData.ReservationInvoiceCloseOut,

			//MenuCodeData.ClientDebitMemo,
			//MenuCodeData.ClientCreditMemo,
		];

		private static readonly List<short> invoiceDocuments = [
			MenuCodeData.CashPurchaseInvoice,
			MenuCodeData.CreditPurchaseInvoice,
			MenuCodeData.PurchaseInvoiceOnTheWayCash,
			MenuCodeData.PurchaseInvoiceOnTheWayCredit,
			MenuCodeData.PurchaseInvoiceInterim,

			MenuCodeData.CashPurchaseInvoiceReturn,
			MenuCodeData.CreditPurchaseInvoiceReturn,
			MenuCodeData.PurchaseInvoiceReturn,
			MenuCodeData.PurchaseInvoiceReturnOnTheWay,

			MenuCodeData.SupplierDebitMemo,
			MenuCodeData.SupplierCreditMemo,

			MenuCodeData.CashSalesInvoice,
			MenuCodeData.CreditSalesInvoice,
			MenuCodeData.CashReservationInvoice,
			MenuCodeData.CreditReservationInvoice,
			MenuCodeData.SalesInvoiceInterim,

			MenuCodeData.CashSalesInvoiceReturn,
			MenuCodeData.CreditSalesInvoiceReturn,
			MenuCodeData.SalesInvoiceReturn,
			MenuCodeData.ReservationInvoiceCloseOut,

			MenuCodeData.ClientDebitMemo,
			MenuCodeData.ClientCreditMemo,

			MenuCodeData.ReceiptVoucher,
			MenuCodeData.PaymentVoucher,
		];

		private static readonly List<short> storeCashIncomeMenus = [
			MenuCodeData.CashPurchaseInvoice,
			MenuCodeData.PurchaseInvoiceOnTheWayCash,
			//MenuCodeData.PurchaseInvoiceInterim,

			MenuCodeData.CashPurchaseInvoiceReturn,
			MenuCodeData.PurchaseInvoiceReturn,
			MenuCodeData.PurchaseInvoiceReturnOnTheWay,

			MenuCodeData.CashSalesInvoice,
			MenuCodeData.CashReservationInvoice,
			//MenuCodeData.SalesInvoiceInterim,

			MenuCodeData.CashSalesInvoiceReturn,
			MenuCodeData.SalesInvoiceReturn,
			MenuCodeData.ReservationInvoiceCloseOut,

			MenuCodeData.ReceiptVoucher,
			MenuCodeData.PaymentVoucher,
		];

		private static readonly List<short> paymentMethodIncomeMenus = [
			MenuCodeData.CashSalesInvoice,
			MenuCodeData.CashReservationInvoice,
			//MenuCodeData.SalesInvoiceInterim,

			MenuCodeData.CashSalesInvoiceReturn,
			MenuCodeData.SalesInvoiceReturn,
			MenuCodeData.ReservationInvoiceCloseOut,

			MenuCodeData.ReceiptVoucher,
			MenuCodeData.PaymentVoucher,
		];

		private static readonly List<short> missingDocumentNumberMenus = [
			MenuCodeData.StockTakingOpenBalance,
			MenuCodeData.ApprovalStockTakingAsOpenBalance,
			MenuCodeData.StockTakingCurrentBalance,
			MenuCodeData.ApprovalStockTakingAsCurrentBalance,
			MenuCodeData.InventoryIn,
			MenuCodeData.InventoryOut,
			MenuCodeData.InternalTransferItems,
			MenuCodeData.InternalReceiveItems,
			MenuCodeData.JournalEntry,
			MenuCodeData.ReceiptVoucher,
			MenuCodeData.PaymentVoucher,
			MenuCodeData.ProductRequest,
			MenuCodeData.ProductRequestPrice,
			MenuCodeData.SupplierQuotation,
			MenuCodeData.CashPurchaseInvoice,
			MenuCodeData.CreditPurchaseInvoice,
			MenuCodeData.CashPurchaseInvoiceReturn,
			MenuCodeData.CreditPurchaseInvoiceReturn,
			MenuCodeData.PurchaseOrder,
			MenuCodeData.ReceiptStatement,
			MenuCodeData.ReceiptStatementReturn,
			MenuCodeData.PurchaseInvoiceInterim,
			MenuCodeData.PurchaseInvoiceOnTheWayCash,
			MenuCodeData.PurchaseInvoiceOnTheWayCredit,
			MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay,
			MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWayReturn,
			MenuCodeData.PurchaseInvoiceReturnOnTheWay,
			MenuCodeData.ReturnFromPurchaseInvoice,
			MenuCodeData.PurchaseInvoiceReturn,
			MenuCodeData.SupplierDebitMemo,
			MenuCodeData.SupplierCreditMemo,
			MenuCodeData.ClientPriceRequest,
			MenuCodeData.ClientQuotation,
			MenuCodeData.ClientQuotationApproval,
			MenuCodeData.CashSalesInvoice,
			MenuCodeData.CreditSalesInvoice,
			MenuCodeData.CashSalesInvoiceReturn,
			MenuCodeData.CreditSalesInvoiceReturn,
			MenuCodeData.ProformaInvoice,
			MenuCodeData.StockOutFromProformaInvoice,
			MenuCodeData.StockOutReturnFromStockOut,
			MenuCodeData.SalesInvoiceInterim,
			MenuCodeData.CashReservationInvoice,
			MenuCodeData.CreditReservationInvoice,
			MenuCodeData.StockOutFromReservation,
			MenuCodeData.StockOutReturnFromReservation,
			MenuCodeData.ReservationInvoiceCloseOut,
			MenuCodeData.StockOutReturnFromInvoice,
			MenuCodeData.SalesInvoiceReturn,
			MenuCodeData.ClientDebitMemo,
			MenuCodeData.ClientCreditMemo,
		];


		public async Task<List<MenuCodeDropDownDto>> GetMenusInventoryDocumentsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => inventoryDocuments.Contains(x.MenuCode)).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }

        public async Task<List<MenuCodeDropDownDto>> GetMenusItemTradingMovementDocumentsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => itemTradingMovementReport.Contains(x.MenuCode)).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }

        public async Task<List<MenuCodeDropDownDto>> GetMenusInvoiceDocumentsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => invoiceDocuments.Contains(x.MenuCode)).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }

        public async Task<List<MenuCodeDropDownDto>> GetMenusStoreCashIncomeDocumentsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => storeCashIncomeMenus.Contains(x.MenuCode)).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }
		
        public async Task<List<MenuCodeDropDownDto>> GetMenusPaymentMethodsIncomeDocumentsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => paymentMethodIncomeMenus.Contains(x.MenuCode)).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }

        public async Task<List<MenuCodeDropDownDto>> GetMenusMissingNumberDocumentsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var allMenus = await GetAllMenus();
            
			return allMenus.Where(x => missingDocumentNumberMenus.Contains(x.MenuCode)).Select(s => new MenuCodeDropDownDto()
            {
                MenuCode = s.MenuCode,
                MenuName = language == LanguageCode.Arabic ? s.MenuNameAr : s.MenuNameEn,
            }).ToList();
        }
    }
}
