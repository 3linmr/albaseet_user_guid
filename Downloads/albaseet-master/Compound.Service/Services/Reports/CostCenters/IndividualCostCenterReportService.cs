using Compound.CoreOne.Contracts.Reports.CostCenters;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;
using Compound.CoreOne.Contracts.Reports.Shared;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.CostCenters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;
using Inventory.CoreOne.Contracts;
using Purchases.CoreOne.Contracts;
using Accounting.CoreOne.Contracts;
using Sales.CoreOne.Contracts;

namespace Compound.Service.Services.Reports.CostCenters;

public class IndividualCostCenterReportService : IIndividualCostCenterReportService
{
	private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
	private readonly ICostCenterService _costCenterService;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IGeneralInvoiceService _generalInvoiceService;
	private readonly IJournalHeaderService _journalHeaderService;
	private readonly IJournalDetailService _journalDetailService;

	public IndividualCostCenterReportService(ICostCenterJournalDetailService costCenterJournalDetailService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IInventoryInHeaderService inventoryInHeaderService, IInventoryOutHeaderService inventoryOutHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IPaymentVoucherHeaderService paymentVoucherHeaderService, IReceiptVoucherHeaderService receiptVoucherHeaderService, IGeneralInvoiceService generalInvoiceService, IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService)
	{
		_costCenterJournalDetailService = costCenterJournalDetailService;
		_costCenterService = costCenterService;
		_httpContextAccessor = httpContextAccessor;
		_generalInvoiceService = generalInvoiceService;
		_journalHeaderService = journalHeaderService;
		_journalDetailService = journalDetailService;
	}

	public IQueryable<IndividualCostCenterReportDto> GetIndividualCostCenterReport(int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
	{
		var language = _httpContextAccessor.GetProgramCurrentLanguage();

		var costCenters = from costCenter in _costCenterService.GetAll().Where(x => x.CompanyId == companyId && x.IsMainCostCenter == false)
						  from costCenterJournalDetail in _costCenterJournalDetailService.GetAll().Where(x => x.CostCenterId == costCenter.CostCenterId && (includeItems || x.ItemId == null)).DefaultIfEmpty()
						  from document in _generalInvoiceService.GetGeneralInvoices().Where(x => x.InvoiceId == costCenterJournalDetail.ReferenceHeaderId && x.MenuCode == costCenterJournalDetail.MenuCode).DefaultIfEmpty()
						  from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == costCenterJournalDetail.JournalDetailId).DefaultIfEmpty()
						  from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == journalDetail.JournalHeaderId).DefaultIfEmpty()
						  select new IndividualCostCenterReportDto
						  {
							  CostCenterId = costCenter.CostCenterId,
							  CostCenterCode = costCenter.CostCenterCode,
							  CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							  OpenBalance = costCenterJournalDetail != null ?
									  (
										  (fromDate != null && (document.DocumentDate != null ? document.DocumentDate : (journalHeader != null ? journalHeader.TicketDate : null)) < fromDate) ?
											  (debitOnly ? 0 : costCenterJournalDetail.CreditValue) - costCenterJournalDetail.DebitValue :
											  0
									  )
									  : 0,
							  DebitValue = costCenterJournalDetail != null ?
									  (
										  (fromDate == null || (document.DocumentDate != null ? document.DocumentDate : (journalHeader != null ? journalHeader.TicketDate : null)) >= fromDate) &&
										  (toDate == null || (document.DocumentDate != null ? document.DocumentDate : (journalHeader != null ? journalHeader.TicketDate : null)) <= toDate) ?
											  costCenterJournalDetail.DebitValue :
											  0
									  )
									  : 0,
							  CreditValue = costCenterJournalDetail != null ?
							          (
										  (fromDate == null || (document.DocumentDate != null ? document.DocumentDate : (journalHeader != null ? journalHeader.TicketDate : null)) >= fromDate) &&
										  (toDate == null || (document.DocumentDate != null ? document.DocumentDate : (journalHeader != null ? journalHeader.TicketDate : null)) <= toDate) ?
											  (debitOnly ? 0 : costCenterJournalDetail.CreditValue) :
											  0
									  ) : 0,
							  CurrentBalance = costCenterJournalDetail != null ?
							          (
										  (toDate == null || (document.DocumentDate != null ? document.DocumentDate : (journalHeader != null ? journalHeader.TicketDate : null)) <= toDate) ?
											  (debitOnly ? 0 : costCenterJournalDetail.CreditValue) - costCenterJournalDetail.DebitValue :
											  0
									  ) : 0,
							  CreatedAt = costCenter.CreatedAt,
							  UserNameCreated = costCenter.UserNameCreated,
							  ModifiedAt = costCenter.ModifiedAt,
							  UserNameModified = costCenter.UserNameModified
						  };

		var costCenterGrouped = from costCenter in costCenters
								group costCenter by new { costCenter.CostCenterId, costCenter.CostCenterName, costCenter.CostCenterCode, costCenter.CreatedAt, costCenter.UserNameCreated, costCenter.ModifiedAt, costCenter.UserNameModified } into g
								select new IndividualCostCenterReportDto
								{
									CostCenterId = g.Key.CostCenterId,
									CostCenterName = g.Key.CostCenterName,
									CostCenterCode = g.Key.CostCenterCode,
									OpenBalance = g.Sum(x => x.OpenBalance),
									DebitValue = g.Sum(x => x.DebitValue),
									CreditValue = g.Sum(x => x.CreditValue),
									CurrentBalance = g.Sum(x => x.CurrentBalance),
									CreatedAt = g.Key.CreatedAt,
									UserNameCreated = g.Key.UserNameCreated,
									ModifiedAt = g.Key.ModifiedAt,
									UserNameModified = g.Key.UserNameModified
								};

		return costCenterGrouped;
	}
}
