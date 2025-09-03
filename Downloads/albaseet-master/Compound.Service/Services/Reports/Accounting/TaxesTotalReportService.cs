using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Compound.Service.Services.Reports.Accounting
{
    public class TaxesTotalReportService : ITaxesTotalReportService
    {
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly ITaxService _taxService;
        private readonly ITaxTypeService _taxTypeService;
        private readonly IJournalTypeService _journalTypeService;
        private readonly IStringLocalizer<TaxesTotalReportService> _localizer;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
		private readonly ISalesInvoiceDetailTaxService _salesInvoiceDetailTaxService;
		private readonly ISalesInvoiceReturnDetailTaxService _salesInvoiceReturnDetailTaxService;
		private readonly IPurchaseInvoiceReturnDetailTaxService _purchaseInvoiceReturnDetailTaxService;
		private readonly IPurchaseInvoiceDetailTaxService _purchaseInvoiceDetailTaxService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;
		private readonly IStoreService _storeService;

        public TaxesTotalReportService(IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService, IHttpContextAccessor httpContextAccessor, IAccountService accountService, ITaxService taxService, ITaxTypeService taxTypeService, IJournalTypeService journalTypeService, IStringLocalizer<TaxesTotalReportService> localizer, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, ISalesInvoiceDetailTaxService salesInvoiceDetailTaxService, ISalesInvoiceReturnDetailTaxService salesInvoiceReturnDetailTaxService, IPurchaseInvoiceReturnDetailTaxService purchaseInvoiceReturnDetailTaxService, IPurchaseInvoiceDetailTaxService purchaseInvoiceDetailTaxService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IStoreService storeService)
        {
            _journalHeaderService = journalHeaderService;
            _journalDetailService = journalDetailService;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _taxService = taxService;
            _taxTypeService = taxTypeService;
            _journalTypeService = journalTypeService;
			_localizer = localizer;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_salesInvoiceDetailTaxService = salesInvoiceDetailTaxService;
			_salesInvoiceReturnDetailTaxService = salesInvoiceReturnDetailTaxService;
			_purchaseInvoiceReturnDetailTaxService = purchaseInvoiceReturnDetailTaxService;
			_purchaseInvoiceDetailTaxService = purchaseInvoiceDetailTaxService;
			_clientCreditMemoService = clientCreditMemoService;
			_clientDebitMemoService = clientDebitMemoService;
			_supplierCreditMemoService = supplierCreditMemoService;
			_supplierDebitMemoService = supplierDebitMemoService;
			_storeService = storeService;
        }

        public async Task<List<TaxesTotalReportDto>> GetTaxesTotalReport(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyStoreIds = _storeService.GetAllStores().Where(x => x.CompanyId == companyId).Select(x => x.StoreId).ToList();

			var salesInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
								from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => salesInvoiceHeader.SalesInvoiceHeaderId == x.SalesInvoiceHeaderId)
								select new TaxesTotalReportDto
								{
									TaxTypeId = salesInvoiceDetail.VatTaxTypeId,
									TotalValue = salesInvoiceDetail.GrossValue,
									TaxPercent = salesInvoiceDetail.VatPercent,
									TotalTaxValue = salesInvoiceDetail.VatValue,

									TotalPurchaseOrSalesAmount = salesInvoiceDetail.GrossValue,
									TotalPurchaseOrSaleReturnAmount = 0,
									TotalJournalEntryAmount = 0,
								};

			var salesInvoiceReturns = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
									  from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId == x.SalesInvoiceReturnHeaderId)
									  select new TaxesTotalReportDto
									  {
										  TaxTypeId = salesInvoiceReturnDetail.VatTaxTypeId,
										  TotalValue = -salesInvoiceReturnDetail.GrossValue,
										  TaxPercent = salesInvoiceReturnDetail.VatPercent,
										  TotalTaxValue = -salesInvoiceReturnDetail.VatValue,

										  TotalPurchaseOrSalesAmount = 0,
										  TotalPurchaseOrSaleReturnAmount = salesInvoiceReturnDetail.GrossValue,
										  TotalJournalEntryAmount = 0,
									  };

			var clientCreditMemos = from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate)) 
									from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientCreditMemo.JournalHeaderId && x.IsTax)
									from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == true)
									from parentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
									select new TaxesTotalReportDto
									{
									    TaxTypeId = journalDetail.TaxTypeId,
									    TotalValue = -parentJournalDetail.DebitValue,
									    TaxPercent = journalDetail.TaxPercent,
									    TotalTaxValue = -journalDetail.DebitValue,

									    TotalPurchaseOrSalesAmount = 0,
									    TotalPurchaseOrSaleReturnAmount = parentJournalDetail.DebitValue,
									    TotalJournalEntryAmount = 0,
									};

			var clientDebitMemos = from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate)) 
								   from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientDebitMemo.JournalHeaderId && x.IsTax)
								   from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == true)
								   from parentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
								   select new TaxesTotalReportDto
								   {
								       TaxTypeId = journalDetail.TaxTypeId,
								       TotalValue = parentJournalDetail.CreditValue,
								       TaxPercent = journalDetail.TaxPercent,
								       TotalTaxValue = journalDetail.CreditValue,

								       TotalPurchaseOrSalesAmount = parentJournalDetail.CreditValue,
								       TotalPurchaseOrSaleReturnAmount = 0,
								       TotalJournalEntryAmount = 0,
								   };

			var creditJournalEntries = from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalTypeId == JournalTypeData.JournalEntry && companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.TicketDate >= fromDate) && (toDate == null || x.TicketDate <= toDate))
									   from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId && x.IsTax && x.CreditValue > 0)
									   from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == true)
									   from taxParentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
									   select new TaxesTotalReportDto
									   {
										   TaxTypeId = journalDetail.TaxTypeId ?? TaxTypeData.Taxable,
										   TotalValue = taxParentJournalDetail.CreditValue,
										   TaxPercent = journalDetail.TaxPercent,
										   TotalTaxValue = journalDetail.CreditValue,

										   TotalPurchaseOrSalesAmount = 0,
										   TotalPurchaseOrSaleReturnAmount = 0,
										   TotalJournalEntryAmount = taxParentJournalDetail.CreditValue,
									   };


			var totalSales = (await (from taxType in _taxTypeService.GetAll()
									 from data in salesInvoices.Concat(salesInvoiceReturns).Concat(clientCreditMemos).Concat(clientDebitMemos).Concat(creditJournalEntries).Where(x => x.TaxTypeId == taxType.TaxTypeId).DefaultIfEmpty()
									 group data by new { taxType.TaxTypeId, taxType.TaxTypeNameAr, taxType.TaxTypeNameEn, data.TaxPercent } into g
									 select new TaxesTotalReportDto {
									     TaxTypeId = g.Key.TaxTypeId,
									     TaxTypeName = language == LanguageCode.Arabic ? g.Key.TaxTypeNameAr : g.Key.TaxTypeNameEn,
									     TotalValue = g.Sum(x => x.TotalValue),
									     TaxPercent = (decimal?)g.Key.TaxPercent ?? 0,
									     TotalTaxValue = g.Sum(x => x.TotalTaxValue),

									     TotalPurchaseOrSalesAmount = g.Sum(x => x.TotalPurchaseOrSalesAmount),
									     TotalPurchaseOrSaleReturnAmount = g.Sum(x => x.TotalPurchaseOrSaleReturnAmount),
									     TotalJournalEntryAmount = g.Sum(x => x.TotalJournalEntryAmount),
								     }).ToListAsync()).OrderBy(x => x.TaxTypeId);

			var purchaseInvoices = from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
								   from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => purchaseInvoiceHeader.PurchaseInvoiceHeaderId == x.PurchaseInvoiceHeaderId)
								   select new TaxesTotalReportDto
								   {
									   TaxTypeId = purchaseInvoiceDetail.VatTaxTypeId,
									   TotalValue = purchaseInvoiceDetail.GrossValue,
									   TaxPercent = purchaseInvoiceDetail.VatPercent,
									   TotalTaxValue = purchaseInvoiceDetail.VatValue,

									   TotalPurchaseOrSalesAmount = purchaseInvoiceDetail.GrossValue,
									   TotalPurchaseOrSaleReturnAmount = 0,
									   TotalJournalEntryAmount = 0,
								   };

			var purchaseInvoiceReturns = from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
										 from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId == x.PurchaseInvoiceReturnHeaderId)
										 select new TaxesTotalReportDto
										 {
									         TaxTypeId = purchaseInvoiceReturnDetail.VatTaxTypeId,
											 TotalValue = -purchaseInvoiceReturnDetail.GrossValue,
											 TaxPercent = purchaseInvoiceReturnDetail.VatPercent,
											 TotalTaxValue = -purchaseInvoiceReturnDetail.VatValue,

											 TotalPurchaseOrSalesAmount = 0,
											 TotalPurchaseOrSaleReturnAmount = purchaseInvoiceReturnDetail.GrossValue,
											 TotalJournalEntryAmount = 0,
										 };

			var supplierCreditMemos = from supplierCreditMemo in _supplierCreditMemoService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
									  from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == supplierCreditMemo.JournalHeaderId && x.IsTax)
									  from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == true)
									  from parentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
									  select new TaxesTotalReportDto
									  {
										  TaxTypeId = journalDetail.TaxTypeId,
										  TotalValue = parentJournalDetail.DebitValue,
										  TaxPercent = journalDetail.TaxPercent,
										  TotalTaxValue = journalDetail.DebitValue,

										  TotalPurchaseOrSalesAmount = parentJournalDetail.DebitValue,
										  TotalPurchaseOrSaleReturnAmount = 0,
										  TotalJournalEntryAmount = 0,
									  };

			var supplierDebitMemos = from supplierDebitMemo in _supplierDebitMemoService.GetAll().Where(x => companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
									 from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == supplierDebitMemo.JournalHeaderId && x.IsTax)
									 from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == true)
									 from parentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
									 select new TaxesTotalReportDto
									 {
										 TaxTypeId = journalDetail.TaxTypeId,
										 TotalValue = -parentJournalDetail.CreditValue,
										 TaxPercent = journalDetail.TaxPercent,
										 TotalTaxValue = -journalDetail.CreditValue,

										 TotalPurchaseOrSalesAmount = 0,
										 TotalPurchaseOrSaleReturnAmount = parentJournalDetail.CreditValue,
										 TotalJournalEntryAmount = 0,
									 };

			var debitJournalEntries = from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalTypeId == JournalTypeData.JournalEntry && companyStoreIds.Contains(x.StoreId) && (fromDate == null || x.TicketDate >= fromDate) && (toDate == null || x.TicketDate <= toDate))
									  from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId && x.IsTax && x.DebitValue > 0)
									  from tax in _taxService.GetAll().Where(x => x.TaxId == journalDetail.TaxId && x.IsVatTax == true)
									  from taxParentJournalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == journalDetail.TaxParentId)
									  select new TaxesTotalReportDto
									  {
										  TaxTypeId = journalDetail.TaxTypeId ?? TaxTypeData.Taxable,
										  TotalValue = taxParentJournalDetail.DebitValue,
										  TaxPercent = journalDetail.TaxPercent,
										  TotalTaxValue = journalDetail.DebitValue,

										  TotalPurchaseOrSalesAmount = 0,
										  TotalPurchaseOrSaleReturnAmount = 0,
										  TotalJournalEntryAmount = taxParentJournalDetail.DebitValue,
									  };

			var totalPurchases = (await (from taxType in _taxTypeService.GetAll()
										 from data in purchaseInvoices.Concat(purchaseInvoiceReturns).Concat(supplierDebitMemos).Concat(supplierCreditMemos).Concat(debitJournalEntries).Where(x => x.TaxTypeId == taxType.TaxTypeId).DefaultIfEmpty()
										 group data by new { taxType.TaxTypeId, taxType.TaxTypeNameAr, taxType.TaxTypeNameEn, data.TaxPercent } into g
										 select new TaxesTotalReportDto
										 {
											 TaxTypeId = g.Key.TaxTypeId,
											 TaxTypeName = language == LanguageCode.Arabic ? g.Key.TaxTypeNameAr : g.Key.TaxTypeNameEn,
											 TotalValue = g.Sum(x => x.TotalValue),
											 TaxPercent = (decimal?)g.Key.TaxPercent ?? 0,
											 TotalTaxValue = g.Sum(x => x.TotalTaxValue),

											 TotalPurchaseOrSalesAmount = g.Sum(x => x.TotalPurchaseOrSalesAmount),
											 TotalPurchaseOrSaleReturnAmount = g.Sum(x => x.TotalPurchaseOrSaleReturnAmount),
											 TotalJournalEntryAmount = g.Sum(x => x.TotalJournalEntryAmount),
										 }).ToListAsync()).OrderBy(x => x.TaxTypeId);

			var result = totalSales.Append(new TaxesTotalReportDto {
				TaxTypeId = null,
				TaxTypeName = _localizer["TotalSales"],
				TotalValue = totalSales.Sum(x => x.TotalValue),
				TaxPercent = 0,
				TotalTaxValue = totalSales.Sum(x => x.TotalTaxValue),

				TotalPurchaseOrSalesAmount = totalSales.Sum(x => x.TotalPurchaseOrSalesAmount),
				TotalPurchaseOrSaleReturnAmount = totalSales.Sum(x => x.TotalPurchaseOrSaleReturnAmount),
				TotalJournalEntryAmount = totalSales.Sum(x => x.TotalJournalEntryAmount),
			}).Concat(totalPurchases).Append(new TaxesTotalReportDto {
				TaxTypeId = null,
				TaxTypeName = _localizer["TotalPurchases"],
				TotalValue = totalPurchases.Sum(x => x.TotalValue),
				TaxPercent = 0,
				TotalTaxValue = totalPurchases.Sum(x => x.TotalTaxValue),

				TotalPurchaseOrSalesAmount = totalPurchases.Sum(x => x.TotalPurchaseOrSalesAmount),
				TotalPurchaseOrSaleReturnAmount = totalPurchases.Sum(x => x.TotalPurchaseOrSaleReturnAmount),
				TotalJournalEntryAmount = totalPurchases.Sum(x => x.TotalJournalEntryAmount),
			}).ToList();

			int serial = 0;
			result.ForEach(x => x.Serial = serial++);

			return result;
        }
    }
}
