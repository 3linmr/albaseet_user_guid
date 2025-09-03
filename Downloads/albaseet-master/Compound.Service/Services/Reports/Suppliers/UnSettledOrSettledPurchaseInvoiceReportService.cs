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
using Shared.CoreOne.Contracts.Accounts;

namespace Compound.Service.Services.Reports.Suppliers
{
	public class UnSettledOrSettledPurchaseInvoiceReportService: IUnSettledOrSettledPurchaseInvoiceReportService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly ISupplierService _supplierService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPurchaseValueService _purchaseValueService;
		private readonly IStoreService _storeService;
		private readonly IPurchaseInvoiceSettlementService _purchaseInvoiceSettlementService;
		private readonly IMenuService _menuService;
		private readonly IAccountService _accountService;
		private readonly IBranchService _branchService;
		private readonly ICompanyService _companyService;

		public UnSettledOrSettledPurchaseInvoiceReportService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, ISupplierService supplierService, IHttpContextAccessor httpContextAccessor, IPurchaseValueService purchaseValueService, IStoreService storeService, IPurchaseInvoiceSettlementService purchaseInvoiceSettlementService, IMenuService menuService, IAccountService accountService, IBranchService branchService, ICompanyService companyService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_supplierService = supplierService;
			_httpContextAccessor = httpContextAccessor;
			_purchaseValueService = purchaseValueService;
			_storeService = storeService;
			_purchaseInvoiceSettlementService = purchaseInvoiceSettlementService;
			_menuService = menuService;
			_accountService = accountService;
			_branchService = branchService;
			_companyService = companyService;
		}

		public IQueryable<UnSettledOrSettledPurchaseInvoicesReportDto> GetUnSettledOrSettledPurchaseInvoicesReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId, bool getSettled)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow().Date;

			return from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && x.CreditPayment && x.IsSettlementCompleted == getSettled && (toDate == null || x.DueDate <= toDate) && (fromDate == null || x.DueDate >= fromDate) && (supplierId == null || x.SupplierId == supplierId))
				   from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceHeader.MenuCode).DefaultIfEmpty()
				   from purchaseInvoiceValue in _purchaseValueService.GetOverallValueOfPurchaseInvoices().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
				   from purchaseInvoiceSettleValue in _purchaseInvoiceSettlementService.GetPurchaseInvoiceSettledValues().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
				   from supplier in _supplierService.GetAll().Where(x => x.SupplierId == purchaseInvoiceHeader.SupplierId)
				   from account in _accountService.GetAll().Where(x => x.AccountId == supplier.AccountId)
				   from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceHeader.StoreId)
				   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
				   from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
				   where purchaseInvoiceValue.OverallValue > 0
				   orderby purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.EntryDate
				   select new UnSettledOrSettledPurchaseInvoicesReportDto
				   {
					   PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
					   DocumentFullCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
					   MenuCode = purchaseInvoiceHeader.MenuCode,
					   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
					   DocumentDate = purchaseInvoiceHeader.DocumentDate,
					   EntryDate = purchaseInvoiceHeader.EntryDate,
					   SupplierId = purchaseInvoiceHeader.SupplierId,
					   SupplierCode = supplier.SupplierCode,
					   SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
					   AccountId = account.AccountId,
					   AccountCode = account.AccountCode,
					   AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
					   InvoiceValue = purchaseInvoiceValue.OverallValue,
					   SettledValue = purchaseInvoiceSettleValue.SettledValue ?? 0,
					   RemainingValue = purchaseInvoiceValue.OverallValue - (purchaseInvoiceSettleValue.SettledValue ?? 0),
					   StoreId = purchaseInvoiceHeader.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					   BranchId = branch.BranchId,
					   BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
					   CompanyId = company.CompanyId,
					   CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					   DueDate = purchaseInvoiceHeader.DueDate,
					   Age = Math.Max(EF.Functions.DateDiffDay(purchaseInvoiceHeader.DueDate, today) ?? 0, 0),
					   Reference = purchaseInvoiceHeader.Reference,
					   RemarksAr = purchaseInvoiceHeader.RemarksAr,
					   RemarksEn = purchaseInvoiceHeader.RemarksEn,
					   RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, purchaseInvoiceHeader.DueDate) ?? 0, 0),
					   InvoiceDuration = EF.Functions.DateDiffDay(purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.DueDate) ?? 0,
					   CreatedAt = purchaseInvoiceHeader.CreatedAt,
					   UserNameCreated = purchaseInvoiceHeader.UserNameCreated,
					   ModifiedAt = purchaseInvoiceHeader.ModifiedAt,
					   UserNameModified = purchaseInvoiceHeader.UserNameModified,
				   };
		}
	}
}
