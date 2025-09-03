using Compound.CoreOne.Contracts.Reports.CostCenters;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Journal;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Contracts.Reports.Shared;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Items;

namespace Compound.Service.Services.Reports.CostCenters
{
    public class CostCenterJournalReportService : ICostCenterJournalReportService
    {
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IAccountService _accountService;
        private readonly IGeneralInvoiceDetailService _generalInvoiceDetailService;
        private readonly IGeneralInvoiceService _generalInvoiceService;
        private readonly IStoreService _storeService;
		private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
		private readonly IMenuService _menuService;
		private readonly IItemService _itemService;
		private readonly IBranchService _branchService;
		private readonly ICompanyService _companyService;

		public CostCenterJournalReportService(ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IJournalHeaderService journalHeaderService, IJournalDetailService journalDetailService, IAccountService accountService, IGeneralInvoiceDetailService generalInvoiceDetailService, IGeneralInvoiceService generalInvoiceService, IStoreService storeService, ICostCenterJournalDetailService costCenterJournalDetailService, IMenuService menuService, IItemService itemService, IBranchService branchService, ICompanyService companyService)
		{
			_costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
			_journalHeaderService = journalHeaderService;
			_journalDetailService = journalDetailService;
			_accountService = accountService;
			_generalInvoiceDetailService = generalInvoiceDetailService;
			_generalInvoiceService = generalInvoiceService;
			_storeService = storeService;
			_costCenterJournalDetailService = costCenterJournalDetailService;
			_menuService = menuService;
			_itemService = itemService;
			_branchService = branchService;
			_companyService = companyService;
		}

        public IQueryable<CostCenterJournalReportDto> GetCostCenterJournalReport(int? costCenterId, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var costCenterJournals = from costCenter in _costCenterService.GetAll().Where(x => x.CompanyId == companyId && (costCenterId == null || x.CostCenterId == costCenterId))
                                     from costCenterJournalDetail in _costCenterJournalDetailService.GetAll().Where(x => x.CostCenterId == costCenter.CostCenterId && (includeItems || x.ItemId == null))
                                     from documentThruCostCenterJournalDetail in _generalInvoiceDetailService.GetGeneralInvoiceDetails().Where(x => x.InvoiceId == costCenterJournalDetail.ReferenceHeaderId && x.InvoiceDetailId == costCenterJournalDetail.ReferenceDetailId && x.MenuCode == costCenterJournalDetail.MenuCode).DefaultIfEmpty()
                                     from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalDetailId == costCenterJournalDetail.JournalDetailId).DefaultIfEmpty()
                                     from journalHeader in _journalHeaderService.GetAll().Where(x => x.JournalHeaderId == journalDetail.JournalHeaderId).DefaultIfEmpty()
                                     from documentThruJournalHeader in _generalInvoiceService.GetGeneralInvoices().Where(x => x.JournalHeaderId == journalHeader.JournalHeaderId).DefaultIfEmpty()
                                     from menu in _menuService.GetAll().Where(x => x.MenuCode == (costCenterJournalDetail.MenuCode ?? documentThruJournalHeader.MenuCode)).DefaultIfEmpty()
                                     from item in _itemService.GetAll().Where(x => x.ItemId == documentThruCostCenterJournalDetail.ItemId).DefaultIfEmpty()
                                     from account in _accountService.GetAll().Where(x => x.AccountId == journalDetail.AccountId).DefaultIfEmpty()
                                     from store in _storeService.GetAll().Where(x => x.StoreId == ((int?)documentThruJournalHeader.StoreId ?? (int?)documentThruCostCenterJournalDetail.StoreId)).DefaultIfEmpty()
                                     from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId).DefaultIfEmpty()
                                     from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId).DefaultIfEmpty()
                                     where (fromDate == null || ((DateTime?)documentThruCostCenterJournalDetail.DocumentDate ?? documentThruJournalHeader.DocumentDate) >= fromDate) &&
										   (toDate == null || ((DateTime?)documentThruCostCenterJournalDetail.DocumentDate ?? documentThruJournalHeader.DocumentDate) <= toDate)
                                     orderby ((DateTime?)documentThruCostCenterJournalDetail.DocumentDate ?? documentThruJournalHeader.DocumentDate),
                                             ((DateTime?)documentThruCostCenterJournalDetail.EntryDate ?? documentThruJournalHeader.EntryDate),
                                             (costCenterJournalDetail.MenuCode ?? documentThruJournalHeader.MenuCode),
                                             ((int?)documentThruCostCenterJournalDetail.InvoiceId ?? documentThruJournalHeader.InvoiceId)
									 select new CostCenterJournalReportDto
                                     {
                                         MenuCode = costCenterJournalDetail.MenuCode ?? documentThruJournalHeader.MenuCode,
                                         MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
                                         HeaderId = (int?)documentThruCostCenterJournalDetail.InvoiceId ?? documentThruJournalHeader.InvoiceId,

										 DocumentFullCode = documentThruCostCenterJournalDetail.FullInvoiceCode ?? documentThruJournalHeader.FullInvoiceCode,
                                         DocumentDate = (DateTime?)documentThruCostCenterJournalDetail.DocumentDate ?? documentThruJournalHeader.DocumentDate,
                                         EntryDate = (DateTime?)documentThruCostCenterJournalDetail.EntryDate ?? documentThruJournalHeader.EntryDate,

                                         CostCenterId = costCenter.CostCenterId,
                                         CostCenterCode = costCenter.CostCenterCode,
                                         CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,

                                         AccountCode = item.ItemCode ?? account.AccountCode,
                                         AccountName = (item.ItemTypeId == ItemTypeData.Note ? documentThruCostCenterJournalDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn)) ??
                                                       (language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn),

                                         CreditValue = debitOnly ? 0 : costCenterJournalDetail.CreditValue,
										 DebitValue = costCenterJournalDetail.DebitValue,
                                         ProfitAndLossValue = (debitOnly ? 0 : costCenterJournalDetail.CreditValue) - costCenterJournalDetail.DebitValue,

                                         Quantity = (decimal?)documentThruCostCenterJournalDetail.Quantity,
                                         Price = (decimal?)documentThruCostCenterJournalDetail.Price,
                                         Reference = documentThruCostCenterJournalDetail.Reference ?? documentThruJournalHeader.Reference,
                                         Remarks = language == LanguageCode.Arabic ? costCenterJournalDetail.RemarksAr : costCenterJournalDetail.RemarksEn,

                                         StoreId = store.StoreId,
                                         StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                                         BranchId = branch.BranchId,
                                         BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
										 CompanyId = company.CompanyId,
										 CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,

                                         CreatedAt = costCenterJournalDetail.CreatedAt,
                                         UserNameCreated = costCenterJournalDetail.UserNameCreated,
                                         ModifiedAt = costCenterJournalDetail.ModifiedAt,
                                         UserNameModified = costCenterJournalDetail.UserNameModified,
									 };

			return costCenterJournals;
        }
    }
}
