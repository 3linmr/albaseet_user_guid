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
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Journal;

namespace Compound.Service.Services.Reports.Suppliers
{
	public class PurchaseTradingActivityReportService: IPurchaseTradingActivityReportService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly ISupplierService _supplierService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPurchaseValueService _purchaseValueService;
		private readonly IStoreService _storeService;
		private readonly IPurchaseInvoiceSettlementService _purchaseInvoiceSettlementService;
		private readonly IMenuService _menuService;
		private readonly IBranchService _branchService;
		private readonly ICompanyService _companyService;
		private readonly IAccountService _accountService;
		private readonly IInvoiceTypeService _invoiceTypeService;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;
		private readonly IJournalDetailService _journalDetailService;

		public PurchaseTradingActivityReportService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISupplierService supplierService, IHttpContextAccessor httpContextAccessor, IPurchaseValueService purchaseValueService, IStoreService storeService, IPurchaseInvoiceSettlementService purchaseInvoiceSettlementService, IMenuService menuService, IBranchService branchService, ICompanyService companyService, IAccountService accountService, IInvoiceTypeService invoiceTypeService, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IJournalDetailService journalDetailService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_supplierService = supplierService;
			_httpContextAccessor = httpContextAccessor;
			_purchaseValueService = purchaseValueService;
			_storeService = storeService;
			_purchaseInvoiceSettlementService = purchaseInvoiceSettlementService;
			_menuService = menuService;
			_branchService = branchService;
			_companyService = companyService;
			_accountService = accountService;
			_invoiceTypeService = invoiceTypeService;
			_supplierCreditMemoService = supplierCreditMemoService;
			_supplierDebitMemoService = supplierDebitMemoService;
			_journalDetailService = journalDetailService;
		}

		public IQueryable<PurchaseTradingActivityReportDto> GetPurchaseTradingActivityReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow().Date;

			var purchaseInvoices = from supplier in _supplierService.GetAllSuppliers()
								   from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.SupplierId == supplier.SupplierId)
								   from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceHeader.MenuCode).DefaultIfEmpty()
								   from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceHeader.StoreId)
								   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
								   from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
								   from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == purchaseInvoiceHeader.InvoiceTypeId)
								   where (fromDate == null || purchaseInvoiceHeader.DocumentDate >= fromDate) &&
										 (toDate == null || purchaseInvoiceHeader.DocumentDate <= toDate) &&
										 (supplierId == null || purchaseInvoiceHeader.SupplierId == supplierId) &&
										 storeIds.Contains(store.StoreId)
								   select new PurchaseTradingActivityReportDto
								   {
									   HeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
									   DocumentFullCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
									   SupplierId = purchaseInvoiceHeader.SupplierId,
									   SupplierCode = supplier.SupplierCode,
									   SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
									   AccountId = supplier.AccountId,
									   AccountCode = supplier.AccountCode,
									   AccountName = supplier.AccountName,
									   Reference = purchaseInvoiceHeader.Reference,
									   RemarksAr = purchaseInvoiceHeader.RemarksAr,
									   RemarksEn = purchaseInvoiceHeader.RemarksEn,
									   MenuCode = purchaseInvoiceHeader.MenuCode,
									   MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
									   DocumentDate = purchaseInvoiceHeader.DocumentDate,
									   EntryDate = purchaseInvoiceHeader.EntryDate,
									   InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
									   InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
									   StoreId = purchaseInvoiceHeader.StoreId,
									   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
									   TotalValue = purchaseInvoiceHeader.TotalValue,
									   DiscountPercent = purchaseInvoiceHeader.DiscountPercent,
									   DiscountValue = purchaseInvoiceHeader.DiscountValue,
									   TotalItemDiscount = purchaseInvoiceHeader.TotalItemDiscount,
									   GrossValue = purchaseInvoiceHeader.GrossValue,
									   VatValue = purchaseInvoiceHeader.VatValue,
									   SubNetValue = purchaseInvoiceHeader.SubNetValue,
									   OtherTaxValue = purchaseInvoiceHeader.OtherTaxValue,
									   NetValue = purchaseInvoiceHeader.NetValue,
									   TotalInvoiceExpense = purchaseInvoiceHeader.TotalInvoiceExpense,
									   CostValue = purchaseInvoiceHeader.TotalCostValue,
									   CompanyId = company.CompanyId,
									   CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
									   BranchId = branch.BranchId,
									   BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,

                                       ContractDate = supplier.ContractDate,
							           TaxCode = supplier.TaxCode,
							           ShipmentTypeId = supplier.ShipmentTypeId,
							           ShipmentTypeName = supplier.ShipmentTypeName,
							           CountryId = supplier.CountryId,
							           CountryName = supplier.CountryName,
							           StateId = supplier.StateId,
							           StateName = supplier.StateName,
							           CityId = supplier.CityId,
							           CityName = supplier.CityName,
							           DistrictId = supplier.DistrictId,
							           DistrictName = supplier.DistrictName,
							           PostalCode = supplier.PostalCode,
							           BuildingNumber = supplier.BuildingNumber,
							           CommercialRegister = supplier.CommercialRegister,
							           Street1 = supplier.Street1,
							           Street2 = supplier.Street2,
							           AdditionalNumber = supplier.AdditionalNumber,
							           CountryCode = supplier.CountryCode,
							           Address1 = supplier.Address1,
							           Address2 = supplier.Address2,
							           Address3 = supplier.Address3,
							           Address4 = supplier.Address4,
							           FirstResponsibleName = supplier.FirstResponsibleName,
							           FirstResponsiblePhone = supplier.FirstResponsiblePhone,
							           FirstResponsibleEmail = supplier.FirstResponsibleEmail,
							           SecondResponsibleName = supplier.SecondResponsibleName,
							           SecondResponsiblePhone = supplier.SecondResponsiblePhone,
							           SecondResponsibleEmail = supplier.SecondResponsibleEmail,
							           IsCredit = supplier.IsCredit,
							           IsActive = supplier.IsActive,
							           IsActiveName = supplier.IsActiveName,
							           InActiveReasons = supplier.InActiveReasons,

					                   CreatedAt = purchaseInvoiceHeader.CreatedAt,
					                   UserNameCreated = purchaseInvoiceHeader.UserNameCreated,
					                   ModifiedAt = purchaseInvoiceHeader.ModifiedAt,
					                   UserNameModified = purchaseInvoiceHeader.UserNameModified,
								   };

			var purchaseInvoiceReturns = from supplier in _supplierService.GetAllSuppliers()
										 from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.SupplierId == supplier.SupplierId)
										 from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId)
										 from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceReturnHeader.MenuCode).DefaultIfEmpty()
										 from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceReturnHeader.StoreId)
										 from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
										 from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
								         from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == purchaseInvoiceHeader.InvoiceTypeId)
										 where (fromDate == null || purchaseInvoiceReturnHeader.DocumentDate >= fromDate) &&
											   (toDate == null || purchaseInvoiceReturnHeader.DocumentDate <= toDate) &&
										       (supplierId == null || purchaseInvoiceReturnHeader.SupplierId == supplierId) &&
											   storeIds.Contains(store.StoreId)
										 select new PurchaseTradingActivityReportDto
										 {
											 HeaderId = purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId,
											 DocumentFullCode = purchaseInvoiceReturnHeader.Prefix + purchaseInvoiceReturnHeader.DocumentCode + purchaseInvoiceReturnHeader.Suffix,
											 SupplierId = purchaseInvoiceReturnHeader.SupplierId,
											 SupplierCode = supplier.SupplierCode,
											 SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
											 AccountId = supplier.AccountId,
											 AccountCode = supplier.AccountCode,
									         AccountName = supplier.AccountName,
											 Reference = purchaseInvoiceReturnHeader.Reference,
											 RemarksAr = purchaseInvoiceReturnHeader.RemarksAr,
											 RemarksEn = purchaseInvoiceReturnHeader.RemarksEn,
											 MenuCode = purchaseInvoiceReturnHeader.MenuCode,
											 MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
											 DocumentDate = purchaseInvoiceReturnHeader.DocumentDate,
											 EntryDate = purchaseInvoiceReturnHeader.EntryDate,
									         InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
									         InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
											 StoreId = purchaseInvoiceReturnHeader.StoreId,
											 StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
											 TotalValue = -purchaseInvoiceReturnHeader.TotalValue,
											 DiscountPercent = purchaseInvoiceReturnHeader.DiscountPercent,
											 DiscountValue = -purchaseInvoiceReturnHeader.DiscountValue,
											 TotalItemDiscount = -purchaseInvoiceReturnHeader.TotalItemDiscount,
											 GrossValue = -purchaseInvoiceReturnHeader.GrossValue,
											 VatValue = -purchaseInvoiceReturnHeader.VatValue,
											 SubNetValue = -purchaseInvoiceReturnHeader.SubNetValue,
											 OtherTaxValue = -purchaseInvoiceReturnHeader.OtherTaxValue,
											 NetValue = -purchaseInvoiceReturnHeader.NetValue,
											 TotalInvoiceExpense = 0.0M,
											 CostValue = -purchaseInvoiceReturnHeader.TotalCostValue,
											 CompanyId = company.CompanyId,
											 CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
											 BranchId = branch.BranchId,
											 BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,

                                             ContractDate = supplier.ContractDate,
							                 TaxCode = supplier.TaxCode,
							                 ShipmentTypeId = supplier.ShipmentTypeId,
							                 ShipmentTypeName = supplier.ShipmentTypeName,
							                 CountryId = supplier.CountryId,
							                 CountryName = supplier.CountryName,
							                 StateId = supplier.StateId,
							                 StateName = supplier.StateName,
							                 CityId = supplier.CityId,
							                 CityName = supplier.CityName,
							                 DistrictId = supplier.DistrictId,
							                 DistrictName = supplier.DistrictName,
							                 PostalCode = supplier.PostalCode,
							                 BuildingNumber = supplier.BuildingNumber,
							                 CommercialRegister = supplier.CommercialRegister,
							                 Street1 = supplier.Street1,
							                 Street2 = supplier.Street2,
							                 AdditionalNumber = supplier.AdditionalNumber,
							                 CountryCode = supplier.CountryCode,
							                 Address1 = supplier.Address1,
							                 Address2 = supplier.Address2,
							                 Address3 = supplier.Address3,
							                 Address4 = supplier.Address4,
							                 FirstResponsibleName = supplier.FirstResponsibleName,
							                 FirstResponsiblePhone = supplier.FirstResponsiblePhone,
							                 FirstResponsibleEmail = supplier.FirstResponsibleEmail,
							                 SecondResponsibleName = supplier.SecondResponsibleName,
							                 SecondResponsiblePhone = supplier.SecondResponsiblePhone,
							                 SecondResponsibleEmail = supplier.SecondResponsibleEmail,
							                 IsCredit = supplier.IsCredit,
							                 IsActive = supplier.IsActive,
							                 IsActiveName = supplier.IsActiveName,
							                 InActiveReasons = supplier.InActiveReasons,

					                         CreatedAt = purchaseInvoiceReturnHeader.CreatedAt,
					                         UserNameCreated = purchaseInvoiceReturnHeader.UserNameCreated,
					                         ModifiedAt = purchaseInvoiceReturnHeader.ModifiedAt,
					                         UserNameModified = purchaseInvoiceReturnHeader.UserNameModified,
										 };

			var supplierCreditMemos = from supplier in _supplierService.GetAllSuppliers()
									  from supplierCreditMemo in _supplierCreditMemoService.GetAll().Where(x => x.SupplierId == supplier.SupplierId)
								      from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == supplierCreditMemo.JournalHeaderId && x.IsTax)
								      from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									  from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.SupplierCreditMemo).DefaultIfEmpty()
									  from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierCreditMemo.PurchaseInvoiceHeaderId)
									  from store in _storeService.GetAll().Where(x => x.StoreId == supplierCreditMemo.StoreId)
									  from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
									  from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
									  from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == purchaseInvoiceHeader.InvoiceTypeId)
									  where (fromDate == null || supplierCreditMemo.DocumentDate >= fromDate) &&
											(toDate == null || supplierCreditMemo.DocumentDate <= toDate) &&
											(supplierId == null || supplierCreditMemo.SupplierId == supplierId) &&
											storeIds.Contains(store.StoreId)
									  select new PurchaseTradingActivityReportDto
									  {
										  HeaderId = supplierCreditMemo.PurchaseInvoiceHeaderId,
										  DocumentFullCode = supplierCreditMemo.Prefix + supplierCreditMemo.DocumentCode + supplierCreditMemo.Suffix,
										  SupplierId = supplierCreditMemo.SupplierId,
										  SupplierCode = supplier.SupplierCode,
										  SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
										  AccountId = supplier.AccountId,
										  AccountCode = supplier.AccountCode,
									      AccountName = supplier.AccountName,
										  Reference = supplierCreditMemo.Reference,
										  RemarksAr = supplierCreditMemo.RemarksAr,
										  RemarksEn = supplierCreditMemo.RemarksEn,
										  MenuCode = menu.MenuCode,
										  MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
										  DocumentDate = supplierCreditMemo.DocumentDate,
										  EntryDate = supplierCreditMemo.EntryDate,
										  InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
										  InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
										  StoreId = supplierCreditMemo.StoreId,
										  StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
										  TotalValue = memoValueBeforeTaxJournal.DebitValue,
										  DiscountPercent = 0.0M,
										  DiscountValue = 0.0M,
										  TotalItemDiscount = 0.0M,
										  GrossValue = memoValueBeforeTaxJournal.DebitValue,
										  VatValue = taxJournal.DebitValue,
										  SubNetValue = supplierCreditMemo.MemoValue,
										  OtherTaxValue = 0.0M,
										  NetValue = supplierCreditMemo.MemoValue,
										  TotalInvoiceExpense = 0.0M,
										  CostValue = 0.0M,
										  CompanyId = company.CompanyId,
										  CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
										  BranchId = branch.BranchId,
										  BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,

                                          ContractDate = supplier.ContractDate,
							              TaxCode = supplier.TaxCode,
							              ShipmentTypeId = supplier.ShipmentTypeId,
							              ShipmentTypeName = supplier.ShipmentTypeName,
							              CountryId = supplier.CountryId,
							              CountryName = supplier.CountryName,
							              StateId = supplier.StateId,
							              StateName = supplier.StateName,
							              CityId = supplier.CityId,
							              CityName = supplier.CityName,
							              DistrictId = supplier.DistrictId,
							              DistrictName = supplier.DistrictName,
							              PostalCode = supplier.PostalCode,
							              BuildingNumber = supplier.BuildingNumber,
							              CommercialRegister = supplier.CommercialRegister,
							              Street1 = supplier.Street1,
							              Street2 = supplier.Street2,
							              AdditionalNumber = supplier.AdditionalNumber,
							              CountryCode = supplier.CountryCode,
							              Address1 = supplier.Address1,
							              Address2 = supplier.Address2,
							              Address3 = supplier.Address3,
							              Address4 = supplier.Address4,
							              FirstResponsibleName = supplier.FirstResponsibleName,
							              FirstResponsiblePhone = supplier.FirstResponsiblePhone,
							              FirstResponsibleEmail = supplier.FirstResponsibleEmail,
							              SecondResponsibleName = supplier.SecondResponsibleName,
							              SecondResponsiblePhone = supplier.SecondResponsiblePhone,
							              SecondResponsibleEmail = supplier.SecondResponsibleEmail,
							              IsCredit = supplier.IsCredit,
							              IsActive = supplier.IsActive,
							              IsActiveName = supplier.IsActiveName,
							              InActiveReasons = supplier.InActiveReasons,

					                      CreatedAt = supplierCreditMemo.CreatedAt,
					                      UserNameCreated = supplierCreditMemo.UserNameCreated,
					                      ModifiedAt = supplierCreditMemo.ModifiedAt,
					                      UserNameModified = supplierCreditMemo.UserNameModified,
									  };

			var supplierDebitMemos = from supplier in _supplierService.GetAllSuppliers()
									 from supplierDebitMemo in _supplierDebitMemoService.GetAll().Where(x => x.SupplierId == supplier.SupplierId)
								     from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == supplierDebitMemo.JournalHeaderId && x.IsTax)
								     from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									 from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.SupplierDebitMemo).DefaultIfEmpty()
									 from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == supplierDebitMemo.PurchaseInvoiceHeaderId)
									 from store in _storeService.GetAll().Where(x => x.StoreId == supplierDebitMemo.StoreId)
									 from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
									 from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
									 from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == purchaseInvoiceHeader.InvoiceTypeId)
									 where (fromDate == null || supplierDebitMemo.DocumentDate >= fromDate) &&
										   (toDate == null || supplierDebitMemo.DocumentDate <= toDate) &&
										   (supplierId == null || supplierDebitMemo.SupplierId == supplierId) &&
										   storeIds.Contains(store.StoreId)
									 select new PurchaseTradingActivityReportDto
									 {
										 HeaderId = supplierDebitMemo.PurchaseInvoiceHeaderId,
										 DocumentFullCode = supplierDebitMemo.Prefix + supplierDebitMemo.DocumentCode + supplierDebitMemo.Suffix,
										 SupplierId = supplierDebitMemo.SupplierId,
										 SupplierCode = supplier.SupplierCode,
										 SupplierName = language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn,
										 AccountId = supplier.AccountId,
										 AccountCode = supplier.AccountCode,
									     AccountName = supplier.AccountName,
										 Reference = supplierDebitMemo.Reference,
										 RemarksAr = supplierDebitMemo.RemarksAr,
										 RemarksEn = supplierDebitMemo.RemarksEn,
										 MenuCode = menu.MenuCode,
										 MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
										 DocumentDate = supplierDebitMemo.DocumentDate,
										 EntryDate = supplierDebitMemo.EntryDate,
										 InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
										 InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
										 StoreId = supplierDebitMemo.StoreId,
										 StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
										 TotalValue = -memoValueBeforeTaxJournal.CreditValue,
										 DiscountPercent = 0.0M,
										 DiscountValue = 0.0M,
										 TotalItemDiscount = 0.0M,
										 GrossValue = -memoValueBeforeTaxJournal.CreditValue,
										 VatValue = -taxJournal.CreditValue,
										 SubNetValue = -supplierDebitMemo.MemoValue,
										 OtherTaxValue = 0.0M,
										 NetValue = -supplierDebitMemo.MemoValue,
										 TotalInvoiceExpense = 0.0M,
										 CostValue = 0.0M,
										 CompanyId = company.CompanyId,
										 CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
										 BranchId = branch.BranchId,
										 BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,

                                         ContractDate = supplier.ContractDate,
							             TaxCode = supplier.TaxCode,
							             ShipmentTypeId = supplier.ShipmentTypeId,
							             ShipmentTypeName = supplier.ShipmentTypeName,
							             CountryId = supplier.CountryId,
							             CountryName = supplier.CountryName,
							             StateId = supplier.StateId,
							             StateName = supplier.StateName,
							             CityId = supplier.CityId,
							             CityName = supplier.CityName,
							             DistrictId = supplier.DistrictId,
							             DistrictName = supplier.DistrictName,
							             PostalCode = supplier.PostalCode,
							             BuildingNumber = supplier.BuildingNumber,
							             CommercialRegister = supplier.CommercialRegister,
							             Street1 = supplier.Street1,
							             Street2 = supplier.Street2,
							             AdditionalNumber = supplier.AdditionalNumber,
							             CountryCode = supplier.CountryCode,
							             Address1 = supplier.Address1,
							             Address2 = supplier.Address2,
							             Address3 = supplier.Address3,
							             Address4 = supplier.Address4,
							             FirstResponsibleName = supplier.FirstResponsibleName,
							             FirstResponsiblePhone = supplier.FirstResponsiblePhone,
							             FirstResponsibleEmail = supplier.FirstResponsibleEmail,
							             SecondResponsibleName = supplier.SecondResponsibleName,
							             SecondResponsiblePhone = supplier.SecondResponsiblePhone,
							             SecondResponsibleEmail = supplier.SecondResponsibleEmail,
							             IsCredit = supplier.IsCredit,
							             IsActive = supplier.IsActive,
							             IsActiveName = supplier.IsActiveName,
							             InActiveReasons = supplier.InActiveReasons,

					                     CreatedAt = supplierDebitMemo.CreatedAt,
					                     UserNameCreated = supplierDebitMemo.UserNameCreated,
					                     ModifiedAt = supplierDebitMemo.ModifiedAt,
					                     UserNameModified = supplierDebitMemo.UserNameModified,
									 };

			return purchaseInvoices.Concat(purchaseInvoiceReturns).Concat(supplierCreditMemos).Concat(supplierDebitMemos).OrderBy(x => x.DocumentDate).ThenBy(x => x.EntryDate).ThenBy(x => x.MenuCode).ThenBy(x => x.HeaderId).AsNoTracking();
		}
	}
}
