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
using Shared.CoreOne.Contracts.Accounts;

namespace Compound.Service.Services.Reports.Clients
{
	public class UnSettledOrSettledSalesInvoiceReportService: IUnSettledOrSettledSalesInvoicesReportService
    {
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IClientService _clientService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesValueService _salesValueService;
		private readonly IStoreService _storeService;
		private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;
		private readonly IMenuService _menuService;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly ISellerService _sellerService;
		private readonly IAccountService _accountService;

        public UnSettledOrSettledSalesInvoiceReportService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IClientService clientService, IHttpContextAccessor httpContextAccessor, ISalesValueService salesValueService, IStoreService storeService, ISalesInvoiceSettlementService salesInvoiceSettlementService, IMenuService menuService,IBranchService branchService,ICompanyService companyService,ISellerService sellerService, IAccountService accountService)
		{
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_clientService = clientService;
			_httpContextAccessor = httpContextAccessor;
			_salesValueService = salesValueService;
			_storeService = storeService;
			_salesInvoiceSettlementService = salesInvoiceSettlementService;
			_menuService = menuService;
			_branchService = branchService;
            _companyService = companyService;
			_sellerService = sellerService;
			_accountService = accountService;
		}

		public IQueryable<UnSettledOrSettledSalesInvoicesReportDto> GetUnSettledOrSettledSalesInvoicesReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, bool getSettled)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var today = DateHelper.GetDateTimeNow().Date;

			return from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && x.CreditPayment && x.IsSettlementCompleted == getSettled && (toDate == null || x.DueDate <= toDate) && (fromDate == null || x.DueDate >= fromDate) && (clientId == null || x.ClientId == clientId) && (sellerId == null || x.SellerId == sellerId))
				   from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
				   from salesInvoiceValue in _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
				   from salesInvoiceSettleValue in _salesInvoiceSettlementService.GetSalesInvoiceSettledValues().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
				   from client in _clientService.GetAll().Where(x => x.ClientId == salesInvoiceHeader.ClientId)
				   from account in _accountService.GetAll().Where(x => x.AccountId == client.AccountId)
				   from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
				   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
				   from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
				   from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceHeader.SellerId).DefaultIfEmpty()
                   where salesInvoiceValue.OverallNetValue > 0
				   orderby salesInvoiceHeader.DocumentDate, salesInvoiceHeader.EntryDate
				   select new UnSettledOrSettledSalesInvoicesReportDto
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
                       InvoiceValue = salesInvoiceValue.OverallNetValue,
					   SettledValue = salesInvoiceSettleValue.SettledValue ?? 0,
					   RemainingValue = salesInvoiceValue.OverallNetValue - (salesInvoiceSettleValue.SettledValue ?? 0),
					   BranchName = branch != null ?( language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn):null,
					   CompanyName = company!=null?( language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn):null,
					   StoreId =seller!=null? salesInvoiceHeader.StoreId:0,
					   StoreName =store!=null?( language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn):null,
					   SellerId = salesInvoiceHeader.SellerId,
					   SellerCode = seller.SellerCode,
					   SellerName = store != null ?( language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn):null,
					   UserNameCreated = salesInvoiceHeader.UserNameCreated,
					   CreatedAt = salesInvoiceHeader.CreatedAt,
					   UserNameModified = salesInvoiceHeader.UserNameModified,
					   ModifiedAt = salesInvoiceHeader.ModifiedAt,
					   DueDate = salesInvoiceHeader.DueDate,
					   Age = Math.Max(EF.Functions.DateDiffDay(salesInvoiceHeader.DueDate, today) ?? 0, 0),
					   Reference = salesInvoiceHeader.Reference,
					   RemarksAr = salesInvoiceHeader.RemarksAr,
					   RemarksEn = salesInvoiceHeader.RemarksEn,
					   RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, salesInvoiceHeader.DueDate) ?? 0, 0),
					   InvoiceDuration = EF.Functions.DateDiffDay(salesInvoiceHeader.DocumentDate, salesInvoiceHeader.DueDate) ?? 0,
				   };
		}
	}
}
