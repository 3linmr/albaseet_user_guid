using Accounting.CoreOne.Contracts;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Contracts.Reports.Clients;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Compound.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.Service.Services.InvoiceSettlement;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service.Services.Accounts;
using Shared.Service.Services.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.Clients
{
    public class ClientsExceedCreditLimitReportService : IClientsExceedCreditLimitReportService
    {
        private readonly IAccountCategoryService _accountCategoryService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IAccountService _accountService;
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAgeingSalesInvoiceReportService _ageingSalesInvoiceReportService;
        private readonly IClientService _clientService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IMenuService _menuService;
        private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
        private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;
        private readonly ISalesValueService _salesValueService;
        private readonly ISellerService _sellerService;
		public ClientsExceedCreditLimitReportService(IAccountCategoryService accountCategoryService, IJournalDetailService journalDetailService, IAccountService accountService, IJournalHeaderService journalHeaderService, IHttpContextAccessor httpContextAccessor,
			IAgeingSalesInvoiceReportService ageingSalesInvoiceReportService, IClientService clientService, ISalesInvoiceHeaderService SalesInvoiceHeaderService, IMenuService menuService, IReceiptVoucherHeaderService receiptVoucherHeaderService, ISalesInvoiceSettlementService salesInvoiceSettlementService, ISalesValueService salesValueService, ISellerService sellerService)
		{
			_accountCategoryService = accountCategoryService;
			_journalDetailService = journalDetailService;
			_accountService = accountService;
			_journalHeaderService = journalHeaderService;
			_httpContextAccessor = httpContextAccessor;
			_ageingSalesInvoiceReportService = ageingSalesInvoiceReportService;
			_clientService = clientService;
			_salesInvoiceHeaderService = SalesInvoiceHeaderService;
			_menuService = menuService;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_salesInvoiceSettlementService = salesInvoiceSettlementService;
			_salesValueService = salesValueService;
			_sellerService = sellerService;
		}

		public IQueryable<ClientsExceedCreditLimitDto> GetClientsExceedCreditLimitReport(int companyId, int? clientId, bool isDay)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return GetClientsExceedCreditLimit(companyId,clientId,isDay);
        }

        private IQueryable<ClientsExceedCreditLimitDto> GetClientsExceedCreditLimit(int companyId, int? clientId, bool isDay)
        {
            var today = DateHelper.GetDateTimeNow();
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var query = from client in _clientService.GetAllClients().Where(x => x.CompanyId == companyId && (clientId == null || x.ClientId == clientId))
                        from account in _accountService.GetAll().Where(x => x.AccountId == client.AccountId)
                        join journalDetail in _journalDetailService.GetAll()on account.AccountId equals journalDetail.AccountId
                        from lastSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ClientId==client.ClientId && x.CreditPayment).OrderByDescending(x=>x.DocumentDate).Take(1).DefaultIfEmpty()
                        from lastUnSettledSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ClientId==client.ClientId && x.CreditPayment && x.IsSettlementCompleted == false).OrderByDescending(x=> x.DocumentDate).Take(1).DefaultIfEmpty()
                        from lastInvoiceValue in _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == lastSalesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
                        from salesInvoiceSettleValue in _salesInvoiceSettlementService.GetSalesInvoiceSettledValues().Where(x => x.SalesInvoiceHeaderId == lastSalesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
                        from menu in _menuService.GetAll().Where(x => x.MenuCode == lastSalesInvoiceHeader.MenuCode).DefaultIfEmpty()
                        from receiptVoucherHeader in _receiptVoucherHeaderService.GetAll().Where(x => x.ClientId == client.ClientId).OrderByDescending(x => x.DocumentDate).Take(1).DefaultIfEmpty()
                        from lastSettlementValue in _salesInvoiceSettlementService.GetReceiptVoucherSettledValues().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeader.ReceiptVoucherHeaderId).DefaultIfEmpty()
                        from seller in _sellerService.GetAll().Where(x=>x.SellerId== lastSalesInvoiceHeader.SellerId).DefaultIfEmpty()
                        group new { journalDetail, lastSalesInvoiceHeader, receiptVoucherHeader} by new
                        {
                            account.AccountId,
                            AccountCode = account.AccountCode ?? "",
                            client.ClientId,
                            client.ClientCode,
                            client.ClientNameAr,
                            client.ClientNameEn,
                            account.AccountLevel,
                            MainAccountId = account.MainAccountId ?? 0,
                            AccountNameAr = account.AccountNameAr ?? "",
                            AccountNameEn = account.AccountNameEn ?? "",
                            CreditLimitValues = client.CreditLimitValues,
                            CreditLimitDays = client.CreditLimitDays,
                            lastInvoiceFullCode = lastSalesInvoiceHeader.Prefix + lastSalesInvoiceHeader.DocumentCode + lastSalesInvoiceHeader.Suffix,
                            lastInvoiceValue = lastInvoiceValue.OverallNetValue,
                            LastInvoiceDate = lastSalesInvoiceHeader.DocumentDate,
                            LastUnSettledDueDate = lastUnSettledSalesInvoiceHeader.DueDate,
                            LastUnSettledInvoiceDate = lastUnSettledSalesInvoiceHeader.DocumentDate,
                            LastReceiptVoucherFullCode = receiptVoucherHeader.Prefix + receiptVoucherHeader.ReceiptVoucherCode + receiptVoucherHeader.Suffix,
                            LastReceiptVoucherValue = lastSettlementValue.SettledValue ?? 0,
                            lastSalesInvoiceHeader.DueDate,
                            sellerNameAr=seller.SellerNameAr,
                            sellerNameEn=seller.SellerNameEn,

							client.ContractDate,
							client.TaxCode,
							client.CountryId,
							client.CountryName,
							client.StateId,
							client.StateName,
							client.CityId,
							client.CityName,
							client.DistrictId,
							client.DistrictName,
							client.PostalCode,
							client.BuildingNumber,
							client.CommercialRegister,
							client.Street1,
							client.Street2,
							client.AdditionalNumber,
							client.CountryCode,
							client.Address1,
							client.Address2,
							client.Address3,
							client.Address4,
							client.FirstResponsibleName,
							client.FirstResponsiblePhone,
							client.FirstResponsibleEmail,
							client.SecondResponsibleName,
							client.SecondResponsiblePhone,
							client.SecondResponsibleEmail,
							client.IsCredit,
							client.IsActive,
							client.InActiveReasons,
							client.IsActiveName,

                            client.CreatedAt,
                            client.UserNameCreated,
                            client.ModifiedAt,
							client.UserNameModified,
						} into g
                        // Filter the results to include only those where the calculated Balance exceeds the CreditLimit
                        where  !isDay ? g.Sum(x => x.journalDetail.DebitValue - x.journalDetail.CreditValue) > g.Key.CreditLimitValues :
                                             (g.Key.LastUnSettledDueDate != null && g.Key.LastUnSettledDueDate.Value.AddDays(g.Key.CreditLimitDays) < today) 
                        select new ClientsExceedCreditLimitDto
                        {
                            AccountId = g.Key.AccountId,
                            AccountName = language == LanguageData.LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,

                            ClientId = g.Key.ClientId,
                            ClientCode = g.Key.ClientCode,
                            ClientName = language == LanguageData.LanguageCode.Arabic ? g.Key.ClientNameAr : g.Key.ClientNameEn,

                            LastInvoiceFullCode = g.Key.lastInvoiceFullCode,
                            LastInvoiceDate = g.Key.LastInvoiceDate,
                            LastInvoiceValue = g.Key.lastInvoiceValue,

                            LastReceiptVoucherFullCode = g.Key.LastReceiptVoucherFullCode,
                            LastReceiptVoucherValue = g.Key.LastReceiptVoucherValue,

                            Balance = g.Sum(x => x.journalDetail.DebitValue - x.journalDetail.CreditValue),
                            CreditLimitValues = g.Key.CreditLimitValues,
                            OverdueAmount = g.Sum(x => x.journalDetail.DebitValue - x.journalDetail.CreditValue) - g.Key.CreditLimitValues,

                            InvoiceDuration = EF.Functions.DateDiffDay(g.Key.LastUnSettledInvoiceDate, g.Key.LastUnSettledDueDate),
                            InvoiceDueDate=g.Key.LastUnSettledDueDate,
                            CreditLimitDays = g.Key.CreditLimitDays,
                            InvoiceDueDateFinal = g.Key.LastUnSettledDueDate.HasValue? g.Key.LastUnSettledDueDate.Value.AddDays(g.Key.CreditLimitDays) : null,
                            DaysOverdue = g.Key.LastUnSettledDueDate.HasValue ? Math.Max(EF.Functions.DateDiffDay(g.Key.LastUnSettledDueDate.Value.AddDays(g.Key.CreditLimitDays),today),0) : 0,

                            SellerName= language == LanguageData.LanguageCode.Arabic ? g.Key.sellerNameAr : g.Key.sellerNameEn,

							ContractDate = g.Key.ContractDate,
							TaxCode = g.Key.TaxCode,
							CountryId = g.Key.CountryId,
							CountryName = g.Key.CountryName,
							StateId = g.Key.StateId,
							StateName = g.Key.StateName,
							CityId = g.Key.CityId,
							CityName = g.Key.CityName,
							DistrictId = g.Key.DistrictId,
							DistrictName = g.Key.DistrictName,
							PostalCode = g.Key.PostalCode,
							BuildingNumber = g.Key.BuildingNumber,
							CommercialRegister = g.Key.CommercialRegister,
							ClientStreet1 = g.Key.Street1,
							ClientStreet2 = g.Key.Street2,
							ClientAdditionalNumber = g.Key.AdditionalNumber,
							CountryCode = g.Key.CountryCode,
							ClientAddress1 = g.Key.Address1,
							ClientAddress2 = g.Key.Address2,
							ClientAddress3 = g.Key.Address3,
							ClientAddress4 = g.Key.Address4,
							FirstResponsibleName = g.Key.FirstResponsibleName,
							FirstResponsiblePhone = g.Key.FirstResponsiblePhone,
							FirstResponsibleEmail = g.Key.FirstResponsibleEmail,
							SecondResponsibleName = g.Key.SecondResponsibleName,
							SecondResponsiblePhone = g.Key.SecondResponsiblePhone,
							SecondResponsibleEmail = g.Key.SecondResponsibleEmail,
							IsCredit = g.Key.IsCredit,
							IsActive = g.Key.IsActive,
							InActiveReasons = g.Key.InActiveReasons,
							IsActiveName = g.Key.IsActiveName,

                            CreatedAt = g.Key.CreatedAt,
                            UserNameCreated = g.Key.UserNameCreated,
                            ModifiedAt = g.Key.ModifiedAt,
                            UserNameModified = g.Key.UserNameModified,
                        };

            return query;
        }
    }
}
