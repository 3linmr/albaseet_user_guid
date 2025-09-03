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
using Shared.CoreOne.Contracts.Menus;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.CoreOne.Contracts.Reports.Clients;
using Sales.CoreOne.Contracts;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Contracts.Accounts;
using Compound.CoreOne.Contracts.InvoiceSettlement;

namespace Compound.Service.Services.Reports.Clients
{
	public class AgeingSalesInvoiceReportService: IAgeingSalesInvoiceReportService
    {
		private readonly ISalesInvoiceHeaderService _SalesInvoiceHeaderService;
		private readonly IClientService _clientService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesValueService _SalesValueService;
		private readonly IStoreService _storeService;
		private readonly IMenuService _menuService;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly ISellerService _sellerService;
		private readonly IAccountService _accountService;
		private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;

        public AgeingSalesInvoiceReportService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IClientService clientService, IHttpContextAccessor httpContextAccessor, ISalesValueService salesValueService, IStoreService storeService, IMenuService menuService,ICompanyService companyService,IBranchService branchService,ISellerService sellerService, IAccountService accountService, ISalesInvoiceSettlementService salesInvoiceSettlementService)
		{
            _SalesInvoiceHeaderService = salesInvoiceHeaderService;
			_clientService = clientService;
			_httpContextAccessor = httpContextAccessor;
            _SalesValueService = salesValueService;
			_storeService = storeService;
			_menuService = menuService;
			_branchService = branchService;
            _companyService = companyService;
			_sellerService = sellerService;
			_accountService = accountService;
			_salesInvoiceSettlementService = salesInvoiceSettlementService;
		}

		public IQueryable<AgeingSalesInvoiceReportDto> GetAgeingSalesInvoiceReport(List<int> storeIds, DateTime? toDate, int? clientId, int? sellerId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow().Date;

			return from salesInvoiceHeader in _SalesInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && x.CreditPayment && x.IsSettlementCompleted == false && (toDate == null || x.DueDate <= toDate) && (clientId == null || x.ClientId == clientId) && (sellerId == null || x.SellerId == sellerId))
				   from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
				   from salesInvoiceValue in _SalesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
                   from settleValue in _salesInvoiceSettlementService.GetSalesInvoiceSettledValues().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
                   from client in _clientService.GetAll().Where(x => x.ClientId == salesInvoiceHeader.ClientId)
                   from account in _accountService.GetAll().Where(x => x.AccountId == client.AccountId)
                   from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
                   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
                   from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
                   from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceHeader.SellerId).DefaultIfEmpty()	
                   where salesInvoiceValue.OverallNetValue > 0
                   orderby salesInvoiceHeader.DocumentDate, salesInvoiceHeader.EntryDate
				   select new AgeingSalesInvoiceReportDto
				   {
					   SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
					   DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
					   MenuCode = salesInvoiceHeader.MenuCode,
					   MenuName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : null,
					   DocumentDate = salesInvoiceHeader.DocumentDate,
					   EntryDate = salesInvoiceHeader.EntryDate,
					   ClientId = salesInvoiceHeader.ClientId,
					   ClientCode = client.ClientCode,
					   ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
                       AccountId = account.AccountId,
                       AccountCode = account.AccountCode,
                       AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
                       BranchName = branch != null ? (language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn):null,
                       CompanyName = branch != null ? (language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn):null,
                       StoreId = salesInvoiceHeader.StoreId,
                       StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                       SellerId = seller != null ? seller.SellerId : null,
					   SellerCode = seller != null ? seller.SellerCode : null,
					   SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : null,
					   CreatedAt = salesInvoiceHeader.CreatedAt,
					   UserNameCreated = salesInvoiceHeader.UserNameCreated,
					   ModifiedAt= salesInvoiceHeader.ModifiedAt,
					   UserNameModified = salesInvoiceHeader.UserNameModified,
					   InvoiceValue = salesInvoiceValue.OverallNetValue,
					   DueDate = salesInvoiceHeader.DueDate,
					   Age = Math.Max(EF.Functions.DateDiffDay(salesInvoiceHeader.DueDate, today) ?? 0, 0),
					   RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, salesInvoiceHeader.DueDate) ?? 0, 0),
                       RemainingValue = salesInvoiceValue.OverallNetValue - (settleValue.SettledValue ?? 0),
                       InvoiceDuration = Math.Max(EF.Functions.DateDiffDay(salesInvoiceHeader.DocumentDate, salesInvoiceHeader.DueDate) ?? 0, 0),
                   };
		}
	}
}
