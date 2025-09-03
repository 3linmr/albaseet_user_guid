using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Contracts;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Journal;

namespace Sales.Service.Services
{
	public class SalesValueService: ISalesValueService
	{
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IJournalDetailService _journalDetailService;

		public SalesValueService(ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, IJournalDetailService journalDetailService)
		{
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_clientCreditMemoService = clientCreditMemoService;
			_clientDebitMemoService = clientDebitMemoService;
			_journalDetailService = journalDetailService;
		}

		public async Task<decimal> GetOverallValueOfSalesInvoice(int salesInvoiceHeaderId)
		{
			return await GetOverallValueOfSalesInvoiceInternal(salesInvoiceHeaderId, null, null);
		}

		public async Task<decimal> GetOverallValueOfSalesInvoiceExceptClientCredit(int salesInvoiceHeaderId, int exceptClientCredit)
		{
			return await GetOverallValueOfSalesInvoiceInternal(salesInvoiceHeaderId, exceptClientCredit, null);
		}

		public async Task<decimal> GetOverallValueOfSalesInvoiceExceptClientDebit(int salesInvoiceHeaderId, int exceptClientDebit)
		{
			return await GetOverallValueOfSalesInvoiceInternal(salesInvoiceHeaderId, null, exceptClientDebit);
		}

		private async Task<decimal> GetOverallValueOfSalesInvoiceInternal(int salesInvoiceHeaderId, int? exceptClientCreditMemo, int? exceptClientDebitMemo)
		{
			var salesInvoiceHeaderValue = (await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId))?.NetValue ?? 0;

			var salesInvoiceReturnsValueTotal = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).SumAsync(x => x.NetValue);

			var clientCreditMemosValuesTotal = await _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.ClientCreditMemoId != exceptClientCreditMemo).SumAsync(x => x.MemoValue);

			var clientDebitMemosValuesTotal = await _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.ClientDebitMemoId != exceptClientDebitMemo).SumAsync(x => x.MemoValue);

			return (salesInvoiceHeaderValue + clientDebitMemosValuesTotal) - (salesInvoiceReturnsValueTotal + clientCreditMemosValuesTotal);
		}

		public IQueryable<SalesInvoiceOverallValueDto> GetOverallValueOfSalesInvoices()
		{
			return GetOverallValueOfSalesInvoicesWithDateRange(null, null);
		}

		public IQueryable<SalesInvoiceOverallValueDto> GetOverallValueOfSalesInvoicesWithDateRange(DateTime? fromDate = null, DateTime? toDate = null)
		{
			//equation:
			// overall net value = salesInvoiceNetValue - salesInvoiceReturnsNetValues + clientDebitValues - clientCreditValues
			// overall gross value = salesInvoiceGrossValue - salesInvoiceReturnsGrossValues + clientDebitValues - clientCreditValues
			// overall total value = salesInvoiceTotalValue - salesInvoiceReturnsTotalValues
			// overall cost value = salesInvoiceCostValue - salesInvoiceReturnsCostValues
			// overall discount value = salesInvoiceDiscountValue - salesInvoiceReturnsDiscountValues - clientDebitValues + clientCreditValues

			var salesInvoices = _salesInvoiceHeaderService.GetAll().Where(x =>
					(fromDate == null || x.DocumentDate >= fromDate) &&
					(toDate == null || x.DocumentDate <= toDate)
				).Select(
				x => new SalesInvoiceOverallValueDto
				{
					SalesInvoiceHeaderId = x.SalesInvoiceHeaderId,
					OverallNetValue = x.NetValue,
					OverallTotalValue = x.TotalValue,
					OverallGrossValue = x.GrossValue,
					OverallCostValue = x.TotalCostValue,
					OverallDiscount = x.TotalItemDiscount + x.DiscountValue,
					OverallTaxValue = x.VatValue + x.OtherTaxValue,
				});

			var salesInvoiceReturns = _salesInvoiceReturnHeaderService.GetAll().Where(x =>
					(fromDate == null || x.DocumentDate >= fromDate) &&
					(toDate == null || x.DocumentDate <= toDate)
				).Select(
				x => new SalesInvoiceOverallValueDto
				{
					SalesInvoiceHeaderId = x.SalesInvoiceHeaderId,
					OverallNetValue = -x.NetValue,
					OverallTotalValue = -x.TotalValue,
					OverallGrossValue = -x.GrossValue,
					OverallCostValue = -x.TotalCostValue,
					OverallDiscount = -(x.TotalItemDiscount + x.DiscountValue),
                    OverallTaxValue = -(x.VatValue + x.OtherTaxValue),
                });

			var clientCreditMemos = from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
								    from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientCreditMemo.JournalHeaderId && x.IsTax)
								    from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									select new SalesInvoiceOverallValueDto
									{
										SalesInvoiceHeaderId = clientCreditMemo.SalesInvoiceHeaderId,
										OverallTotalValue = 0,
										OverallNetValue = -clientCreditMemo.MemoValue,
										OverallGrossValue = -memoValueBeforeTaxJournal.DebitValue,
										OverallCostValue = 0,
										OverallDiscount = memoValueBeforeTaxJournal.DebitValue,
										OverallTaxValue = -taxJournal.DebitValue,
									};

			var clientDebitMemos = from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
								    from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientDebitMemo.JournalHeaderId && x.IsTax)
								    from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									select new SalesInvoiceOverallValueDto
									{
										SalesInvoiceHeaderId = clientDebitMemo.SalesInvoiceHeaderId,
										OverallTotalValue = 0,
										OverallNetValue = clientDebitMemo.MemoValue,
										OverallGrossValue = memoValueBeforeTaxJournal.CreditValue,
										OverallCostValue = 0,
										OverallDiscount = -memoValueBeforeTaxJournal.CreditValue,
										OverallTaxValue = taxJournal.CreditValue,
									};

			return salesInvoices.Concat(salesInvoiceReturns).Concat(clientCreditMemos).Concat(clientDebitMemos).GroupBy(x => x.SalesInvoiceHeaderId).Select(x => new SalesInvoiceOverallValueDto
			{
				SalesInvoiceHeaderId = x.Key,
				OverallNetValue = x.Sum(y => y.OverallNetValue),
				OverallTotalValue = x.Sum(y => y.OverallTotalValue),
				OverallGrossValue = x.Sum(y => y.OverallGrossValue),
				OverallCostValue = x.Sum(y => y.OverallCostValue),
				OverallProfit = x.Sum(y => y.OverallGrossValue) - x.Sum(y => y.OverallCostValue),
				OverallProfitPercent = x.Sum(y => y.OverallCostValue) != 0 ? ( x.Sum(y => y.OverallGrossValue) - x.Sum(y => y.OverallCostValue) ) / x.Sum(y => y.OverallCostValue) * 100 : 0,
				OverallDiscount = x.Sum(y => y.OverallDiscount),
				OverallDiscountPercent = x.Sum(y => y.OverallTotalValue) != 0 ? x.Sum(y => y.OverallDiscount) / x.Sum(y => y.OverallTotalValue) * 100 : 0,
				OverallTaxValue = x.Sum(y => y.OverallTaxValue),
			});
		}
	}
}
