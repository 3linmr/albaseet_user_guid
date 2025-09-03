using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Contracts.Reports.Clients;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.Service.Services.InvoiceSettlement;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Compound.Service.Services.Reports.Clients
{
    public class LowProfitSalesInvoicesReportService : ILowProfitSalesInvoicesReportService
    {
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesValueService _salesValueService;
        private readonly IStoreService _storeService;
        private readonly IMenuService _menuService;
        private readonly ISellerService _sellerService;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IAccountService _accountService;

        public LowProfitSalesInvoicesReportService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IClientService clientService, IHttpContextAccessor httpContextAccessor, ISalesValueService salesValueService, IStoreService storeService, IMenuService menuService, ISellerService sellerService, IBranchService branchService, ICompanyService companyService,IAccountService accountService)
        {
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
            _salesValueService = salesValueService;
            _storeService = storeService;
            _menuService = menuService;
            _sellerService = sellerService;
            _branchService = branchService;
            _companyService = companyService;
            _accountService = accountService;
        }

        public IQueryable<LowProfitSalesInvoicesReportDto> GetLowProfitInvoicesReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, decimal profitThreshold)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var today = DateHelper.GetDateTimeNow().Date;

            return from client in _clientService.GetAllClients()
                   from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ClientId == client.ClientId && storeIds.Contains(x.StoreId) && (toDate == null || x.DueDate <= toDate) && (fromDate == null || x.DueDate >= fromDate) && (clientId == null || x.ClientId == clientId) && (sellerId == null || x.SellerId == sellerId))
                   from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode)
                   from salesInvoiceValue in _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
                   from account in _accountService.GetAll().Where(x => x.AccountId == client.AccountId)
                   from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
                   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
                   from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
                   from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceHeader.SellerId).DefaultIfEmpty()
                   where salesInvoiceValue.OverallGrossValue > 0 && salesInvoiceValue.OverallProfitPercent < profitThreshold
                   orderby salesInvoiceHeader.DocumentDate, salesInvoiceHeader.EntryDate
				   select new LowProfitSalesInvoicesReportDto
                   {
                       SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
                       DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
                       MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
                       MenuCode = salesInvoiceHeader.MenuCode,
                       DocumentDate = salesInvoiceHeader.DocumentDate,
                       EntryDate = salesInvoiceHeader.EntryDate,
                       AccountId = account.AccountId,
                       AccountCode = account.AccountCode,
                       AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
                       ClientId = salesInvoiceHeader.ClientId,
                       ClientCode = client.ClientCode,
                       ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
                       BranchName = store.Branch != null ? language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn : null,
                       CompanyName = store.Branch != null ? language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn : null,
                       SellerCode = seller != null ? seller.SellerCode : null,
                       SellerId = seller != null ? seller.SellerId : 0,
                       SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : null,
                       StoreId = salesInvoiceHeader.StoreId,
                       StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

                       InvoiceValueBeforeTax = salesInvoiceValue.OverallGrossValue,
                       TaxValue = salesInvoiceValue.OverallTaxValue,
                       InvoiceValueAfterTax = salesInvoiceValue.OverallNetValue,
                       CostValue = salesInvoiceValue.OverallCostValue,
                       DiscountValue = salesInvoiceValue.OverallDiscount,
                       Profit = salesInvoiceValue.OverallProfit,
                       ProfitPercent = salesInvoiceValue.OverallProfitPercent,

                       Reference = salesInvoiceHeader.Reference,
                       RemarksAr = salesInvoiceHeader.RemarksAr,
                       RemarksEn = salesInvoiceHeader.RemarksEn,

					   ContractDate = client.ContractDate,
					   CreditLimitDays = client.CreditLimitDays,
					   DebitLimitDays = client.DebitLimitDays,
					   CreditLimitValues = client.CreditLimitValues,
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

                       CreatedAt = salesInvoiceHeader.CreatedAt,
                       ModifiedAt = salesInvoiceHeader.ModifiedAt,
                       UserNameModified = salesInvoiceHeader.UserNameModified,
                       UserNameCreated = salesInvoiceHeader.UserNameCreated,
                   };
        }
    }
}
