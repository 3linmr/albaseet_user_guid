using Accounting.CoreOne.Contracts;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Contracts.Reports.Suppliers;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Compound.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Models.Dtos.Reports.Suppliers;
using Compound.Service.Services.InvoiceSettlement;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
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

namespace Compound.Service.Services.Reports.Suppliers
{
    public class SuppliersExceedCreditLimitReportService : ISuppliersExceedCreditLimitReportService
    {
        private readonly IAccountCategoryService _accountCategoryService;
        private readonly IJournalDetailService _journalDetailService;
        private readonly IAccountService _accountService;
        private readonly IJournalHeaderService _journalHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAgeingPurchaseInvoiceReportService _ageingPurchaseInvoiceReportService;
        private readonly ISupplierService _supplierService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IMenuService _menuService;
        private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
        private readonly IPurchaseInvoiceSettlementService _purchaseInvoiceSettlementService;
        private readonly IPurchaseValueService _purchaseValueService;
        private readonly ISellerService _sellerService;

		public SuppliersExceedCreditLimitReportService(IAccountCategoryService accountCategoryService, IJournalDetailService journalDetailService, IAccountService accountService, IJournalHeaderService journalHeaderService, IHttpContextAccessor httpContextAccessor,
			IAgeingPurchaseInvoiceReportService ageingPurchaseInvoiceReportService, ISupplierService supplierService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IMenuService menuService, IPaymentVoucherHeaderService paymentVoucherHeaderService, IPurchaseInvoiceSettlementService purchaseInvoiceSettlementService, IPurchaseValueService purchaseValueService, ISellerService sellerService)
		{
			_accountCategoryService = accountCategoryService;
			_journalDetailService = journalDetailService;
			_accountService = accountService;
			_journalHeaderService = journalHeaderService;
			_httpContextAccessor = httpContextAccessor;
			_ageingPurchaseInvoiceReportService = ageingPurchaseInvoiceReportService;
			_supplierService = supplierService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_menuService = menuService;
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_purchaseInvoiceSettlementService = purchaseInvoiceSettlementService;
			_purchaseValueService = purchaseValueService;
			_sellerService = sellerService;
		}

		public IQueryable<SuppliersExceedCreditLimitDto> GetSuppliersExceedCreditLimitReport(int companyId, int? supplierId, bool isDay)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return GetSuppliersExceedCreditLimit(companyId,supplierId, isDay);
        }

        private IQueryable<SuppliersExceedCreditLimitDto> GetSuppliersExceedCreditLimit(int companyId, int? supplierId, bool isDay)
        {
            var today = DateHelper.GetDateTimeNow();

            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var query = from supplier in _supplierService.GetAllSuppliers().Where(x => x.CompanyId == companyId && (supplierId == null || x.SupplierId == supplierId))
						from account in _accountService.GetAll().Where(x => x.AccountId == supplier.AccountId)
                        from journalDetail in _journalDetailService.GetAll().Where(x => x.AccountId == account.AccountId)
                        from lastPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.SupplierId==supplier.SupplierId && x.CreditPayment).OrderByDescending(x=> x.DocumentDate).Take(1).DefaultIfEmpty()
                        from lastUnSettledPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.SupplierId==supplier.SupplierId && x.CreditPayment && x.IsSettlementCompleted == false).OrderByDescending(x=> x.DocumentDate).Take(1).DefaultIfEmpty()
                        from lastInvoiceValue in _purchaseValueService.GetOverallValueOfPurchaseInvoices().Where(x => x.PurchaseInvoiceHeaderId == lastPurchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        from purchaseInvoiceSettleValue in _purchaseInvoiceSettlementService.GetPurchaseInvoiceSettledValues().Where(x => x.PurchaseInvoiceHeaderId == lastPurchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        from paymentVoucherHeader in _paymentVoucherHeaderService.GetAll().Where(x => x.SupplierId == supplier.SupplierId).OrderByDescending(x => x.DocumentDate).Take(1).DefaultIfEmpty()
                        from lastSettlementValue in _purchaseInvoiceSettlementService.GetPaymentVoucherSettledValues().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeader.PaymentVoucherHeaderId).DefaultIfEmpty()
                        group new { journalDetail, lastPurchaseInvoiceHeader, paymentVoucherHeader} by new
                        {
                            account.AccountId,
                            AccountCode = account.AccountCode ?? "",
                            account.AccountLevel,
                            supplier.SupplierId,
                            supplier.SupplierCode,
                            supplier.SupplierNameAr,
                            supplier.SupplierNameEn,
                            MainAccountId = account.MainAccountId ?? 0,
                            AccountNameAr = account.AccountNameAr ?? "",
                            AccountNameEn = account.AccountNameEn ?? "",
                            CreditLimitValues = supplier.CreditLimitValues,
                            CreditLimitDays = supplier.CreditLimitDays,
                            lastInvoiceFullCode = lastPurchaseInvoiceHeader.Prefix + lastPurchaseInvoiceHeader.DocumentCode + lastPurchaseInvoiceHeader.Suffix,
                            lastInvoiceValue = lastInvoiceValue.OverallValue,
                            LastInvoiceDate = lastPurchaseInvoiceHeader.DocumentDate,
							LastUnSettledInvoiceDate = lastUnSettledPurchaseInvoiceHeader.DocumentDate,
							LastUnSettledDueDate = lastUnSettledPurchaseInvoiceHeader.DueDate,
							LastPaymentVoucherFullCode = paymentVoucherHeader.Prefix + paymentVoucherHeader.PaymentVoucherCode + paymentVoucherHeader.Suffix,
                            LastPaymentVoucherValue = lastSettlementValue.SettledValue ?? 0,
                            lastPurchaseInvoiceHeader.DueDate,

                            supplier.ContractDate,
							supplier.TaxCode,
							supplier.ShipmentTypeId,
							supplier.ShipmentTypeName,
							supplier.CountryId,
							supplier.CountryName,
							supplier.StateId,
							supplier.StateName,
							supplier.CityId,
							supplier.CityName,
							supplier.DistrictId,
							supplier.DistrictName,
							supplier.PostalCode,
							supplier.BuildingNumber,
							supplier.CommercialRegister,
							supplier.Street1,
							supplier.Street2,
							supplier.AdditionalNumber,
							supplier.CountryCode,
							supplier.Address1,
							supplier.Address2,
							supplier.Address3,
							supplier.Address4,
							supplier.FirstResponsibleName,
							supplier.FirstResponsiblePhone,
							supplier.FirstResponsibleEmail,
							supplier.SecondResponsibleName,
							supplier.SecondResponsiblePhone,
							supplier.SecondResponsibleEmail,
							supplier.IsCredit,
							supplier.IsActive,
							supplier.IsActiveName,
							supplier.InActiveReasons,

                            supplier.CreatedAt,
                            supplier.UserNameCreated,
                            supplier.ModifiedAt,
                            supplier.UserNameModified,

                        } into g
                        // Filter the results to include only those where the calculated Balance exceeds the CreditLimit
                        where  !isDay ? g.Sum(x => x.journalDetail.CreditValue - x.journalDetail.DebitValue) > g.Key.CreditLimitValues :
                                        (g.Key.LastUnSettledDueDate != null && g.Key.LastUnSettledDueDate.Value.AddDays(g.Key.CreditLimitDays) < today)
                        select new SuppliersExceedCreditLimitDto
                        {
                            AccountId = g.Key.AccountId,
                            AccountName = language == LanguageData.LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,

                            SupplierId = g.Key.SupplierId,
                            SupplierCode = g.Key.SupplierCode,
                            SupplierName = language == LanguageData.LanguageCode.Arabic ? g.Key.SupplierNameAr : g.Key.SupplierNameEn,

                            LastInvoiceFullCode = g.Key.lastInvoiceFullCode,
                            LastInvoiceDate = g.Key.LastInvoiceDate,
                            LastInvoiceValue = g.Key.lastInvoiceValue,

                            LastPaymentVoucherFullCode = g.Key.LastPaymentVoucherFullCode,
                            LastPaymentVoucherValue = g.Key.LastPaymentVoucherValue,

                            Balance = g.Sum(x => x.journalDetail.CreditValue - x.journalDetail.DebitValue),
                            CreditLimitValues = g.Key.CreditLimitValues,
                            OverdueAmount = g.Sum(x => x.journalDetail.CreditValue - x.journalDetail.DebitValue) - g.Key.CreditLimitValues,

                            InvoiceDuration = EF.Functions.DateDiffDay(g.Key.LastUnSettledInvoiceDate, g.Key.LastUnSettledDueDate),
                            InvoiceDueDate = g.Key.LastUnSettledDueDate,
                            CreditLimitDays = g.Key.CreditLimitDays,
                            InvoiceDueDateFinal = g.Key.LastUnSettledDueDate != null ? g.Key.LastUnSettledDueDate.Value.AddDays(g.Key.CreditLimitDays) : null,
                            DaysOverdue = g.Key.LastUnSettledDueDate != null  ? Math.Max( EF.Functions.DateDiffDay(g.Key.LastUnSettledDueDate.Value.AddDays(g.Key.CreditLimitDays), today), 0 ) : 0,

                            ContractDate = g.Key.ContractDate,
							TaxCode = g.Key.TaxCode,
							ShipmentTypeId = g.Key.ShipmentTypeId,
							ShipmentTypeName = g.Key.ShipmentTypeName,
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
							Street1 = g.Key.Street1,
							Street2 = g.Key.Street2,
							AdditionalNumber = g.Key.AdditionalNumber,
							CountryCode = g.Key.CountryCode,
							Address1 = g.Key.Address1,
							Address2 = g.Key.Address2,
							Address3 = g.Key.Address3,
							Address4 = g.Key.Address4,
							FirstResponsibleName = g.Key.FirstResponsibleName,
							FirstResponsiblePhone = g.Key.FirstResponsiblePhone,
							FirstResponsibleEmail = g.Key.FirstResponsibleEmail,
							SecondResponsibleName = g.Key.SecondResponsibleName,
							SecondResponsiblePhone = g.Key.SecondResponsiblePhone,
							SecondResponsibleEmail = g.Key.SecondResponsibleEmail,
							IsCredit = g.Key.IsCredit,
							IsActive = g.Key.IsActive,
							IsActiveName = g.Key.IsActiveName,
							InActiveReasons = g.Key.InActiveReasons,

                            CreatedAt = g.Key.CreatedAt,
                            UserNameCreated = g.Key.UserNameCreated,
                            ModifiedAt = g.Key.ModifiedAt,
                            UserNameModified = g.Key.UserNameModified,
                        };

            return query;
        }
    }
}
