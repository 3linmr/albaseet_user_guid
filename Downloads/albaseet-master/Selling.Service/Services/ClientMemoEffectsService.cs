using Sales.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sales.Service.Services
{
	public class ClientMemoEffectsService: IClientMemoEffectsService
	{
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IInvoiceStockOutReturnService _invoiceStockOutReturnService;

		public ClientMemoEffectsService(ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService, IInvoiceStockOutReturnService invoiceStockOutReturnService)
		{
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_clientDebitMemoService = clientDebitMemoService;
			_clientCreditMemoService = clientCreditMemoService;
			_invoiceStockOutReturnService = invoiceStockOutReturnService;
		}

		public async Task MarkAllDocumentsLinkedToSalesInvoiceAsEnded(int salesInvoiceHeaderId)
		{
			await _salesInvoiceHeaderService.UpdateEndedAndClosed(salesInvoiceHeaderId, true, true);
			await _stockOutHeaderService.UpdateAllStockOutsEndedDirectlyFromSalesInvoice(salesInvoiceHeaderId, true);
			await _stockOutReturnHeaderService.UpdateAllStockOutReturnReservationFromSalesInvoice(salesInvoiceHeaderId, true);
			await _stockOutReturnHeaderService.UpdateAllStockOutReturnsEndedDirectlyFromSalesInvoice(salesInvoiceHeaderId, true);
			await _salesInvoiceReturnHeaderService.UpdateReservationInvoiceCloseOutEndedLinkedToSalesInvoice(salesInvoiceHeaderId, true);
			await _salesInvoiceReturnHeaderService.UpdateAllSalesInvoiceReturnEndedLinkedToSalesInvoice(salesInvoiceHeaderId, true);
		}

		public async Task ReopenAllDocumentsRelatedToSalesInvoice(int salesInvoiceHeaderId)
		{
			//assume all documents are already ended when this function is called and we are opening them up, but only if
			//there is nothing other than the clientDebit/Credit being deleted that is making them ended
			var invoiceHasClientCredit = await _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).AnyAsync();
			var invoiceHasClientDebit = await _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).AnyAsync();

			if (invoiceHasClientCredit || invoiceHasClientDebit) return;

			var salesInvoiceReturnsNotReservation = await _salesInvoiceReturnHeaderService.UpdateAllSalesInvoiceReturnEndedLinkedToSalesInvoice(salesInvoiceHeaderId, false);
			var uninvoicedStockOutReturns = await MarkUnInvoicedStockOutReturnAsNotEnded(salesInvoiceHeaderId);

			var invoiceHasReservationInvoiceCloseOut = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.IsOnTheWay).AnyAsync();
			if (invoiceHasReservationInvoiceCloseOut)
			{
				if (!(salesInvoiceReturnsNotReservation > 0 || uninvoicedStockOutReturns > 0))
				{
					await _salesInvoiceReturnHeaderService.UpdateReservationInvoiceCloseOutEndedLinkedToSalesInvoice(salesInvoiceHeaderId, false);
				}
				return;
			}

			await _stockOutReturnHeaderService.UpdateAllStockOutReturnReservationFromSalesInvoice(salesInvoiceHeaderId, false);
			var stockOutsLinkedToInvoice = await _stockOutHeaderService.UpdateAllStockOutsEndedDirectlyFromSalesInvoice(salesInvoiceHeaderId, false);
			await _salesInvoiceHeaderService.UpdateEndedAndClosed(salesInvoiceHeaderId, false, stockOutsLinkedToInvoice > 0);
		}

		private async Task<int> MarkUnInvoicedStockOutReturnAsNotEnded(int salesInvoiceHeaderId)
		{
			var invoicedStockOutReturns = _invoiceStockOutReturnService.GetAll().Select(x => x.StockOutReturnHeaderId);
			var unInvoicedStockOutReturnsOnSalesInvoice = await _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && !invoicedStockOutReturns.Contains(x.StockOutReturnHeaderId)).Select(x => x.StockOutReturnHeaderId).ToListAsync();
			await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(unInvoicedStockOutReturnsOnSalesInvoice, false);

			return unInvoicedStockOutReturnsOnSalesInvoice.Count;
		}
	}
}
