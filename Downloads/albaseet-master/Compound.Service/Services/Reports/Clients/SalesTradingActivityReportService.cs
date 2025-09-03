using Compound.CoreOne.Contracts.Reports.Clients;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.CoreOne.Models.Dtos.Reports.Suppliers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service.Services.Basics;
using Shared.Service.Services.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Compound.Service.Services.Reports.Clients
{
    public class SalesTradingActivityReportService : ISalesTradingActivityReportService
    {
        private readonly ISalesInvoiceHeaderService _SalesInvoiceHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesValueService _SalesValueService;
        private readonly IStoreService _storeService;
        private readonly IMenuService _menuService;
        private readonly  IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IAccountService _accountService;
        private readonly IInvoiceTypeService _invoiceTypeService;
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly ISellerService _sellerService;
        private readonly IJournalDetailService _journalDetailService;
        public SalesTradingActivityReportService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IClientService clientService, IHttpContextAccessor httpContextAccessor, ISalesValueService salesValueService, IStoreService storeService, IMenuService menuService,
            ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IBranchService branchService, ICompanyService companyService, IAccountService accountService, IInvoiceTypeService invoiceTypeService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService
            ,ISellerService sellerService, IJournalDetailService journalDetailService)
        {
            _SalesInvoiceHeaderService = salesInvoiceHeaderService;
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
            _SalesValueService = salesValueService;
            _storeService = storeService;
            _menuService = menuService;
            _branchService = branchService;
            _companyService = companyService;
            _accountService = accountService;
            _invoiceTypeService = invoiceTypeService;
            _salesInvoiceReturnHeaderService= salesInvoiceReturnHeaderService;
            _clientCreditMemoService = clientCreditMemoService;
            _clientDebitMemoService = clientDebitMemoService;
            _sellerService = sellerService;
			_journalDetailService = journalDetailService;

        }

        public IQueryable<SalesTradingActivityReportDto> GetSalesTradingActivityReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId)
        {

            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var today = DateHelper.GetDateTimeNow().Date;

			var salesInvoices = from client in _clientService.GetAllClients()
								from salesInvoiceHeader in _SalesInvoiceHeaderService.GetAll().Where(x => x.ClientId == client.ClientId)
								from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
								from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
								from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
								from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
								from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == salesInvoiceHeader.InvoiceTypeId)
								from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceHeader.SellerId).DefaultIfEmpty()
								where (fromDate == null || salesInvoiceHeader.DocumentDate >= fromDate) &&
							          (toDate == null || salesInvoiceHeader.DocumentDate <= toDate) &&
									  (clientId == null || salesInvoiceHeader.ClientId == clientId) &&
									  (sellerId == null || salesInvoiceHeader.SellerId == sellerId) &&
									  storeIds.Contains(store.StoreId)
								select new SalesTradingActivityReportDto
								{
									HeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
									DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
									AccountId = client.AccountId,
									AccountCode = client.AccountCode,
									AccountName = client.AccountName,
									ClientId = salesInvoiceHeader.ClientId,
									ClientCode = client.ClientCode,
									ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
									Reference = salesInvoiceHeader.Reference,
									RemarksAr = salesInvoiceHeader.RemarksAr,
									RemarksEn = salesInvoiceHeader.RemarksEn,
									MenuCode = salesInvoiceHeader.MenuCode,
									MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
									DocumentDate = salesInvoiceHeader.DocumentDate,
									EntryDate = salesInvoiceHeader.EntryDate,
									InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
									InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
									StoreId = salesInvoiceHeader.StoreId,
									StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
									TotalValue = salesInvoiceHeader.TotalValue,
									DiscountPercent = salesInvoiceHeader.DiscountPercent,
									DiscountValue = salesInvoiceHeader.DiscountValue,
									TotalItemDiscount = salesInvoiceHeader.TotalItemDiscount,
									GrossValue = salesInvoiceHeader.GrossValue,
									VatValue = salesInvoiceHeader.VatValue,
									SubNetValue = salesInvoiceHeader.SubNetValue,
									OtherTaxValue = salesInvoiceHeader.OtherTaxValue,
									NetValue = salesInvoiceHeader.NetValue,
									CostValue = salesInvoiceHeader.TotalCostValue,
									ProfitValue = salesInvoiceHeader.GrossValue - salesInvoiceHeader.TotalCostValue,
									ProfitPercent = salesInvoiceHeader.TotalCostValue != 0 ? ((salesInvoiceHeader.GrossValue - salesInvoiceHeader.TotalCostValue) / salesInvoiceHeader.TotalCostValue) * 100 : 0,
									CompanyId = company.CompanyId,
									CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
									BranchId = branch.BranchId,
									BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
									SellerId = salesInvoiceHeader.SellerId,
									SellerCode = seller.SellerCode,
									sellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,

									ContractDate = client.ContractDate,
									TaxCode = client.TaxCode,
									CountryId = client.CountryId,
									CountryName = client.CountryName,
									StateId = client.StateId,
									StateName = client.StateName,
									CityId = client.CityId,
									CityName = client.CityName,
									DistrictId = client.DistrictId,
									DistrictName = client.DistrictName,
									PostalCode = client.PostalCode,
									BuildingNumber = client.BuildingNumber,
									CommercialRegister = client.CommercialRegister,
									ClientStreet1 = client.Street1,
									ClientStreet2 = client.Street2,
									ClientAdditionalNumber = client.AdditionalNumber,
									CountryCode = client.CountryCode,
									ClientAddress1 = client.Address1,
									ClientAddress2 = client.Address2,
									ClientAddress3 = client.Address3,
									ClientAddress4 = client.Address4,
									FirstResponsibleName = client.FirstResponsibleName,
									FirstResponsiblePhone = client.FirstResponsiblePhone,
									FirstResponsibleEmail = client.FirstResponsibleEmail,
									SecondResponsibleName = client.SecondResponsibleName,
									SecondResponsiblePhone = client.SecondResponsiblePhone,
									SecondResponsibleEmail = client.SecondResponsibleEmail,
									IsCredit = client.IsCredit,
									IsActive = client.IsActive,
									InActiveReasons = client.InActiveReasons,
									IsActiveName = client.IsActiveName,

									CreatedAt = salesInvoiceHeader.CreatedAt,
									UserNameCreated = salesInvoiceHeader.UserNameCreated,
									ModifiedAt = salesInvoiceHeader.ModifiedAt,
									UserNameModified = salesInvoiceHeader.UserNameModified,
								};

			var salesInvoiceReturns = from client in _clientService.GetAllClients()
									  from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.ClientId == client.ClientId)
									  from salesInvoiceHeader in _SalesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceReturnHeader.SalesInvoiceHeaderId)
									  from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceReturnHeader.MenuCode).DefaultIfEmpty()
									  from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceReturnHeader.StoreId)
									  from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
									  from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
									  from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == salesInvoiceHeader.InvoiceTypeId)
									  from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceReturnHeader.SellerId).DefaultIfEmpty()
									  where (fromDate == null || salesInvoiceReturnHeader.DocumentDate >= fromDate) &&
											(toDate == null || salesInvoiceReturnHeader.DocumentDate <= toDate) &&
											(clientId == null || salesInvoiceReturnHeader.ClientId == clientId) &&
											(sellerId == null || salesInvoiceReturnHeader.SellerId == sellerId) &&
											storeIds.Contains(store.StoreId)
									  select new SalesTradingActivityReportDto
									  {
										  HeaderId = salesInvoiceReturnHeader.SalesInvoiceHeaderId,
										  DocumentFullCode = salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix,
										  ClientId = salesInvoiceReturnHeader.ClientId,
										  ClientCode = client.ClientCode,
										  ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
										  AccountId = client.AccountId,
										  AccountCode = client.AccountCode,
										  AccountName = client.AccountName,
										  Reference = salesInvoiceReturnHeader.Reference,
										  RemarksAr = salesInvoiceReturnHeader.RemarksAr,
										  RemarksEn = salesInvoiceReturnHeader.RemarksEn,
										  MenuCode = salesInvoiceReturnHeader.MenuCode,
										  MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
										  DocumentDate = salesInvoiceReturnHeader.DocumentDate,
										  EntryDate = salesInvoiceReturnHeader.EntryDate,
										  InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
										  InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
										  StoreId = salesInvoiceReturnHeader.StoreId,
										  StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
										  TotalValue = -salesInvoiceReturnHeader.TotalValue,
										  DiscountPercent = salesInvoiceReturnHeader.DiscountPercent,
										  DiscountValue = -salesInvoiceReturnHeader.DiscountValue,
										  TotalItemDiscount = -salesInvoiceReturnHeader.TotalItemDiscount,
										  GrossValue = -salesInvoiceReturnHeader.GrossValue,
										  VatValue = -salesInvoiceReturnHeader.VatValue,
										  SubNetValue = -salesInvoiceReturnHeader.SubNetValue,
										  OtherTaxValue = -salesInvoiceReturnHeader.OtherTaxValue,
										  NetValue = -salesInvoiceReturnHeader.NetValue,
										  CostValue = -salesInvoiceReturnHeader.TotalCostValue,
										  ProfitValue = -(salesInvoiceReturnHeader.GrossValue - salesInvoiceReturnHeader.TotalCostValue),
										  ProfitPercent = salesInvoiceReturnHeader.TotalCostValue != 0 ? -((salesInvoiceReturnHeader.GrossValue - salesInvoiceReturnHeader.TotalCostValue) / (salesInvoiceReturnHeader.TotalCostValue)) * 100 : 0,
										  CompanyId = company.CompanyId,
										  CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
										  BranchId = branch.BranchId,
										  BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
									      SellerId = seller.SellerId,
									      SellerCode = seller.SellerCode,
										  sellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,

										  ContractDate = client.ContractDate,
										  TaxCode = client.TaxCode,
										  CountryId = client.CountryId,
										  CountryName = client.CountryName,
										  StateId = client.StateId,
										  StateName = client.StateName,
										  CityId = client.CityId,
										  CityName = client.CityName,
										  DistrictId = client.DistrictId,
										  DistrictName = client.DistrictName,
										  PostalCode = client.PostalCode,
										  BuildingNumber = client.BuildingNumber,
										  CommercialRegister = client.CommercialRegister,
										  ClientStreet1 = client.Street1,
										  ClientStreet2 = client.Street2,
										  ClientAdditionalNumber = client.AdditionalNumber,
										  CountryCode = client.CountryCode,
										  ClientAddress1 = client.Address1,
										  ClientAddress2 = client.Address2,
										  ClientAddress3 = client.Address3,
										  ClientAddress4 = client.Address4,
										  FirstResponsibleName = client.FirstResponsibleName,
										  FirstResponsiblePhone = client.FirstResponsiblePhone,
										  FirstResponsibleEmail = client.FirstResponsibleEmail,
										  SecondResponsibleName = client.SecondResponsibleName,
										  SecondResponsiblePhone = client.SecondResponsiblePhone,
										  SecondResponsibleEmail = client.SecondResponsibleEmail,
										  IsCredit = client.IsCredit,
										  IsActive = client.IsActive,
										  InActiveReasons = client.InActiveReasons,
										  IsActiveName = client.IsActiveName,

									      CreatedAt = salesInvoiceReturnHeader.CreatedAt,
									      UserNameCreated = salesInvoiceReturnHeader.UserNameCreated,
									      ModifiedAt = salesInvoiceReturnHeader.ModifiedAt,
									      UserNameModified = salesInvoiceReturnHeader.UserNameModified,
									  };

			var clientCreditMemos = from client in _clientService.GetAllClients()				   
									from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => x.ClientId == client.ClientId)
								    from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientCreditMemo.JournalHeaderId && x.IsTax)
								    from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ClientCreditMemo).DefaultIfEmpty()
									from salesInvoiceHeader in _SalesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
									from store in _storeService.GetAll().Where(x => x.StoreId == clientCreditMemo.StoreId)
									from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
									from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
									from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == salesInvoiceHeader.InvoiceTypeId)
									from seller in _sellerService.GetAll().Where(x => x.SellerId == clientCreditMemo.SellerId).DefaultIfEmpty()
									where (fromDate == null || clientCreditMemo.DocumentDate >= fromDate) &&
									(toDate == null || clientCreditMemo.DocumentDate <= toDate) &&
									(clientId == null || clientCreditMemo.ClientId == clientId) &&
									(sellerId == null || clientCreditMemo.SellerId == sellerId) &&
									storeIds.Contains(store.StoreId)
									select new SalesTradingActivityReportDto
									{
										HeaderId = clientCreditMemo.SalesInvoiceHeaderId,
										DocumentFullCode = clientCreditMemo.Prefix + clientCreditMemo.DocumentCode + clientCreditMemo.Suffix,
										ClientId = clientCreditMemo.ClientId,
										ClientCode = client.ClientCode,
										ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
										AccountId = client.AccountId,
										AccountCode = client.AccountCode,
										AccountName = client.AccountName,
										Reference = clientCreditMemo.Reference,
										RemarksAr = clientCreditMemo.RemarksAr,
										RemarksEn = clientCreditMemo.RemarksEn,
										MenuCode = menu.MenuCode,
										MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
										DocumentDate = clientCreditMemo.DocumentDate,
										EntryDate = clientCreditMemo.EntryDate,
										InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
										InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
										StoreId = clientCreditMemo.StoreId,
										StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
										TotalValue = -memoValueBeforeTaxJournal.DebitValue,
										DiscountPercent = 0.0M,
										DiscountValue = 0.0M,
										TotalItemDiscount = 0.0M,
										GrossValue = -memoValueBeforeTaxJournal.DebitValue,
										VatValue = -taxJournal.DebitValue,
										SubNetValue = -clientCreditMemo.MemoValue,
										OtherTaxValue = 0.0M,
										NetValue = -clientCreditMemo.MemoValue,
										CostValue = 0.0M,
										ProfitValue = -(memoValueBeforeTaxJournal.DebitValue),
										ProfitPercent = 0.0M,
										CompanyId = company.CompanyId,
										CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
										BranchId = branch.BranchId,
										BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
									    SellerId = seller.SellerId,
									    SellerCode = seller.SellerCode,
										sellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,

										ContractDate = client.ContractDate,
										TaxCode = client.TaxCode,
										CountryId = client.CountryId,
										CountryName = client.CountryName,
										StateId = client.StateId,
										StateName = client.StateName,
										CityId = client.CityId,
										CityName = client.CityName,
										DistrictId = client.DistrictId,
										DistrictName = client.DistrictName,
										PostalCode = client.PostalCode,
										BuildingNumber = client.BuildingNumber,
										CommercialRegister = client.CommercialRegister,
										ClientStreet1 = client.Street1,
										ClientStreet2 = client.Street2,
										ClientAdditionalNumber = client.AdditionalNumber,
										CountryCode = client.CountryCode,
										ClientAddress1 = client.Address1,
										ClientAddress2 = client.Address2,
										ClientAddress3 = client.Address3,
										ClientAddress4 = client.Address4,
										FirstResponsibleName = client.FirstResponsibleName,
										FirstResponsiblePhone = client.FirstResponsiblePhone,
										FirstResponsibleEmail = client.FirstResponsibleEmail,
										SecondResponsibleName = client.SecondResponsibleName,
										SecondResponsiblePhone = client.SecondResponsiblePhone,
										SecondResponsibleEmail = client.SecondResponsibleEmail,
										IsCredit = client.IsCredit,
										IsActive = client.IsActive,
										InActiveReasons = client.InActiveReasons,
										IsActiveName = client.IsActiveName,

									    CreatedAt = clientCreditMemo.CreatedAt,
									    UserNameCreated = clientCreditMemo.UserNameCreated,
									    ModifiedAt = clientCreditMemo.ModifiedAt,
									    UserNameModified = clientCreditMemo.UserNameModified,
									};

			var clientDebitMemos = from client in _clientService.GetAllClients()
								   from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => x.ClientId == client.ClientId)
								   from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientDebitMemo.JournalHeaderId && x.IsTax)
								   from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
								   from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ClientDebitMemo).DefaultIfEmpty()
								   from salesInvoiceHeader in _SalesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
								   from store in _storeService.GetAll().Where(x => x.StoreId == clientDebitMemo.StoreId)
								   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
								   from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
								   from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == salesInvoiceHeader.InvoiceTypeId)
								   from seller in _sellerService.GetAll().Where(x => x.SellerId == clientDebitMemo.SellerId).DefaultIfEmpty()
								   where (fromDate == null || clientDebitMemo.DocumentDate >= fromDate) &&
								   (toDate == null || clientDebitMemo.DocumentDate <= toDate) &&
								   (clientId == null || clientDebitMemo.ClientId == clientId) &&
								   (sellerId == null || clientDebitMemo.SellerId == sellerId) &&
								   storeIds.Contains(store.StoreId)
								   select new SalesTradingActivityReportDto
								   {
									   HeaderId = clientDebitMemo.SalesInvoiceHeaderId,
									   DocumentFullCode = clientDebitMemo.Prefix + clientDebitMemo.DocumentCode + clientDebitMemo.Suffix,
									   ClientId = clientDebitMemo.ClientId,
									   ClientCode = client.ClientId,
									   ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
									   AccountId = client.AccountId,
									   AccountCode = client.AccountCode,
									   AccountName = client.AccountName,
									   Reference = clientDebitMemo.Reference,
									   RemarksAr = clientDebitMemo.RemarksAr,
									   RemarksEn = clientDebitMemo.RemarksEn,
									   MenuCode = menu.MenuCode,
									   MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
									   DocumentDate = clientDebitMemo.DocumentDate,
									   EntryDate = clientDebitMemo.EntryDate,
									   InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
									   InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
									   StoreId = clientDebitMemo.StoreId,
									   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
									   TotalValue = memoValueBeforeTaxJournal.CreditValue,
									   DiscountPercent = 0.0M,
									   DiscountValue = 0.0M,
									   TotalItemDiscount = 0.0M,
									   GrossValue = memoValueBeforeTaxJournal.CreditValue,
									   VatValue = taxJournal.CreditValue,
									   SubNetValue = clientDebitMemo.MemoValue,
									   OtherTaxValue = 0.0M,
									   NetValue = clientDebitMemo.MemoValue,
									   CostValue = 0.0M,
									   ProfitValue = memoValueBeforeTaxJournal.CreditValue,
									   ProfitPercent = 0.0M,
									   CompanyId = company.CompanyId,
									   CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
									   BranchId = branch.BranchId,
									   BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
									   SellerId = seller.SellerId,
									   SellerCode = seller.SellerCode,
									   sellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,

									   ContractDate = client.ContractDate,
									   TaxCode = client.TaxCode,
									   CountryId = client.CountryId,
									   CountryName = client.CountryName,
									   StateId = client.StateId,
									   StateName = client.StateName,
									   CityId = client.CityId,
									   CityName = client.CityName,
									   DistrictId = client.DistrictId,
									   DistrictName = client.DistrictName,
									   PostalCode = client.PostalCode,
									   BuildingNumber = client.BuildingNumber,
									   CommercialRegister = client.CommercialRegister,
									   ClientStreet1 = client.Street1,
									   ClientStreet2 = client.Street2,
									   ClientAdditionalNumber = client.AdditionalNumber,
									   CountryCode = client.CountryCode,
									   ClientAddress1 = client.Address1,
									   ClientAddress2 = client.Address2,
									   ClientAddress3 = client.Address3,
									   ClientAddress4 = client.Address4,
									   FirstResponsibleName = client.FirstResponsibleName,
									   FirstResponsiblePhone = client.FirstResponsiblePhone,
									   FirstResponsibleEmail = client.FirstResponsibleEmail,
									   SecondResponsibleName = client.SecondResponsibleName,
									   SecondResponsiblePhone = client.SecondResponsiblePhone,
									   SecondResponsibleEmail = client.SecondResponsibleEmail,
									   IsCredit = client.IsCredit,
									   IsActive = client.IsActive,
									   InActiveReasons = client.InActiveReasons,
									   IsActiveName = client.IsActiveName,

									   CreatedAt = clientDebitMemo.CreatedAt,
									   UserNameCreated = clientDebitMemo.UserNameCreated,
									   ModifiedAt = clientDebitMemo.ModifiedAt,
									   UserNameModified = clientDebitMemo.UserNameModified,
								   };

			return salesInvoices.Concat(salesInvoiceReturns).Concat(clientCreditMemos).Concat(clientDebitMemos).OrderBy(x => x.DocumentDate).ThenBy(x => x.EntryDate).ThenBy(x => x.MenuCode).ThenBy(x => x.HeaderId).AsNoTracking();
        }


    }
}
