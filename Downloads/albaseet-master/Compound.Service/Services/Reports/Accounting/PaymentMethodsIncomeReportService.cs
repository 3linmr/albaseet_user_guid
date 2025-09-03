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
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Extensions;

namespace Compound.Service.Services.Reports.Accounting
{
	public class PaymentMethodsIncomeReportService : IPaymentMethodsIncomeReportService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreService _storeService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly IReceiptVoucherDetailService _receiptVoucherDetailService;
		private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly IPaymentVoucherDetailService _paymentVoucherDetailService;
		private readonly IClientService _clientService;
		private readonly IMenuService _menuService;
		private readonly ISellerService _sellerService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly ISalesInvoiceCollectionService _salesInvoiceCollectionService;
		private readonly ISalesInvoiceReturnPaymentService _salesInvoiceReturnPaymentService;
		private readonly IJournalDetailService _journalDetailService;

		public PaymentMethodsIncomeReportService(IHttpContextAccessor httpContextAccessor, IStoreService storeService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IReceiptVoucherHeaderService receiptVoucherHeaderService, IReceiptVoucherDetailService receiptVoucherDetailService, IPaymentVoucherHeaderService paymentVoucherHeaderService, IPaymentVoucherDetailService paymentVoucherDetailService, IClientService clientService, IMenuService menuService, ISellerService sellerService, IPaymentMethodService paymentMethodService, ISalesInvoiceCollectionService salesInvoiceCollectionService, ISalesInvoiceReturnPaymentService salesInvoiceReturnPaymentService, IJournalDetailService journalDetailService)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeService = storeService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_receiptVoucherDetailService = receiptVoucherDetailService;
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_paymentVoucherDetailService = paymentVoucherDetailService;
			_clientService = clientService;
			_menuService = menuService;
			_sellerService = sellerService;
			_paymentMethodService = paymentMethodService;
			_salesInvoiceCollectionService = salesInvoiceCollectionService;
			_salesInvoiceReturnPaymentService = salesInvoiceReturnPaymentService;
			_journalDetailService = journalDetailService;
		}

		public async Task<IEnumerable<PaymentMethodsIncomeReportDto>> GetPaymentMethodsIncomeReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<short> menuCodes)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var invoiceDocuments = GetCashIncomeData(storeIds, fromDate, toDate);

			var data = from invoiceDocument in invoiceDocuments
					   from store in _storeService.GetAll().Where(x => x.StoreId == invoiceDocument.StoreId)
					   from client in _clientService.GetAll().Where(x => x.ClientId == invoiceDocument.ClientId).DefaultIfEmpty()
					   from seller in _sellerService.GetAll().Where(x => x.SellerId == invoiceDocument.SellerId).DefaultIfEmpty()
					   from menu in _menuService.GetAll().Where(x => x.MenuCode == invoiceDocument.MenuCode).DefaultIfEmpty()
					   from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == invoiceDocument.PaymentMethodId)
					   where (menuCodes.Count == 0 || menuCodes.Contains(invoiceDocument.MenuCode))
					   orderby invoiceDocument.DocumentDate, invoiceDocument.EntryDate, invoiceDocument.HeaderId, invoiceDocument.MenuCode
					   select new
					   {
						   MenuCode = invoiceDocument.MenuCode,
						   MenuName = menu != null ? (language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn) : null,
						   HeaderId = invoiceDocument.HeaderId,
						   DocumentFullCode = invoiceDocument.DocumentFullCode,
						   DocumentDate = invoiceDocument.DocumentDate,
						   EntryDate = invoiceDocument.EntryDate,
						   NetValue = invoiceDocument.NetValue,
						   PaidValue = invoiceDocument.NetValue < 0 ? -invoiceDocument.NetValue : 0,
						   ReceivedValue = invoiceDocument.NetValue > 0 ? invoiceDocument.NetValue : 0,
						   ClientId = invoiceDocument.ClientId,
						   ClientName = client != null ? (language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn) : null,
						   SellerId = invoiceDocument.SellerId,
						   SellerName = seller != null ? (language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn) : null,
						   StoreId = invoiceDocument.StoreId,
						   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

						   PaymentMethodId = invoiceDocument.PaymentMethodId,
						   PaymentMethodName = language == LanguageCode.Arabic ? paymentMethod.PaymentMethodNameAr : paymentMethod.PaymentMethodNameEn,
						   PaymentTypeId = paymentMethod.PaymentTypeId,
						   PaymentMethodValue = invoiceDocument.PaymentValue,

						   BankCommissionPaid = invoiceDocument.BankCommissionPaid,

					       CreatedAt = invoiceDocument.CreatedAt,
					       UserNameCreated = invoiceDocument.UserNameCreated,
					       ModifiedAt = invoiceDocument.ModifiedAt,
					       UserNameModified = invoiceDocument.UserNameModified,
					   };

			return from d in data.AsEnumerable()
				   group d by new { d.MenuCode, d.MenuName, d.HeaderId, d.DocumentFullCode, d.DocumentDate, d.EntryDate, d.NetValue, d.PaidValue, d.ReceivedValue, d.ClientId, d.ClientName, d.SellerId, d.SellerName, d.CreatedAt, d.UserNameCreated, d.ModifiedAt, d.UserNameModified, d.StoreId, d.StoreName} into g
				   select new PaymentMethodsIncomeReportDto
				   {
					   MenuCode = g.Key.MenuCode,
					   MenuName = g.Key.MenuName,
					   HeaderId = g.Key.HeaderId,
					   DocumentFullCode = g.Key.DocumentFullCode,
					   DocumentDate = g.Key.DocumentDate,
					   EntryDate = g.Key.EntryDate,
					   NetValue = g.Key.NetValue,
					   PaidValue = g.Key.PaidValue,
					   ReceivedValue = g.Key.ReceivedValue,
					   ClientId = g.Key.ClientId,
					   ClientName = g.Key.ClientName,
					   SellerId = g.Key.SellerId,
					   SellerName = g.Key.SellerName,
					   StoreId = g.Key.StoreId,
					   StoreName = g.Key.StoreName,

					   CashPaymentValue = g.Where(x => x.PaymentTypeId == PaymentTypeData.Cash).Sum(x => x.PaymentMethodValue),
					   BankAccountPaymentValue = g.Where(x => x.PaymentTypeId == PaymentTypeData.BankAccount).Sum(x => x.PaymentMethodValue),
					   BankCardPaymentValue = g.Where(x => x.PaymentTypeId == PaymentTypeData.BankCard).Sum(x => x.PaymentMethodValue),
					   InstallmentPaymentValue = g.Where(x => x.PaymentTypeId == PaymentTypeData.Installment).Sum(x => x.PaymentMethodValue),
					   CreditTransferPaymentValue = g.Where(x => x.PaymentTypeId == PaymentTypeData.CreditTransfer).Sum(x => x.PaymentMethodValue),

					   PaidBankCommissionValue = g.Sum(x => x.BankCommissionPaid),

					   CashPaymentValueAfterCommission = g.Where(x => x.PaymentTypeId == PaymentTypeData.Cash).Sum(x => x.PaymentMethodValue - x.BankCommissionPaid),
					   BankAccountPaymentValueAfterCommission = g.Where(x => x.PaymentTypeId == PaymentTypeData.BankAccount).Sum(x => x.PaymentMethodValue - x.BankCommissionPaid),
					   BankCardPaymentValueAfterCommission = g.Where(x => x.PaymentTypeId == PaymentTypeData.BankCard).Sum(x => x.PaymentMethodValue - x.BankCommissionPaid),
					   InstallmentPaymentValueAfterCommission = g.Where(x => x.PaymentTypeId == PaymentTypeData.Installment).Sum(x => x.PaymentMethodValue - x.BankCommissionPaid),
					   CreditTransferPaymentValueAfterCommission = g.Where(x => x.PaymentTypeId == PaymentTypeData.CreditTransfer).Sum(x => x.PaymentMethodValue - x.BankCommissionPaid),

					   AllPaymentMethodsConcat = string.Join(", ", g.Select(x => x.PaymentMethodName + ": " + x.PaymentMethodValue.ToNormalizedString())),
					   AllPaymentMethodsAfterCommissionConcat = string.Join(", ", g.Select(x => x.PaymentMethodName + ": " + (x.PaymentMethodValue - x.BankCommissionPaid).ToNormalizedString())),

					   NetValueAfterCommission = g.Key.NetValue - g.Sum(x => x.BankCommissionPaid),

					   CreatedAt = g.Key.CreatedAt,
					   UserNameCreated = g.Key.UserNameCreated,
					   ModifiedAt = g.Key.ModifiedAt,
					   UserNameModified = g.Key.UserNameModified,
				   };
		}

		private IQueryable<PaymentMethodsIncomeReportDataDto> GetCashIncomeData(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var salesInvoiceHeaders = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(
										  x => storeIds.Contains(x.StoreId) &&
										  !x.CreditPayment &&
										  (fromDate == null || x.DocumentDate >= fromDate) &&
										  (toDate == null || x.DocumentDate <= toDate)
									  )
									  from salesInvoiceCollection in _salesInvoiceCollectionService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
									  from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == salesInvoiceCollection.PaymentMethodId)
									  from commission in _journalDetailService.GetAll().Where(x =>
											  x.JournalHeaderId == salesInvoiceHeader.JournalHeaderId &&
											  (x.AccountId == paymentMethod.CommissionAccountId || x.AccountId == paymentMethod.CommissionTaxAccountId) && //Bank Commission also include commission tax
											  (paymentMethod.PaymentTypeId == PaymentTypeData.BankCard || paymentMethod.PaymentTypeId == PaymentTypeData.BankAccount)
										  ).GroupBy(x => x.JournalHeaderId)
										  .Select(x => x.Sum(y => y.DebitValue)).DefaultIfEmpty()
									  select new PaymentMethodsIncomeReportDataDto
									  {
										  HeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
										  MenuCode = salesInvoiceHeader.MenuCode ?? 0,
										  DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
										  DocumentDate = salesInvoiceHeader.DocumentDate,
										  EntryDate = salesInvoiceHeader.EntryDate,
										  NetValue = salesInvoiceHeader.NetValue,
										  ClientId = salesInvoiceHeader.ClientId,
										  SellerId = salesInvoiceHeader.SellerId,
										  StoreId = salesInvoiceHeader.StoreId,

										  PaymentMethodId = paymentMethod.PaymentMethodId,
										  PaymentValue = salesInvoiceCollection.CollectedValue,
										  BankCommissionPaid = (decimal?)commission ?? 0,

										  CreatedAt = salesInvoiceHeader.CreatedAt,
										  UserNameCreated = salesInvoiceHeader.UserNameCreated,
										  ModifiedAt = salesInvoiceHeader.ModifiedAt,
										  UserNameModified = salesInvoiceHeader.UserNameModified,
									  };

			var salesInvoiceReturnHeaders = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(
												x => storeIds.Contains(x.StoreId) &&
												!x.CreditPayment &&
												(fromDate == null || x.DocumentDate >= fromDate) &&
												(toDate == null || x.DocumentDate <= toDate)
											)
											from salesInvoiceReturnPayment in _salesInvoiceReturnPaymentService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == salesInvoiceReturnPayment.PaymentMethodId)
											from commission in _journalDetailService.GetAll().Where(x =>
													x.JournalHeaderId == salesInvoiceReturnHeader.JournalHeaderId &&
													(x.AccountId == paymentMethod.CommissionAccountId || x.AccountId == paymentMethod.CommissionTaxAccountId) &&
													(paymentMethod.PaymentTypeId == PaymentTypeData.BankCard || paymentMethod.PaymentTypeId == PaymentTypeData.BankAccount)
												).GroupBy(x => x.JournalHeaderId)
												.Select(x => x.Sum(y => y.CreditValue)).DefaultIfEmpty()
											select new PaymentMethodsIncomeReportDataDto
											{
												HeaderId = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId,
												MenuCode = salesInvoiceReturnHeader.MenuCode ?? 0,
												DocumentFullCode = salesInvoiceReturnHeader.Prefix + salesInvoiceReturnHeader.DocumentCode + salesInvoiceReturnHeader.Suffix,
												DocumentDate = salesInvoiceReturnHeader.DocumentDate,
												EntryDate = salesInvoiceReturnHeader.EntryDate,
												NetValue = -salesInvoiceReturnHeader.NetValue,
												ClientId = salesInvoiceReturnHeader.ClientId,
												SellerId = salesInvoiceReturnHeader.SellerId,
												StoreId = salesInvoiceReturnHeader.StoreId,

												PaymentMethodId = paymentMethod.PaymentMethodId,
												PaymentValue = -salesInvoiceReturnPayment.PaidValue,
												BankCommissionPaid = (decimal?)commission ?? 0,

											    CreatedAt = salesInvoiceReturnHeader.CreatedAt,
											    UserNameCreated = salesInvoiceReturnHeader.UserNameCreated,
											    ModifiedAt = salesInvoiceReturnHeader.ModifiedAt,
											    UserNameModified = salesInvoiceReturnHeader.UserNameModified,
											};

			var receiptVoucherNetValues = from receiptVoucherDetail in _receiptVoucherDetailService.GetAll()
										  from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == receiptVoucherDetail.PaymentMethodId)
										  where paymentMethod.PaymentTypeId == PaymentTypeData.Cash
										  group receiptVoucherDetail by receiptVoucherDetail.ReceiptVoucherDetailId into g
										  select new
										  {
											  ReceiptVoucherDetailId = g.Key,
											  NetValue = g.Sum(x => x.DebitValue)
										  };

			var receiptVoucherHeaders = from receiptVoucherHeader in _receiptVoucherHeaderService.GetAll().Where(
											x => storeIds.Contains(x.StoreId) &&
											(fromDate == null || x.DocumentDate >= fromDate) &&
											(toDate == null || x.DocumentDate <= toDate)
										)
										from receiptVoucherDetail in _receiptVoucherDetailService.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeader.ReceiptVoucherHeaderId)
										from netValue in receiptVoucherNetValues.Where(x => x.ReceiptVoucherDetailId == receiptVoucherDetail.ReceiptVoucherDetailId)
										from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == receiptVoucherDetail.PaymentMethodId)
										from commission in _journalDetailService.GetAll().Where(x =>
										        x.JournalHeaderId == receiptVoucherHeader.JournalHeaderId &&
												(x.AccountId == paymentMethod.CommissionAccountId || x.AccountId == paymentMethod.CommissionTaxAccountId) &&
												(paymentMethod.PaymentTypeId == PaymentTypeData.BankCard || paymentMethod.PaymentTypeId == PaymentTypeData.BankAccount)
											).GroupBy(x => x.JournalHeaderId)
											.Select(x => x.Sum(y => y.DebitValue)).DefaultIfEmpty()
										select new PaymentMethodsIncomeReportDataDto
										{
											HeaderId = receiptVoucherHeader.ReceiptVoucherHeaderId,
											MenuCode = MenuCodeData.ReceiptVoucher,
											DocumentFullCode = receiptVoucherHeader.Prefix + receiptVoucherHeader.ReceiptVoucherCode + receiptVoucherHeader.Suffix,
											DocumentDate = receiptVoucherHeader.DocumentDate,
											EntryDate = receiptVoucherHeader.EntryDate,
											NetValue = netValue.NetValue,
											ClientId = receiptVoucherHeader.ClientId,
											SellerId = receiptVoucherHeader.SellerId,
											StoreId = receiptVoucherHeader.StoreId,

											PaymentMethodId = paymentMethod.PaymentMethodId,
											PaymentValue = receiptVoucherDetail.DebitValue,
											BankCommissionPaid = (decimal?)commission ?? 0,

											CreatedAt = receiptVoucherHeader.CreatedAt,
											UserNameCreated = receiptVoucherHeader.UserNameCreated,
											ModifiedAt = receiptVoucherHeader.ModifiedAt,
											UserNameModified = receiptVoucherHeader.UserNameModified,
										};

			var paymentVoucherNetValues = from paymentVoucherDetail in _paymentVoucherDetailService.GetAll()
										  from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == paymentVoucherDetail.PaymentMethodId)
										  where paymentMethod.PaymentTypeId == PaymentTypeData.Cash
										  group paymentVoucherDetail by paymentVoucherDetail.PaymentVoucherDetailId into g
										  select new
										  {
											  PaymentVoucherDetailId = g.Key,
											  NetValue = g.Sum(x => x.DebitValue)
										  };

			var paymentVoucherHeaders = from paymentVoucherHeader in _paymentVoucherHeaderService.GetAll().Where(
											x => storeIds.Contains(x.StoreId) &&
											(fromDate == null || x.DocumentDate >= fromDate) &&
											(toDate == null || x.DocumentDate <= toDate)
										)
										from paymentVoucherDetail in _paymentVoucherDetailService.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeader.PaymentVoucherHeaderId)
										from netValue in paymentVoucherNetValues.Where(x => x.PaymentVoucherDetailId == paymentVoucherDetail.PaymentVoucherDetailId)
										from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == paymentVoucherDetail.PaymentMethodId)
										from commission in _journalDetailService.GetAll().Where(x =>
										        x.JournalHeaderId == paymentVoucherHeader.JournalHeaderId &&
												(x.AccountId == paymentMethod.CommissionAccountId || x.AccountId == paymentMethod.CommissionTaxAccountId) &&
												(paymentMethod.PaymentTypeId == PaymentTypeData.BankCard || paymentMethod.PaymentTypeId == PaymentTypeData.BankAccount)
											).GroupBy(x => x.JournalHeaderId)
											.Select(x => x.Sum(y => y.CreditValue)).DefaultIfEmpty()
										select new PaymentMethodsIncomeReportDataDto
										{
											HeaderId = paymentVoucherHeader.PaymentVoucherHeaderId,
											MenuCode = MenuCodeData.PaymentVoucher,
											DocumentFullCode = paymentVoucherHeader.Prefix + paymentVoucherHeader.PaymentVoucherCode + paymentVoucherHeader.Suffix,
											DocumentDate = paymentVoucherHeader.DocumentDate,
											EntryDate = paymentVoucherHeader.EntryDate,
											NetValue = netValue.NetValue,
											ClientId = null,
											SellerId = paymentVoucherHeader.SellerId,
											StoreId = paymentVoucherHeader.StoreId,

											PaymentMethodId = paymentMethod.PaymentMethodId,
											PaymentValue = -paymentVoucherDetail.CreditValue,
											BankCommissionPaid = (decimal?)commission ?? 0,

											CreatedAt = paymentVoucherHeader.CreatedAt,
											UserNameCreated = paymentVoucherHeader.UserNameCreated,
											ModifiedAt = paymentVoucherHeader.ModifiedAt,
											UserNameModified = paymentVoucherHeader.UserNameModified,
										};


			return salesInvoiceHeaders
				.Concat(salesInvoiceReturnHeaders)
				.Concat(receiptVoucherHeaders)
				.Concat(paymentVoucherHeaders);
		}
	}
}
