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
using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;

namespace Compound.Service.Services.Reports.Suppliers
{
    public class PurchaseInvoicesDueWithinPeriodReportService : IPurchaseInvoicesDueWithinPeriodReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnSettledOrSettledPurchaseInvoiceReportService _unSettledOrSettledPurchaseInvoicesReportService;

        public PurchaseInvoicesDueWithinPeriodReportService(IUnSettledOrSettledPurchaseInvoiceReportService unSettledOrSettledPurchaseInvoicesReportService,IHttpContextAccessor httpContextAccessor)
        {
            _unSettledOrSettledPurchaseInvoicesReportService = unSettledOrSettledPurchaseInvoicesReportService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<PurchaseInvoicesDueWithinPeriodReportDto> GetPurchaseInvoicesDueWithinPeriodReport(List<int> storeIds, int periodDays, int? supplierId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var today = DateHelper.GetDateTimeNow().Date;
            var toDate = today.AddDays(periodDays);

            return from purchaseInvoicesDue in _unSettledOrSettledPurchaseInvoicesReportService.GetUnSettledOrSettledPurchaseInvoicesReport(storeIds, null, toDate, supplierId, false)
                   select new PurchaseInvoicesDueWithinPeriodReportDto
                   {
                       PurchaseInvoiceHeaderId = purchaseInvoicesDue.PurchaseInvoiceHeaderId,
                       DocumentFullCode = purchaseInvoicesDue.DocumentFullCode,
                       MenuCode = purchaseInvoicesDue.MenuCode,
					   MenuName = purchaseInvoicesDue.MenuName,
                       DocumentDate = purchaseInvoicesDue.DocumentDate,
					   EntryDate = purchaseInvoicesDue.EntryDate,
                       SupplierId = purchaseInvoicesDue.SupplierId,
                       SupplierCode = purchaseInvoicesDue.SupplierCode,
                       SupplierName = purchaseInvoicesDue.SupplierName,
                       AccountId = purchaseInvoicesDue.AccountId,
					   AccountCode = purchaseInvoicesDue.AccountCode,
					   AccountName = purchaseInvoicesDue.AccountName,
					   InvoiceValue = purchaseInvoicesDue.InvoiceValue,
                       SettledValue = purchaseInvoicesDue.SettledValue ,
                       RemainingValue = purchaseInvoicesDue.InvoiceValue - ( purchaseInvoicesDue.SettledValue ),
                       StoreId = purchaseInvoicesDue.StoreId,
                       StoreName = purchaseInvoicesDue.StoreName,
                       BranchId = purchaseInvoicesDue.BranchId,
					   BranchName = purchaseInvoicesDue.BranchName,
                       CompanyId = purchaseInvoicesDue.CompanyId,
					   CompanyName = purchaseInvoicesDue.CompanyName,
                       DueDate = purchaseInvoicesDue.DueDate,
                       Age = Math.Max(EF.Functions.DateDiffDay(purchaseInvoicesDue.DueDate, today) ?? 0, 0),
                       Reference = purchaseInvoicesDue.Reference,
                       RemarksAr = purchaseInvoicesDue.RemarksAr,
                       RemarksEn = purchaseInvoicesDue.RemarksEn,
                       RemainingDays = Math.Max(EF.Functions.DateDiffDay(today,purchaseInvoicesDue.DueDate) ?? 0, 0),
					   InvoiceDuration = purchaseInvoicesDue.InvoiceDuration,

					   CreatedAt = purchaseInvoicesDue.CreatedAt,
                       UserNameCreated = purchaseInvoicesDue.UserNameCreated,
                       ModifiedAt = purchaseInvoicesDue.ModifiedAt,
                       UserNameModified = purchaseInvoicesDue.UserNameModified,
                   };
        }
    }
}
