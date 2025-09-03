using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Accounts;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Accounting.CoreOne.Contracts;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Modules;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;

namespace Compound.Service.Services.Reports.Accounting
{
    public class GeneralJournalReportService : IGeneralJournalReportService
    {

        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IAccountService _accountService;
        private readonly IJournalTypeService _journalTypeService;
        private readonly ITaxService _taxService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInvoiceTypeService _invoiceTypeService;
        private readonly ICurrencyService _currencyService;
		private readonly IGeneralInvoiceService _generalInvoiceService;
        private readonly IStoreService _storeService;

        public GeneralJournalReportService(IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService, IAccountService accountService, IJournalTypeService journalTypeService, ITaxService taxService, IHttpContextAccessor httpContextAccessor, IInvoiceTypeService invoiceTypeService, ICurrencyService currencyService, IGeneralInvoiceService generalInvoiceService, IStoreService storeService)
        {
            _journalHeaderService = journalHeaderService;
            _journalDetailService = journalDetailService;
            _accountService = accountService;
            _journalTypeService = journalTypeService;
            _taxService = taxService;
            _httpContextAccessor = httpContextAccessor;
            _invoiceTypeService = invoiceTypeService;
            _currencyService = currencyService;
		    _generalInvoiceService = generalInvoiceService;
            _storeService = storeService;
        }

        public IQueryable<GeneralJournalReportDto> GetGeneralJournalReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<byte> journalTypes)
        {
            return GetGeneralJournalReportInternal(storeIds, fromDate, toDate, journalTypes);
        }

        public async Task<IQueryable<GeneralJournalReportDto>> GetGeneralJournalReportUserStores()
        {
            var userStores = (await _storeService.GetUserStores()).Select(x => x.StoreId).ToList();
            return GetGeneralJournalReportInternal(userStores, null, null, []);
        }

        public IQueryable<GeneralJournalReportDto> GetGeneralJournalReportInternal(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<byte> journalTypes)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var journals = from journalHeader in _journalHeaderService.GetAll()
                           from journalType in _journalTypeService.GetAll().Where(x => x.JournalTypeId == journalHeader.JournalTypeId)
                           from store in _storeService.GetAll().Where(x => x.StoreId == journalHeader.StoreId)
                           from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId)
                           from account in _accountService.GetAll().Where(x => x.AccountId == journalDetail.AccountId)
                           from currency in _currencyService.GetAll().Where(x => x.CurrencyId == journalDetail.CurrencyId).DefaultIfEmpty()
                           from generalInvoice in _generalInvoiceService.GetGeneralInvoices().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId).DefaultIfEmpty()
                           from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId).DefaultIfEmpty()
                           from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == generalInvoice.InvoiceTypeId /*No need for null check here*/).DefaultIfEmpty()
                           where storeIds.Contains(journalHeader.StoreId) && (fromDate == null || journalHeader.TicketDate >= fromDate) && (toDate == null || journalHeader.TicketDate <= toDate) && (journalTypes.Count == 0 || journalTypes.Contains(journalHeader.JournalTypeId))
                           orderby journalHeader.TicketDate, journalHeader.EntryDate descending
                           select new GeneralJournalReportDto
                           {
                               JournalDetailId = journalDetail.JournalDetailId,
                               JournalHeaderId = journalHeader.JournalHeaderId,
                               JournalId = journalHeader.JournalId,
                               TicketId = journalHeader.TicketId,
                               Serial = journalDetail.Serial,
                               JournalTypeId = journalHeader.JournalTypeId,
                               JournalTypeName = language == LanguageCode.Arabic ? journalType.JournalTypeNameAr : journalType.JournalTypeNameEn,
                               InvoiceId = generalInvoice.InvoiceId != null ? generalInvoice.InvoiceId : null,
                               InvoiceFullCode = generalInvoice.FullInvoiceCode != null ? generalInvoice.FullInvoiceCode : null,
                               JournalFullCode = journalHeader.Prefix + journalHeader.JournalCode + journalHeader.Suffix,
                               TicketDate = journalHeader.TicketDate,
                               EntryDate = journalHeader.EntryDate,
							   StoreId = journalHeader.StoreId,
                               StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                               AccountId = account.AccountId,
                               AccountCode = account.AccountCode,
                               AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
                               IsPrivate = account.IsPrivate,
                               InvoiceReference = generalInvoice.Reference != null ? generalInvoice.Reference : null,
                               PeerReference = journalHeader.PeerReference,
                               CreditValue = journalDetail.CreditValue,
                               DebitValue = journalDetail.DebitValue,
                               Difference = journalDetail.CreditValue - journalDetail.DebitValue,
                               CurrencyId = journalDetail.CurrencyId,
                               CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn,
                               CurrencyRate = journalDetail.CurrencyRate,
                               CreditValueAccount = journalDetail.CreditValueAccount,
                               DebitValueAccount = journalDetail.DebitValueAccount,
                               DifferenceAccount = journalDetail.CreditValueAccount - journalDetail.DebitValueAccount,
                               IsTax = journalDetail.IsTax,
                               TaxId = tax != null ? tax.TaxId : null,
                               TaxParentId = journalDetail.TaxParentId,
                               TaxPercent = journalDetail.TaxPercent,
                               TaxName = tax != null ? language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn : null,
                               TaxCode = journalDetail.TaxCode,
                               TaxReference = journalDetail.TaxReference,
                               TaxDate = journalDetail.TaxDate,
                               InvoiceTypeId = invoiceType.InvoiceTypeId != null ? invoiceType.InvoiceTypeId : null,
                               InvoiceTypeName = invoiceType.InvoiceTypeId != null ? language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn : null,
                               CreatedAt = journalHeader.CreatedAt,
                               UserNameCreated = journalHeader.UserNameCreated,
                               ModifiedAt = journalHeader.ModifiedAt,
                               UserNameModified = journalHeader.UserNameModified
                           };

            return journals;
        }
    }
}
