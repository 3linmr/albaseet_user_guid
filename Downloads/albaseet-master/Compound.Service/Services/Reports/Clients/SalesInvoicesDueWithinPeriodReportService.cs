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
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.CoreOne.Contracts.Reports.Clients;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Extensions;

namespace Compound.Service.Services.Reports.Clients
{
    public class SalesInvoicesDueWithinPeriodReportService : ISalesInvoicesDueWithinPeriodReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnSettledOrSettledSalesInvoicesReportService _unSettledOrSettledSalesInvoicesReportService;
     

        public SalesInvoicesDueWithinPeriodReportService(IUnSettledOrSettledSalesInvoicesReportService unSettledOrSettledSalesInvoicesReportService,IHttpContextAccessor httpContextAccessor,ISellerService sellerService)
        {
            _unSettledOrSettledSalesInvoicesReportService = unSettledOrSettledSalesInvoicesReportService;
            _httpContextAccessor = httpContextAccessor;
        
        }

        public IQueryable<SalesInvoicesDueWithinPeriodReportDto> GetSalesInvoicesDueWithinPeriodReport(List<int> storeIds, int periodDays, int? clientId, int? sellerId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var today = DateHelper.GetDateTimeNow().Date;
            var toDate = today.AddDays(periodDays);

            return from salesInvoicesDue in _unSettledOrSettledSalesInvoicesReportService.GetUnSettledOrSettledSalesInvoicesReport(storeIds, null, toDate, clientId, sellerId, false)
                   select new SalesInvoicesDueWithinPeriodReportDto
                   {
                           SalesInvoiceHeaderId = salesInvoicesDue.SalesInvoiceHeaderId,
                           DocumentFullCode = salesInvoicesDue.DocumentFullCode,
                           BranchName = salesInvoicesDue.BranchName,
                           CompanyName = salesInvoicesDue.CompanyName,
                           StoreId = salesInvoicesDue.StoreId,
                           StoreName = salesInvoicesDue.StoreName,
                           SellerId = salesInvoicesDue.SellerId,
                           SellerCode = salesInvoicesDue.SellerCode,
                           SellerName = salesInvoicesDue.SellerName,
                           CreatedAt = salesInvoicesDue.CreatedAt,
                           UserNameCreated = salesInvoicesDue.UserNameCreated,
                           ModifiedAt = salesInvoicesDue.ModifiedAt,
                           UserNameModified = salesInvoicesDue.UserNameModified,
                           DueDate = salesInvoicesDue.DueDate,
                           MenuCode = salesInvoicesDue.MenuCode,
                           MenuName = salesInvoicesDue.MenuName,
                           DocumentDate = salesInvoicesDue.DocumentDate,
                           EntryDate = salesInvoicesDue.EntryDate,
                           ClientId = salesInvoicesDue.ClientId,
                           ClientCode = salesInvoicesDue.ClientCode,
                           ClientName = salesInvoicesDue.ClientName,
                           AccountId = salesInvoicesDue.AccountId,
                           AccountCode = salesInvoicesDue.AccountCode,
                           AccountName = salesInvoicesDue.AccountName,
                           InvoiceValue = salesInvoicesDue.InvoiceValue,
                           SettledValue = salesInvoicesDue.SettledValue ,
                           RemainingValue = salesInvoicesDue.InvoiceValue - (salesInvoicesDue.SettledValue ),
                           RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, salesInvoicesDue.DueDate) ?? 0, 0),
                           InvoiceDuration = Math.Max(EF.Functions.DateDiffDay(salesInvoicesDue.DocumentDate, salesInvoicesDue.DueDate) ?? 0, 0),
                           Age = Math.Max(EF.Functions.DateDiffDay(salesInvoicesDue.DueDate, today) ?? 0, 0),
                           Reference = salesInvoicesDue.Reference,
                           RemarksAr = salesInvoicesDue.RemarksAr,
                           RemarksEn = salesInvoicesDue.RemarksEn,
                       
                       };
        }
    }
}
