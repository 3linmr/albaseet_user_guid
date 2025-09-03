using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Contracts.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Compound.Service.Services.Reports.Accounting
{
    public class TaxesDetailReportService : ITaxesDetailReportService
    {
        private readonly IJournalDetailService _journalDetailService;
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJournalTypeService _journalTypeService;
        private readonly ITaxTypeService _taxTypeService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IAccountService _accountService;
        private readonly IClientService _clientService;
        private readonly ISupplierService _supplierService;
        private readonly IJournalHeaderService _journalHeaderService1;
        private readonly IEntityTypeService _entityTypeService;
		private readonly IGeneralInvoiceService _generalInvoiceService;
		private readonly IBranchService _branchService;
		private readonly ICompanyService _companyService;
		private readonly IInvoiceTypeService _invoiceTypeService;

        public TaxesDetailReportService(IJournalDetailService journalDetailService,IJournalHeaderService journalHeaderService,IHttpContextAccessor httpContextAccessor,IJournalTypeService journalTypeService,ITaxTypeService taxTypeService,IStoreService storeService,ITaxService taxService,IAccountService accountService,IClientService clientService,ISupplierService supplierService,IJournalHeaderService journalHeaderService1,IEntityTypeService entityTypeService        , IGeneralInvoiceService generalInvoiceService, IBranchService branchService, ICompanyService companyService, IInvoiceTypeService invoiceTypeService)
        {
            _journalDetailService = journalDetailService;
            _journalHeaderService = journalHeaderService;
            _httpContextAccessor = httpContextAccessor;
            _journalTypeService = journalTypeService;
            _taxTypeService = taxTypeService;
            _storeService = storeService;
            _taxService = taxService;
            _accountService = accountService;
            _clientService = clientService;
            _supplierService = supplierService;
            _journalHeaderService1 = journalHeaderService1;
            _entityTypeService = entityTypeService;
			_generalInvoiceService = generalInvoiceService;
			_branchService = branchService;
			_companyService = companyService;
			_invoiceTypeService = invoiceTypeService;
        }

        public IQueryable<TaxesDetailReportDto> GetTaxesDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isVatTax)            
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var taxData = from journalHeader in _journalHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.TicketDate >= fromDate) && (toDate == null || x.TicketDate <= toDate))
                          from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId && x.IsTax == true)
                          from parentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
                          from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == isVatTax)
                          from journalType in _journalTypeService.GetAll().Where(x => x.JournalTypeId == journalHeader.JournalTypeId)
                          from store in _storeService.GetAll().Where(x => x.StoreId == journalHeader.StoreId)
                          from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
                          from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
                          from client in _clientService.GetAll().Where(x => x.ClientId == journalDetail.EntityId).DefaultIfEmpty()
                          from supplier in _supplierService.GetAll().Where(x => x.SupplierId == journalDetail.EntityId).DefaultIfEmpty()
                          from invoice in _generalInvoiceService.GetGeneralInvoices().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId).DefaultIfEmpty()
                          from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == invoice.InvoiceTypeId).DefaultIfEmpty()
                          orderby journalHeader.TicketDate, journalHeader.EntryDate, journalHeader.JournalHeaderId descending
						  select new TaxesDetailReportDto
                          {
                              JournalHeaderId = journalHeader.JournalHeaderId,
                              TicketId = journalHeader.TicketId,
                              Serial = journalDetail.Serial,
                              TicketDate = journalHeader.TicketDate,
                              EntryDate = journalHeader.EntryDate,
                              InvoiceId = invoice.InvoiceId,
                              InvoiceFullCode = invoice.FullInvoiceCode,
                              JournalFullCode = journalHeader.Prefix + journalHeader.JournalCode + journalHeader.Suffix,
                              JournalTypeId = journalHeader.JournalTypeId,
                              JournalTypeName = language == LanguageCode.Arabic ? journalType.JournalTypeNameAr : journalType.JournalTypeNameEn,
                              InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,

							  AccountNameAr = client.ClientNameAr ?? supplier.SupplierNameAr,
							  AccountNameEn = client.ClientNameEn ?? supplier.SupplierNameEn,

                              GrossValue = Math.Max(parentJournalDetail.CreditValue, parentJournalDetail.DebitValue), //parent journal detail is the value before any taxes
                              TaxValue = Math.Max(journalDetail.CreditValue, journalDetail.DebitValue),
                              TaxPaid = journalDetail.DebitValue,
                              TaxCollected = journalDetail.CreditValue,

                              IsTax = journalDetail.IsTax,
                              TaxId = journalDetail.TaxId,
                              TaxPercent = journalDetail.TaxPercent,
                              TaxName = tax != null ? language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn : "",
                              TaxCode = journalDetail.TaxCode,
                              TaxReference = journalDetail.TaxReference,
                              TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
                              TaxDate = journalDetail.TaxDate,

                              PeerReference = journalHeader.PeerReference,
                              InvoiceReference = invoice.Reference,
                              RemarksAr = invoice.RemarksAr ?? journalHeader.RemarksAr,
							  RemarksEn = invoice.RemarksEn ?? journalHeader.RemarksEn,

                              PostalCode = client.PostalCode ?? supplier.PostalCode,
                              BuildingNumber = client.BuildingNumber ?? supplier.BuildingNumber,
                              Street1 = client.Street1 ?? supplier.Street1,
							  Street2 = client.Street2 ?? supplier.Street2,
                              AdditionalNumber = client.AdditionalNumber ?? supplier.AdditionalNumber,
							  Address1 = client.Address1 ?? supplier.Address1,
                              Address2 = client.Address2 ?? supplier.Address2,
                              Address3 = client.Address3 ?? supplier.Address3,
                              Address4 = client.Address4 ?? supplier.Address4,
                              FirstResponsibleName = client.FirstResponsibleName ?? supplier.FirstResponsibleName,
                              FirstResponsiblePhone = client.FirstResponsiblePhone ?? supplier.FirstResponsiblePhone,
							  FirstResponsibleEmail = client.FirstResponsibleEmail ?? supplier.FirstResponsibleEmail,
                              SecondResponsibleName = client.SecondResponsibleName ?? supplier.SecondResponsibleName,
							  SecondResponsiblePhone = client.SecondResponsiblePhone ?? supplier.SecondResponsiblePhone,
							  SecondResponsibleEmail = client.SecondResponsibleEmail ?? supplier.SecondResponsibleEmail,

							  StoreId = journalHeader.StoreId,
                              StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
							  BranchId = store.BranchId,
                              BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
							  CompanyId = branch.CompanyId,
                              CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,

							  CreatedAt = journalHeader.CreatedAt,
                              UserNameCreated = journalHeader.UserNameCreated,
                              ModifiedAt = journalHeader.ModifiedAt,
                              UserNameModified = journalHeader.UserNameModified
                          };

            return taxData;
        }
    }
}
