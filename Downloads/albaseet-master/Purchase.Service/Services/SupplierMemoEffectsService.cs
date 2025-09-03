using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.Service.Services
{
	public class SupplierMemoEffectsService: ISupplierMemoEffectsService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IStockInHeaderService _stockInHeaderService;
		private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;

		public SupplierMemoEffectsService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_stockInHeaderService = stockInHeaderService;
			_stockInReturnHeaderService = stockInReturnHeaderService;
			_supplierDebitMemoService = supplierDebitMemoService;
			_supplierCreditMemoService = supplierCreditMemoService;
		}

		public async Task MarkAllDocumentsLinkedToPurchaseInvoiceAsEnded(int purchaseInvoiceHeaderId)
		{
			await _purchaseInvoiceHeaderService.UpdateEndedAndClosed(purchaseInvoiceHeaderId, true, true);
			await _stockInHeaderService.UpdateAllStockInsEndedDirectlyFromPurchaseInvoice(purchaseInvoiceHeaderId, true);
			await _stockInReturnHeaderService.UpdateAllStockInReturnsOnTheWayEndedFromPurchaseInvoice(purchaseInvoiceHeaderId, true);
			await _stockInReturnHeaderService.UpdateAllStockInReturnsEndedDirectlyFromPurchaseInvoice(purchaseInvoiceHeaderId, true);
			await _purchaseInvoiceReturnHeaderService.UpdatePurchaseInvoiceReturnOnTheWayEndedLinkedToPurchaseInvoice(purchaseInvoiceHeaderId, true);
			await _purchaseInvoiceReturnHeaderService.UpdatePurchaseInvoiceReturnNotOnTheWayEndedLinkedToPurchaseInvoice(purchaseInvoiceHeaderId, true);
		}

		public async Task ReopenAllDocumentsRelatedToPurchaseInvoice(int purchaseInvoiceHeaderId)
		{
			//assume all documents are already ended when this function is called and we are opening them up, but only if
			//there is nothing other than the supplierDebit/Credit being deleted that is making them ended
			var invoiceHasSupplierCredit = await _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();
			var invoiceHasSupplierDebit = await _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();

			if (invoiceHasSupplierCredit || invoiceHasSupplierDebit) return;

			var invoiceHasPurchaseInvoiceReturnNotOnTheWay = await _purchaseInvoiceReturnHeaderService.UpdatePurchaseInvoiceReturnNotOnTheWayEndedLinkedToPurchaseInvoice(purchaseInvoiceHeaderId, false);
			if (invoiceHasPurchaseInvoiceReturnNotOnTheWay) return;

			var stockInReturnsNotOnTheWayLinkedToPurchaseInvoice = await _stockInReturnHeaderService.UpdateAllStockInReturnsEndedDirectlyFromPurchaseInvoice(purchaseInvoiceHeaderId, false);

			var invoiceHasPurchaseInvoiceReturnOnTheWay = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && x.IsOnTheWay).AnyAsync();
			if (invoiceHasPurchaseInvoiceReturnOnTheWay)
			{
				if (!(stockInReturnsNotOnTheWayLinkedToPurchaseInvoice > 0))
				{
					await _purchaseInvoiceReturnHeaderService.UpdatePurchaseInvoiceReturnOnTheWayEndedLinkedToPurchaseInvoice(purchaseInvoiceHeaderId, false);
				}
				return;
			}

			await _stockInReturnHeaderService.UpdateAllStockInReturnsOnTheWayEndedFromPurchaseInvoice(purchaseInvoiceHeaderId, false);
			var stockInsLinkedToInvoice = await _stockInHeaderService.UpdateAllStockInsEndedDirectlyFromPurchaseInvoice(purchaseInvoiceHeaderId, false);
			await _purchaseInvoiceHeaderService.UpdateEndedAndClosed(purchaseInvoiceHeaderId, false, stockInsLinkedToInvoice > 0);
		}
	}
}
