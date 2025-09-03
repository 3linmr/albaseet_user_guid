using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Contracts;
using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Models.Dtos.ViewModels;

namespace Purchases.Service.Services
{
	public class PurchaseValueService: IPurchaseValueService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;

		public PurchaseValueService(IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService)
		{
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_supplierCreditMemoService = supplierCreditMemoService;
			_supplierDebitMemoService = supplierDebitMemoService;
		}

		public async Task<decimal> GetOverallValueOfPurchaseInvoice(int purchaseInvoiceHeaderId)
		{
			return await GetOverallValueOfPurchaseInvoiceInternal(purchaseInvoiceHeaderId, null, null);
		}

		public async Task<decimal> GetOverallValueOfPurchaseInvoiceExceptSupplierCredit(int purchaseInvoiceHeaderId, int exceptSupplierCredit)
		{
			return await GetOverallValueOfPurchaseInvoiceInternal(purchaseInvoiceHeaderId, exceptSupplierCredit, null);
		}

		public async Task<decimal> GetOverallValueOfPurchaseInvoiceExceptSupplierDebit(int purchaseInvoiceHeaderId, int exceptSupplierDebit)
		{
			return await GetOverallValueOfPurchaseInvoiceInternal(purchaseInvoiceHeaderId, null, exceptSupplierDebit);
		}

		private async Task<decimal> GetOverallValueOfPurchaseInvoiceInternal(int purchaseInvoiceHeaderId, int? exceptSupplierCreditMemo, int? exceptSupplierDebitMemo)
		{
			var purchaseInvoiceHeaderValue = (await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId))?.NetValue ?? 0;

			var purchaseInvoiceReturnsValueTotal = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).SumAsync(x => x.NetValue);

			var supplierCreditMemosValueTotal = await _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && x.SupplierCreditMemoId != exceptSupplierCreditMemo).SumAsync(x => x.MemoValue);

			var supplierDebitMemosValuesTotal = await _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && x.SupplierDebitMemoId != exceptSupplierDebitMemo).SumAsync(x => x.MemoValue);

			return (purchaseInvoiceHeaderValue + supplierCreditMemosValueTotal) - (purchaseInvoiceReturnsValueTotal + supplierDebitMemosValuesTotal);
		}

		public IQueryable<PurchaseInvoiceOverallValueDto> GetOverallValueOfPurchaseInvoices()
		{
			//equation: purchaseInvoiceValue - purchaseInvoiceReturns + supplierCredits - supplierDebits
			var purchaseInvoices = _purchaseInvoiceHeaderService.GetAll().Select( 
				x => new PurchaseInvoiceOverallValueDto { PurchaseInvoiceHeaderId = x.PurchaseInvoiceHeaderId, OverallValue = x.NetValue});

			var purchaseInvoiceReturns = _purchaseInvoiceReturnHeaderService.GetAll().Select(
				x => new PurchaseInvoiceOverallValueDto { PurchaseInvoiceHeaderId = x.PurchaseInvoiceHeaderId, OverallValue = - x.NetValue });

			var supplierCreditMemos = _supplierCreditMemoService.GetAll().Select(
				x => new PurchaseInvoiceOverallValueDto { PurchaseInvoiceHeaderId = x.PurchaseInvoiceHeaderId, OverallValue = x.MemoValue });

			var supplierDebitMemos = _supplierDebitMemoService.GetAll().Select(
				x => new PurchaseInvoiceOverallValueDto { PurchaseInvoiceHeaderId = x.PurchaseInvoiceHeaderId, OverallValue = - x.MemoValue });

			return purchaseInvoices.Concat(purchaseInvoiceReturns).Concat(supplierCreditMemos).Concat(supplierDebitMemos).GroupBy(x => x.PurchaseInvoiceHeaderId).Select(x => new PurchaseInvoiceOverallValueDto
			{
				PurchaseInvoiceHeaderId = x.Key,
				OverallValue = x.Sum(y => y.OverallValue)
			});
		}
	}
}
