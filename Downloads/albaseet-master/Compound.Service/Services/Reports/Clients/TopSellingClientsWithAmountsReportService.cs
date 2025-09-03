using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Compound.CoreOne.Contracts.Reports.Clients;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Contracts.Reports.Shared;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Accounts;

namespace Compound.Service.Services.Reports.Clients
{
    public class TopSellingClientsWithAmountsReportService: ITopSellingClientsWithAmountsReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly IStoreService _storeService;
        private readonly IBranchService _branchService;
        private readonly IClientService _clientService;
       private readonly ITotalClientSalesService _totalClientSales;
        private readonly IClientBalanceService _clientBalanceService;
        public TopSellingClientsWithAmountsReportService(IHttpContextAccessor httpContextAccessor, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, IStoreService storeService, IBranchService branchService, IClientService clientService, ITotalClientSalesService totalClientSales, IClientBalanceService clientBalanceService)
        {
            _httpContextAccessor = httpContextAccessor;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _clientCreditMemoService = clientCreditMemoService;
            _clientDebitMemoService = clientDebitMemoService;
            _storeService = storeService;
            _branchService = branchService;
            _clientService = clientService;
            _totalClientSales = totalClientSales;
            _clientBalanceService = clientBalanceService;
        }
        public IQueryable<TopSellingClientsWithAmountsReportDto> GetTopSellingClientsWithAmountsReport(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return from client in _clientService.GetAllClients().Where(x => x.CompanyId == companyId)
                   from balance in _clientBalanceService.GetClientBalanceService(toDate, false).Where(x => x.AccountId == client.AccountId).DefaultIfEmpty()
                   from sales in _totalClientSales.GetTotalClientSales(fromDate, toDate, true).Where(x => x.ClientId == client.ClientId).DefaultIfEmpty()
                   orderby ((sales.GrossValue != 0 && sales.GrossValue != null) ? sales.GrossValue:0) descending
                   select new TopSellingClientsWithAmountsReportDto
                   {
				       ClientId = client.ClientId,
				       ClientCode = client.ClientCode,
					   ClientName = (language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn) + (sales.CashClientName != null ? " (" + sales.CashClientName + ")" : null),

					   ClientBalance = balance.Balance ?? 0,

				       GrossValue = (decimal?)sales.GrossValue ?? 0,
				       CostValue = (decimal?)sales.CostValue ?? 0,
				       Profit = ((decimal?)sales.GrossValue ?? 0) - ((decimal?)sales.CostValue ?? 0),
				       ProfitPercent = (sales.CostValue != 0 && sales.CostValue != null) ? (sales.GrossValue - sales.CostValue) / sales.CostValue * 100 : 0,
					   InvoiceCount = sales.InvoiceCount ?? 0,

					   ContractDate = client.ContractDate,
					   AccountId = client.AccountId,
					   AccountCode = client.AccountCode,
					   AccountName = client.AccountName,
					   CreditLimitDays = client.CreditLimitDays,
					   DebitLimitDays = client.DebitLimitDays,
					   CreditLimitValues = client.CreditLimitValues,
					   SellerId = client.SellerId,
					   SellerCode = client.SellerCode,
					   SellerName = client.SellerName,
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

					   CreatedAt = client.CreatedAt,
					   UserNameCreated = client.UserNameCreated,
					   ModifiedAt = client.ModifiedAt,
					   UserNameModified = client.UserNameModified,
                   };
        }
    }
}
