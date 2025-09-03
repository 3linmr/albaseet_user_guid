using Accounting.CoreOne.Contracts;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Contracts.Reports.SellerCommission;
using Compound.CoreOne.Models.Dtos.Reports.SellerCommissions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.CoreOne.Models.StaticData.StaticData;

namespace Compound.Service.Services.Reports.SellerCommission
{
	public class SellerCommissionReportService : ISellerCommissionReportService
	{
		private readonly ISellerCommissionService _sellerCommissionService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISellerCommissionMethodService _sellerCommissionMethodService;
		private readonly ISellerCommissionTypeService _sellerCommissionTypeService;
		private readonly ISellerService _sellerService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly ISalesValueService _salesValueService;
		private readonly IMenuService _menuService;
		private readonly IClientService _clientService;
		private readonly IStoreService _storeService;
		private readonly IInvoiceTypeService _invoiceTypeService;
		private readonly IAccountService _accountService;
		private readonly IStringLocalizer<SellerCommissionReportService> _localizer;
		private readonly ICurrencyService _currencyService;
		private readonly ICompanyService _companyService;

		public SellerCommissionReportService(ISellerCommissionService sellerCommissionService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISellerCommissionMethodService sellerCommissionMethodService, ISellerCommissionTypeService sellerCommissionTypeService, ISellerService sellerService, IHttpContextAccessor httpContextAccessor, ISalesInvoiceSettlementService salesInvoiceSettlementService, IReceiptVoucherHeaderService receiptVoucherHeaderService, ISalesValueService salesValueService, IMenuService menuService, IClientService clientService, IStoreService storeService, IInvoiceTypeService invoiceTypeService, IAccountService accountService, IStringLocalizer<SellerCommissionReportService> localizer, ICurrencyService currencyService, ICompanyService companyService)
		{
			_sellerCommissionService = sellerCommissionService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_sellerCommissionMethodService = sellerCommissionMethodService;
			_sellerCommissionTypeService = sellerCommissionTypeService;
			_sellerService = sellerService;
			_httpContextAccessor = httpContextAccessor;
			_salesInvoiceSettlementService = salesInvoiceSettlementService;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_salesValueService = salesValueService;
			_menuService = menuService;
			_clientService = clientService;
			_storeService = storeService;
			_invoiceTypeService = invoiceTypeService;
			_accountService = accountService;
			_localizer = localizer;
			_currencyService = currencyService;
			_companyService = companyService;;
		}

		public IQueryable<SellerCommissionReportDto> GetSellerCommissionReport(int companyId, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var cashIncomes = GetTotalCashIncomes(fromDate, toDate);
			var documents = GetSalesInvoicesAndReceiptVouchers(fromDate, toDate);

			var fromString = _localizer["From"];
			var toString = _localizer["To"];
			var daysString = _localizer["Days"];

			return from document in documents
				   from seller in _sellerService.GetAllSellers().Where(x => x.SellerId == document.SellerId && x.CompanyId == companyId)
				   from sellerCommissionMethod in _sellerCommissionMethodService.GetAll().Where(x => x.SellerCommissionMethodId == seller.SellerCommissionMethodId)
				   from sellerCommissionType in _sellerCommissionTypeService.GetAll().Where(x => x.SellerCommissionTypeId == sellerCommissionMethod.SellerCommissionTypeId)
				   from cashIncome in cashIncomes.Where(x => x.SellerId == seller.SellerId).DefaultIfEmpty() //left join should not be needed
				   from company in _companyService.GetAll().Where(x => x.CompanyId == seller.CompanyId)
				   from currency in _currencyService.GetAll().Where(x => x.CurrencyId == company.CurrencyId)
				   from cashIncomeSellerCommission in _sellerCommissionService.GetAll()
				        .Where(x => sellerCommissionType.SellerCommissionTypeId == SellerCommissionTypeData.CashIncome && x.SellerCommissionMethodId == sellerCommissionMethod.SellerCommissionMethodId && x.From <= Math.Round((decimal?)cashIncome.CashIncome ?? 0)).OrderByDescending(x => x.From).Take(1).DefaultIfEmpty()
				   from ageOfDebtSellerCommission in _sellerCommissionService.GetAll()
					    .Where(x => sellerCommissionType.SellerCommissionTypeId == SellerCommissionTypeData.AgeOfDebt && x.SellerCommissionMethodId == sellerCommissionMethod.SellerCommissionMethodId && x.From <= document.AgeOfDebt).OrderByDescending(x => x.From).Take(1).DefaultIfEmpty()
				   orderby document.DocumentDate, document.EntryDate
				   select new SellerCommissionReportDto
				   {
					   HeaderId = document.HeaderId,
					   MenuCode = document.MenuCode,
					   MenuName = document.MenuName,
					   DocumentFullCode = document.DocumentFullCode,
					   SalesInvoiceHeaderId = document.SalesInvoiceHeaderId,
					   SalesInvoiceMenuCode = document.SalesInvoiceMenuCode,
					   SalesInvoiceMenuName = document.SalesInvoiceMenuName,
					   SalesInvoiceFullCode = document.SalesInvoiceFullCode,
					   DocumentDate = document.DocumentDate,
					   EntryDate = document.EntryDate,

					   SellerId = document.SellerId,
					   SellerCode = document.SellerCode,
					   SellerName = document.SellerName,

					   SellerCommissionMethodId = sellerCommissionMethod.SellerCommissionMethodId,
					   SellerCommissionMethodCode = sellerCommissionMethod.SellerCommissionMethodCode,
					   SellerCommissionMethodName = language == LanguageCode.Arabic ?
						   sellerCommissionMethod.SellerCommissionMethodNameAr + " (" + sellerCommissionType.SellerCommissionTypeNameAr + ")" :
						   sellerCommissionMethod.SellerCommissionMethodNameEn + " (" + sellerCommissionType.SellerCommissionTypeNameEn + ")",

					   CollectedValue = document.CollectedValue,

					   TotalCashIncome = (decimal?)cashIncome.CashIncome ?? 0,
					   AgeOfDebt = document.AgeOfDebt,
					   SellerCommissionId = sellerCommissionMethod.SellerCommissionTypeId == SellerCommissionTypeData.CashIncome? 
							cashIncomeSellerCommission.SellerCommissionId :
							ageOfDebtSellerCommission.SellerCommissionId,
					   CommissionRange = sellerCommissionMethod.SellerCommissionTypeId == SellerCommissionTypeData.CashIncome? 
							(cashIncomeSellerCommission.From != null ? fromString+" "+cashIncomeSellerCommission.From.ToString()+" "+currency.Symbol+" "+toString+" "+cashIncomeSellerCommission.To.ToString()+" "+currency.Symbol : null) :
							(ageOfDebtSellerCommission.From != null ? fromString+" "+ageOfDebtSellerCommission.From.ToString()+" "+daysString+" "+toString+" "+ageOfDebtSellerCommission.To.ToString()+" "+daysString : null),
					   CommissionPercent = sellerCommissionMethod.SellerCommissionTypeId == SellerCommissionTypeData.CashIncome? 
							cashIncomeSellerCommission.CommissionPercent != null ? (cashIncomeSellerCommission.CommissionPercent.ToString() + "%") : null :
							ageOfDebtSellerCommission.CommissionPercent != null ? (ageOfDebtSellerCommission.CommissionPercent.ToString() + "%") : null,
					   CommissionValue = sellerCommissionMethod.SellerCommissionTypeId == SellerCommissionTypeData.CashIncome? 
							cashIncomeSellerCommission.CommissionPercent != null ? (cashIncomeSellerCommission.CommissionPercent / 100) * document.CollectedValue : null :
							ageOfDebtSellerCommission.CommissionPercent != null ? (ageOfDebtSellerCommission.CommissionPercent / 100) * document.CollectedValue : null,

					   InvoiceValue = document.InvoiceValue,
					   InvoiceSettledValue = document.InvoiceSettledValue,
					   SettleValue = document.SettleValue,

					   ClientId = document.ClientId,
					   ClientCode = document.ClientCode,
					   ClientName = document.ClientName,
					   StoreId = document.StoreId,
					   StoreName = document.StoreName,
					   Reference = document.Reference,
					   InvoiceTypeId = document.InvoiceTypeId,
					   InvoiceTypeName = document.InvoiceTypeName,
					   DueDate = document.DueDate,
					   RemarksAr = document.RemarksAr,
					   RemarksEn = document.RemarksEn,

					   SellerTypeName = seller.SellerTypeName,
					   ContractDate = seller.ContractDate,
					   OutSourcingCompany = seller.OutSourcingCompany,
					   Phone = seller.Phone,
					   WhatsApp = seller.WhatsApp,
					   Address = seller.Address,
					   Email = seller.Email,
					   ClientsDebitLimit = seller.ClientsDebitLimit,
					   IsActive = seller.IsActive,
					   IsActiveName = seller.IsActiveName,
					   InActiveReasons = seller.InActiveReasons,

					   CreatedAt = document.CreatedAt,
					   UserNameCreated = document.UserNameCreated,
					   ModifiedAt = document.ModifiedAt,
					   UserNameModified = document.UserNameModified,
				   };
		}

		private IQueryable<SellerCommissionReportDto> GetSalesInvoicesAndReceiptVouchers(DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var salesInvoiceHeaders = from salesValue in _salesValueService.GetOverallValueOfSalesInvoicesWithDateRange(fromDate, toDate)
									  from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesValue.SalesInvoiceHeaderId)
									  from settleValue in _salesInvoiceSettlementService.GetSalesInvoiceSettledValues().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
									  from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
									  from client in _clientService.GetAll().Where(x => x.ClientId == salesInvoiceHeader.ClientId)
									  from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
									  from invoiceType in _invoiceTypeService.GetAll().Where(x => x.InvoiceTypeId == salesInvoiceHeader.InvoiceTypeId)
									  from seller in _sellerService.GetAll().Where(x => x.SellerId == salesInvoiceHeader.SellerId)
									  select new SellerCommissionReportDto
									  {
										  HeaderId = salesValue.SalesInvoiceHeaderId,
										  MenuCode = salesInvoiceHeader.MenuCode,
										  MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
										  DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
										  SalesInvoiceHeaderId = null,
										  SalesInvoiceMenuCode = null,
										  SalesInvoiceMenuName = null,
										  SalesInvoiceFullCode = null,

										  SellerId = seller.SellerId,
										  SellerCode = seller.SellerCode,
										  SellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,

										  InvoiceValue = salesValue.OverallNetValue,
										  InvoiceSettledValue = salesInvoiceHeader.CreditPayment ? (settleValue.SettledValue ?? 0.0M) : null,

										  AgeOfDebt = null,
										  SettleValue = null,
										  CollectedValue = salesInvoiceHeader.CreditPayment ? 0.0M : salesValue.OverallNetValue,

										  ClientId = salesInvoiceHeader.ClientId,
										  ClientCode = client.ClientCode,
										  ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
										  StoreId = salesInvoiceHeader.StoreId,
										  StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
										  DocumentDate = salesInvoiceHeader.DocumentDate,
										  EntryDate = salesInvoiceHeader.EntryDate,
										  Reference = salesInvoiceHeader.Reference,
										  InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
										  InvoiceTypeName = language == LanguageCode.Arabic ? invoiceType.InvoiceTypeNameAr : invoiceType.InvoiceTypeNameEn,
										  DueDate = salesInvoiceHeader.DueDate,
										  RemarksAr = salesInvoiceHeader.RemarksAr,
										  RemarksEn = salesInvoiceHeader.RemarksEn,

										  CreatedAt = salesInvoiceHeader.CreatedAt,
										  UserNameCreated = salesInvoiceHeader.UserNameCreated,
										  ModifiedAt = salesInvoiceHeader.ModifiedAt,
										  UserNameModified = salesInvoiceHeader.UserNameModified,
									  };

			var receiptVoucherHeaders = from salesInvoiceSettlement in _salesInvoiceSettlementService.GetAll()
										from receiptVoucherHeader in _receiptVoucherHeaderService.GetAll().Where(x => x.ReceiptVoucherHeaderId == salesInvoiceSettlement.ReceiptVoucherHeaderId)
										from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceSettlement.SalesInvoiceHeaderId)
										from salesValue in _salesValueService.GetOverallValueOfSalesInvoicesWithDateRange(fromDate, toDate).Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
										from settleValue in _salesInvoiceSettlementService.GetSalesInvoiceSettledValues().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
										from voucherMenu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ReceiptVoucher)
										from salesInvoiceMenu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode).DefaultIfEmpty()
										from client in _clientService.GetAll().Where(x => x.ClientId == receiptVoucherHeader.ClientId)
										from store in _storeService.GetAll().Where(x => x.StoreId == receiptVoucherHeader.StoreId)
										from seller in _sellerService.GetAll().Where(x => x.SellerId == receiptVoucherHeader.SellerId)
										where (fromDate == null || receiptVoucherHeader.DocumentDate >= fromDate) &&
	                                          (toDate == null || receiptVoucherHeader.DocumentDate <= toDate)
										select new SellerCommissionReportDto
										{
											HeaderId = receiptVoucherHeader.ReceiptVoucherHeaderId,
											MenuCode = voucherMenu.MenuCode,
											MenuName = language == LanguageCode.Arabic ? voucherMenu.MenuNameAr : voucherMenu.MenuNameEn,
											DocumentFullCode = receiptVoucherHeader.Prefix + receiptVoucherHeader.ReceiptVoucherCode + receiptVoucherHeader.Suffix,
											SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
											SalesInvoiceMenuCode = salesInvoiceHeader.MenuCode,
											SalesInvoiceMenuName = language == LanguageCode.Arabic ? salesInvoiceMenu.MenuNameAr : salesInvoiceMenu.MenuNameEn,
											SalesInvoiceFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,

											SellerId = seller.SellerId,
											SellerCode = seller.SellerCode,
											SellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn,

											InvoiceValue = salesValue.OverallNetValue,
										    InvoiceSettledValue = salesInvoiceHeader.CreditPayment ? (settleValue.SettledValue ?? 0.0M) : null,

											AgeOfDebt = EF.Functions.DateDiffDay(salesInvoiceHeader.DocumentDate, receiptVoucherHeader.DocumentDate),
											SettleValue = salesInvoiceSettlement.SettleValue,
											CollectedValue = salesInvoiceSettlement.SettleValue,

											ClientId = (int)receiptVoucherHeader.ClientId!,
											ClientCode = client.ClientCode,
											ClientName = language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn,
											StoreId = receiptVoucherHeader.StoreId,
											StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
											DocumentDate = receiptVoucherHeader.DocumentDate,
											EntryDate = receiptVoucherHeader.EntryDate,
											Reference = receiptVoucherHeader.PeerReference,
											InvoiceTypeId = null,
											InvoiceTypeName = null,
											DueDate = null,
											RemarksAr = salesInvoiceSettlement.RemarksAr,
											RemarksEn = salesInvoiceSettlement.RemarksEn,

										    CreatedAt = receiptVoucherHeader.CreatedAt,
										    UserNameCreated = receiptVoucherHeader.UserNameCreated,
										    ModifiedAt = receiptVoucherHeader.ModifiedAt,
										    UserNameModified = receiptVoucherHeader.UserNameModified,
										};

			return salesInvoiceHeaders.Concat(receiptVoucherHeaders);
		}

		private IQueryable<SellerCashIncomeDto> GetTotalCashIncomes(DateTime? fromDate, DateTime? toDate)
		{
			var cashSalesInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SellerId != null)
									where (
										   salesInvoiceHeader.SellerId != null &&
										!salesInvoiceHeader.CreditPayment &&
										(fromDate == null || salesInvoiceHeader.DocumentDate >= fromDate) &&
										(toDate == null || salesInvoiceHeader.DocumentDate <= toDate)
									)
									select new SellerCashIncomeDto
									{
										SellerId = (int)salesInvoiceHeader.SellerId!,
										CashIncome = salesInvoiceHeader.NetValue,
									};

			var cashSalesInvoiceReturns = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SellerId != null)
										  where (
											  salesInvoiceReturnHeader.SellerId != null &&
											  !salesInvoiceReturnHeader.CreditPayment &&
											  (fromDate == null || salesInvoiceReturnHeader.DocumentDate >= fromDate) &&
											  (toDate == null || salesInvoiceReturnHeader.DocumentDate <= toDate)
										  )
										  select new SellerCashIncomeDto
										  {
											  SellerId = (int)salesInvoiceReturnHeader.SellerId!,
											  CashIncome = -salesInvoiceReturnHeader.NetValue,
										  };

			var salesInvoiceSettlements = from receiptVoucherHeader in _receiptVoucherHeaderService.GetAll()
										  from salesInvoiceSettlement in _salesInvoiceSettlementService.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeader.ReceiptVoucherHeaderId)
										  where (
											  receiptVoucherHeader.SellerId != null &&
											  (fromDate == null || receiptVoucherHeader.DocumentDate >= fromDate) &&
											  (toDate == null || receiptVoucherHeader.DocumentDate <= toDate)
										  )
										  select new SellerCashIncomeDto
										  {
											  SellerId = (int)receiptVoucherHeader.SellerId!,
											  CashIncome = salesInvoiceSettlement.SettleValue,
										  };

			var cashIncomes = from cashIncome in cashSalesInvoices.Concat(cashSalesInvoiceReturns).Concat(salesInvoiceSettlements)
							  group cashIncome by cashIncome.SellerId into g
							  select new SellerCashIncomeDto
							  {
								  SellerId = g.Key,
								  CashIncome = g.Sum(x => x.CashIncome)
							  };

			return cashIncomes;
		}
	}
}
