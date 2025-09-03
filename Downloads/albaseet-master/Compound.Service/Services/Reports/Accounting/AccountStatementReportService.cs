using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Contracts.Reports;
using Accounting.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Compound.Service.Services.InvoiceSettlement;
using Inventory.CoreOne.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using MoreLinq.Extensions;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Sales.Service.Services;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using Shared.Repository;
using Shared.Service.Services.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
#pragma warning disable CS8603 // Possible null reference return.

namespace Compound.Service.Services.Reports.Accounting
{
    public class AccountStatementReportService : IAccountStatementReportService
    {
      
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
        private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IInvoiceTypeService _invoiceTypeService;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJournalTypeService _journalTypeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IStringLocalizer<AccountStatementReportService> _localizer;
        private readonly IGeneralInvoiceService _generalInvoiceService;
        private readonly ITaxTypeService _taxTypeService;
        public AccountStatementReportService(IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService, IAccountService accountService, IHttpContextAccessor httpContextAccessor, IJournalTypeService journalTypeService,ICostCenterService costCenterService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStoreService storeService, IInvoiceTypeService invoiceTypeService,
            IPaymentVoucherHeaderService paymentVoucherHeaderService, IReceiptVoucherHeaderService receiptVoucherHeaderService,
            IStringLocalizer<AccountStatementReportService> localizer, ITaxService taxService,
            IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService,
            IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService, IGeneralInvoiceService generalInvoiceService, ITaxTypeService taxTypeService)
        {
            _journalHeaderService = journalHeaderService;
            _journalDetailService = journalDetailService;
            _invoiceTypeService = invoiceTypeService;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _journalTypeService = journalTypeService;
            _costCenterService = costCenterService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _storeService = storeService;
            _localizer= localizer;
            _paymentVoucherHeaderService = paymentVoucherHeaderService;
            _receiptVoucherHeaderService= receiptVoucherHeaderService;
            _taxService= taxService;
            _clientDebitMemoService= clientDebitMemoService;
            _clientCreditMemoService= clientCreditMemoService;
            _generalInvoiceService= generalInvoiceService;
            _taxTypeService= taxTypeService;
        }
        public IQueryable<AccountStatementDto> GetAccountStatementReport(int accountId, int companyId, DateTime? fromDate, DateTime? toDate)
        {
            //var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var openingBalanceQuery = GetOpeningBalance([accountId], companyId, fromDate);
            var balances = GetJournalBalances([accountId]);
            var journalEntriesQuery = GetJournalEntries([accountId], companyId, fromDate, toDate, balances);
            var closingBalanceQuery = GetClosingBalance([accountId], companyId, toDate);
            var allStatementsQuery = openingBalanceQuery.Concat(journalEntriesQuery).Concat(closingBalanceQuery);
            
            return allStatementsQuery.OrderBy(x => x.TicketDate).ThenBy(x => x.EntryDate).ThenBy(x => x.JournalDetailId);
        }

        /// Returns the opening balance as of the fromDate.
        private IQueryable<AccountStatementDto> GetOpeningBalance(List<int> accountIds, int companyId, DateTime? fromDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var query = from account in _accountService.GetAll().Where(x => accountIds.Contains( x.AccountId) && x.CompanyId == companyId)
                        from journalDetail in _journalDetailService.GetAll().Where(x => x.AccountId == account.AccountId).DefaultIfEmpty()
                        from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == journalDetail.JournalHeaderId).DefaultIfEmpty()
                        from store in _storeService.GetAll().Where(x => x.StoreId == journalHeader.StoreId).DefaultIfEmpty()
                        from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == account.CostCenterId).DefaultIfEmpty()
                        where fromDate == null || (journalHeader != null)
                        group new { journalDetail, journalHeader } by new { account.AccountId, account.AccountCode,account.AccountNameAr, account.AccountNameEn,/*journalHeader.Prefix , journalHeader.Suffix,*/costCenter.CostCenterNameAr,costCenter.CostCenterNameEn } into g
                        select new AccountStatementDto
                        {
							JournalDetailId = null,
							AccountCode = g.Key.AccountCode,
							DocumentFullCode = null,
							AccountId = g.Key.AccountId,
							TicketDate = fromDate != null ? fromDate : DateTime.MinValue,
							EntryDate = null,
							StoreName = null,
							AccountName = language == LanguageData.LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,
							InvoiceTypeName = null,
							JournalTypeName = _localizer["OpeningBalance"],
							DocumentTypeName = _localizer["OpeningBalance"],
							DebitValue = fromDate != null ? (g.Sum(x => x.journalHeader.TicketDate < fromDate ? x.journalDetail.DebitValue : 0)) : 0,
							CreditValue = fromDate != null ? g.Sum(x => x.journalHeader.TicketDate < fromDate ? x.journalDetail.CreditValue : 0) : 0,
							Balance = g.Sum(x => (fromDate != null && x.journalHeader.TicketDate < fromDate) ? x.journalDetail.CreditValue - x.journalDetail.DebitValue : 0),
							DebitValueAccount = fromDate != null ? g.Sum(x => x.journalHeader.TicketDate < fromDate ? x.journalDetail.DebitValueAccount : 0) : 0,
							CreditValueAccount = fromDate != null ? g.Sum(x => x.journalHeader.TicketDate < fromDate ? x.journalDetail.CreditValueAccount : 0) : 0,
							OpeningBalanceAccount = g.Sum(x => (fromDate != null && x.journalHeader.TicketDate < fromDate) ? x.journalDetail.CreditValueAccount - x.journalDetail.DebitValueAccount : 0),
							BalanceAccount = g.Sum(x => (fromDate != null && x.journalHeader.TicketDate < fromDate) ? x.journalDetail.CreditValueAccount - x.journalDetail.DebitValueAccount : 0),
							JournalFullCode = null,
							RemarksAr = null,
							RemarksEn = null,
							CostCenterName = language == LanguageData.LanguageCode.Arabic ? g.Key.CostCenterNameAr : g.Key.CostCenterNameEn,
							DocumentEntrySerial = 0,
							ExchangeRate = 0,
							PeerReference = null,
                            DocumentReference = null,

                            CreatedAt = null,
							UserNameCreated = null,
							ModifiedAt = null,
							UserNameModified = null,
						};

            return query;
        }

        /// Returns the running balance for each journal detail based on previous entries.
        private IQueryable<JournalBalanceDto> GetJournalBalances(List<int> accountIds)
        {
            var balances = from journalDetail in _journalDetailService.GetAll().Where(x => accountIds.Contains(x.AccountId))
                           from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == journalDetail.JournalHeaderId)
                           from previousJournalDetails in _journalDetailService.GetAll().Where(x => x.AccountId == journalDetail.AccountId)
                           // Returns the running balance for each journal detail based on all previous entries for the same account,
                           // ordered by TicketDate, then EntryDate, then JournalDetailId, to ensure correct running balance calculation.
                           from previousJournalHeaders in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == previousJournalDetails.JournalHeaderId)
                           where
                                previousJournalHeaders.TicketDate < journalHeader.TicketDate
                                ||
                                (previousJournalHeaders.TicketDate == journalHeader.TicketDate
                                    && (previousJournalHeaders.EntryDate) < (journalHeader.EntryDate))
                                ||
                                (previousJournalHeaders.TicketDate == journalHeader.TicketDate
                                    && (previousJournalHeaders.EntryDate) == (journalHeader.EntryDate)
                                    && previousJournalDetails.JournalDetailId <= journalDetail.JournalDetailId)
                           group new { journalDetail, previousJournalDetails, journalHeader } by new
                           {
                               journalDetail.JournalDetailId,
                               journalHeader.TicketDate,
                               journalHeader.EntryDate
                           } into g
                           orderby g.Key.TicketDate, g.Key.EntryDate, g.Key.JournalDetailId
                           select new JournalBalanceDto
                           {
                               JournalDetailId = g.Key.JournalDetailId,
                               Balance = g.Sum(x => x.previousJournalDetails.CreditValue - x.previousJournalDetails.DebitValue),
                               BalanceAccount = g.Sum(x => x.previousJournalDetails.CreditValueAccount - x.previousJournalDetails.DebitValueAccount),
                           };

            return balances;
        }

        /// Returns the journal entries within the specified date range.
        private IQueryable<AccountStatementDto> GetJournalEntries(List<int> accountIds, int companyId, DateTime? fromDate, DateTime? toDate, IQueryable<JournalBalanceDto> balances)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var query = from journalHeader in _journalHeaderService.GetAll()
                        from journalType in _journalTypeService.GetAll().Where(x => x.JournalTypeId == journalHeader.JournalTypeId)
                        from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId)
                        from journalBalance in balances.Where(x => x.JournalDetailId == journalDetail.JournalDetailId)
                        from account in _accountService.GetAll().Where(x => accountIds.Contains(x.AccountId) && x.AccountId == journalDetail.AccountId && x.CompanyId == companyId)
                        from store in _storeService.GetAll().Where(x => x.StoreId == journalHeader.StoreId)
                        from generalInvoice in _generalInvoiceService.GetGeneralInvoices() .Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId).DefaultIfEmpty()
                        from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == generalInvoice.TaxTypeId).DefaultIfEmpty()
                        from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == generalInvoice.InvoiceTypeId).DefaultIfEmpty()
                        from taxes in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId).DefaultIfEmpty()
                        from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == account.CostCenterId).DefaultIfEmpty()
                        where (fromDate == null || journalHeader.TicketDate >= fromDate) && (toDate == null || journalHeader.TicketDate <= toDate)
                        orderby journalHeader.TicketDate, journalHeader.EntryDate, journalDetail.JournalDetailId
                        select new AccountStatementDto
						{
							JournalDetailId = journalDetail.JournalDetailId,
							AccountCode = account.AccountCode,
							AccountId = account.AccountId,
							TicketDate = journalHeader.TicketDate,
							EntryDate = journalHeader.EntryDate,
							StoreName = language == LanguageData.LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
							AccountName = language == LanguageData.LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
							InvoiceTypeName = language == LanguageData.LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
							JournalFullCode = journalHeader.Prefix + journalHeader.JournalCode + journalHeader.Suffix,
							DocumentFullCode = generalInvoice != null ? generalInvoice.FullInvoiceCode : null,
							JournalTypeName = language == LanguageData.LanguageCode.Arabic ? journalType.JournalTypeNameAr : journalType.JournalTypeNameEn,
							DocumentTypeName = language == LanguageData.LanguageCode.Arabic ? journalType.JournalTypeNameAr : journalType.JournalTypeNameEn,
							DebitValue = journalDetail.DebitValue,
							CreditValue = journalDetail.CreditValue,
							Balance = journalBalance.Balance,
							CreditValueAccount = journalDetail.CreditValueAccount,
							DebitValueAccount = journalDetail.DebitValueAccount,
							RemarksAr = journalHeader.RemarksAr,
							RemarksEn = journalHeader.RemarksEn,
							PeerReference = journalHeader.PeerReference,
                            DocumentReference = generalInvoice != null ? generalInvoice.Reference : null,
							BalanceAccount = journalBalance.BalanceAccount,
							OpeningBalanceAccount = 0,
							CostCenterName = language == LanguageData.LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							DocumentEntrySerial = journalDetail.Serial,
							ExchangeRate = journalDetail.CurrencyRate,

                            CreatedAt = journalDetail.CreatedAt,
							UserNameCreated = journalDetail.UserNameCreated,
							ModifiedAt = journalDetail.ModifiedAt,
							UserNameModified = journalDetail.UserNameModified,
						};

			return query;
        }

        /// Returns the closing balance as of the toDate.
        private IQueryable<AccountStatementDto> GetClosingBalance(List<int> accountIds, int companyId, DateTime? toDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var query = from account in _accountService.GetAll().Where(x => accountIds.Contains(x.AccountId) && x.CompanyId == companyId)
                        from journalDetail in _journalDetailService.GetAll().Where(x => x.AccountId == account.AccountId).DefaultIfEmpty()
                        from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == journalDetail.JournalHeaderId).DefaultIfEmpty()
                        from store in _storeService.GetAll().Where(x => x.StoreId == journalHeader.StoreId).DefaultIfEmpty()
                        from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == account.CostCenterId).DefaultIfEmpty()
                        where toDate == null || (journalHeader != null)
                        group new { journalDetail, journalHeader } by new { account.AccountId, account.AccountCode, account.AccountNameAr, account.AccountNameEn, costCenter.CostCenterNameAr, costCenter.CostCenterNameEn } into g
                        select new AccountStatementDto
                        {
                            JournalDetailId = null,
                            AccountCode = g.Key.AccountCode,
                            AccountId = g.Key.AccountId,
                            DocumentFullCode = null,
                            AccountName = language == LanguageData.LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,
                            TicketDate = toDate != null ? toDate : DateTime.MaxValue,
                            EntryDate = null,
                            JournalTypeName = _localizer["CurrentBalance"],
                            DocumentTypeName = _localizer["CurrentBalance"],
                            DebitValue = g.Sum(x => x.journalHeader.TicketDate <= toDate || toDate == null ? (x.journalDetail.DebitValue) : 0),
                            CreditValue = g.Sum(x => x.journalHeader.TicketDate <= toDate || toDate == null ? (x.journalDetail.CreditValue) : 0),
                            Balance = g.Sum(x => x.journalHeader.TicketDate <= toDate|| toDate ==null?  (x.journalDetail.CreditValue - x.journalDetail.DebitValue):0 ),
                            StoreName = null,
                            InvoiceTypeName = null,
                            DebitValueAccount = toDate == null?  g.Sum(x =>x.journalDetail.DebitValueAccount ): 0,
                            CreditValueAccount = toDate == null ? g.Sum(x =>x.journalDetail.CreditValueAccount ): g.Sum(x => x.journalHeader.TicketDate <= toDate?x.journalDetail.CreditValueAccount:0),
                            BalanceAccount= g.Sum(x => x.journalHeader.TicketDate <= toDate|| toDate ==null?  (x.journalDetail.CreditValueAccount - x.journalDetail.DebitValueAccount):0 ),
                            RemarksAr = null,
                            RemarksEn = null,
                            CostCenterName = language == LanguageData.LanguageCode.Arabic ? g.Key.CostCenterNameAr : g.Key.CostCenterNameEn,
                            JournalFullCode = null,
                            DocumentEntrySerial = 0,
                            ExchangeRate = 0,
                            PeerReference = null,
                            DocumentReference = null,
                            OpeningBalanceAccount=0,

                            CreatedAt = null,
							UserNameCreated = null,
							ModifiedAt = null,
							UserNameModified = null,
                        };

            return query;
        }

		//Returns Account statement report for a group of accounts
		public IQueryable<AccountStatementDto> GetGeneralAccountsStatementReport(List<int> accountIds, int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            IQueryable<AccountStatementDto> allStatementsQuery = null;

			var openingBalanceQuery = GetOpeningBalance(accountIds, companyId, fromDate);
			var balances = GetJournalBalances(accountIds);
			var journalEntriesQuery = GetJournalEntries(accountIds, companyId, fromDate, toDate, balances);
			var closingBalanceQuery = GetClosingBalance(accountIds, companyId, toDate);
			var accountStatements = openingBalanceQuery.Concat(journalEntriesQuery).Concat(closingBalanceQuery);
			allStatementsQuery = allStatementsQuery == null ? accountStatements : allStatementsQuery.Concat(accountStatements);
			return allStatementsQuery.OrderBy(x => x.AccountId).ThenBy(x => x.TicketDate).ThenBy(x => x.EntryDate).ThenBy(x => x.JournalDetailId);
		}

	}
    
}
