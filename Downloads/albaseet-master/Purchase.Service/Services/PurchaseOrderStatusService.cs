using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.StaticData;
using MoreLinq;

namespace Purchases.Service.Services
{
    public class PurchaseOrderStatusService : IPurchaseOrderStatusService
    {
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInDetailService _stockInDetailService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IStockInReturnDetailService _stockInReturnDetailService;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;

        public PurchaseOrderStatusService(IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IPurchaseOrderDetailService purchaseOrderDetailService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService)
        {
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _stockInHeaderService = stockInHeaderService;
            _stockInDetailService = stockInDetailService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _stockInReturnDetailService = stockInReturnDetailService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
        }

		//menuCode for any PurchaseInvoice should be "PurchaseInvoiceInterim" even if it is another type of invoice
		//menuCode for any PurchaseInvoiceReturn should be "PurchaseInvoiceReturn" even if it is another type of invoice returns
		public async Task UpdatePurchaseOrderStatus(int purchaseOrderHeaderId, int documentStatusId, int menuCode)
        {
            int finalDocumentStatusId;

            if(documentStatusId == -1)
            {
                finalDocumentStatusId = await ComputePurchaseOrderDocumentStatusId(purchaseOrderHeaderId, menuCode);
            }
            else
            {
                finalDocumentStatusId = documentStatusId;
            }

            await _purchaseOrderHeaderService.UpdateDocumentStatus(purchaseOrderHeaderId, finalDocumentStatusId);
        }

        private async Task<int> ComputePurchaseOrderDocumentStatusId(int purchaseOrderHeaderId, int menuCode)
        {
            //menu Code checks are only used to skip unneeded queries, if the checks are removed the resulting status should still be the same
            if (menuCode == MenuCodeData.ReceiptStatement || menuCode == MenuCodeData.ReceiptStatementReturn || ((await IsPurchaseInvoiceExists(purchaseOrderHeaderId)) == false))
            {
                return await GetStatusBasedOnFinalQuantityReceivedFromPurchaseOrder(purchaseOrderHeaderId);
            }

            //MenuCodeData.PurchaseInvoice and MenuCodeData.PurchaseInvoiceReturn here are used to refer to both the direct and indirect versions
            if (menuCode != MenuCodeData.PurchaseInvoiceInterim && menuCode != MenuCodeData.ReturnFromPurchaseInvoice && menuCode != MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWayReturn && menuCode != MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay)
            {
                if (menuCode != MenuCodeData.PurchaseInvoiceReturn && await IsSupplierCreditMemoExists(purchaseOrderHeaderId))
                {
                    return DocumentStatusData.SupplierCreditMemoCreated;
                }

                if (menuCode != MenuCodeData.PurchaseInvoiceReturn && await IsSupplierDebitMemoExists(purchaseOrderHeaderId))
                {
                    return DocumentStatusData.SupplierDebitMemoCreated;
                }

                if (await IsPurchaseInvoiceReturnExists(purchaseOrderHeaderId))
                {
                    return DocumentStatusData.PurchaseInvoiceReturnCreated;
                }
            }

            if (await IsStockInBeforeInvoiceExists(purchaseOrderHeaderId))
            {
                return await GetStatusBasedOnQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrder(purchaseOrderHeaderId);
            }

            if (await IsPurchaseInvoiceReturnOnTheWayExists(purchaseOrderHeaderId))
            {
                return await GetStatusBasedOnQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrderAfterPurchaseInvoiceReturnOnTheWayHasBeenCreated(purchaseOrderHeaderId);
            }
            else
            {
                return await GetStatusBasedOnFinalQuantityReceivedFromPurchaseInvoiceLinkedToPurchaseOrder(purchaseOrderHeaderId);
            }
        }

        private async Task<int> GetStatusBasedOnFinalQuantityReceivedFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            var finalQuantityReceivedFromPurchaseOrder = await GetFinalQuantityReceivedFromPurchaseOrder(purchaseOrderHeaderId);
            var purchaseOrderQuantity = await GetPurchaseOrderQuantity(purchaseOrderHeaderId);
            if (finalQuantityReceivedFromPurchaseOrder == 0)
            {
                return DocumentStatusData.PurchaseOrderCreated;
            }
            else if (finalQuantityReceivedFromPurchaseOrder < purchaseOrderQuantity)
            {
                return DocumentStatusData.QuantityPartiallyReceived;
            }
            else
            {
                return DocumentStatusData.QuantityFullyReceived;
            }
        }

        //Used for purchase invoices on the way
        private async Task<int> GetStatusBasedOnFinalQuantityReceivedFromPurchaseInvoiceLinkedToPurchaseOrder(int purchaseOrderHeaderId)
        {
            var finalQuantityReceivedFromPurchaseInvoice = await GetFinalQuantityReceivedFromPurchaseInvoiceLinkedToPurchaseOrder(purchaseOrderHeaderId);
            var purchaseInvoiceQuantity = await GetPurchaseInvoiceQuantityLinkedToPurchaseOrder(purchaseOrderHeaderId);
            if (finalQuantityReceivedFromPurchaseInvoice == 0)
            {
                return DocumentStatusData.PurchaseInvoiceCreatedWaitingReceive;
            }
            else if (finalQuantityReceivedFromPurchaseInvoice < purchaseInvoiceQuantity)
            {
                return DocumentStatusData.QuantityPartiallyReceived;
            }
            else
            {
                return DocumentStatusData.QuantityFullyReceived;
            }
        }

        //Used for return from purchase invoices created after stock
        private async Task<int> GetStatusBasedOnQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrder(int purchaseOrderHeaderId)
        {
            var quantityReturnedFromPurchaseInvoice = await GetQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrder(purchaseOrderHeaderId);
            var purchaseInvoiceQuantity = await GetPurchaseInvoiceQuantityLinkedToPurchaseOrder(purchaseOrderHeaderId);
            if (quantityReturnedFromPurchaseInvoice == 0)
            {
                return DocumentStatusData.PurchaseInvoiceCreated;
            }
            else if (quantityReturnedFromPurchaseInvoice < purchaseInvoiceQuantity)
            {
                return DocumentStatusData.PartialQuantityReturnedWaitingPurchaseInvoiceReturn;
            }
            else
            {
                return DocumentStatusData.FullQuantityReturnedWaitingPurchaseInvoiceReturn;
            }
        }

        //Used for return after creating purchase invoice return on the way
        private async Task<int> GetStatusBasedOnQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrderAfterPurchaseInvoiceReturnOnTheWayHasBeenCreated(int purchaseOrderHeaderId)
		{
			var quantityReturnedFromPurchaseInvoice = await GetQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrder(purchaseOrderHeaderId);
			var purchaseInvoiceQuantity = await GetPurchaseInvoiceQuantityLinkedToPurchaseOrder(purchaseOrderHeaderId);
            var purchaseInvoiceReturnOnTheWayQuantity = await GetPurchaseInvoiceReturnOnTheWayQuantityLinkedToPurchaseOrder(purchaseOrderHeaderId);

			if (quantityReturnedFromPurchaseInvoice == 0)
			{
				return DocumentStatusData.PurchaseInvoiceReturnCreated;
			}
			else if (quantityReturnedFromPurchaseInvoice < (purchaseInvoiceQuantity - purchaseInvoiceReturnOnTheWayQuantity))
			{
				return DocumentStatusData.PartialQuantityReturnedWaitingPurchaseInvoiceReturn;
			}
			else
			{
				return DocumentStatusData.FullQuantityReturnedWaitingPurchaseInvoiceReturn;
			}
		}

		private async Task<bool> IsPurchaseInvoiceExists(int purchaseOrderHeaderId)
        {
            return await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).AnyAsync();
        }

        private async Task<bool> IsSupplierCreditMemoExists(int purchaseOrderHeaderId)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                          from supplierCreditMemo in _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                          select supplierCreditMemo).AnyAsync();
        }

        private async Task<bool> IsSupplierDebitMemoExists(int purchaseOrderHeaderId)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                          from supplierDebitMemo in _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                          select supplierDebitMemo).AnyAsync();
        }

        private async Task<bool> IsPurchaseInvoiceReturnExists(int purchaseOrderHeaderId)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                          from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId && !x.IsOnTheWay)
                          select purchaseInvoiceReturnHeader).AnyAsync();
        }

		private async Task<bool> IsPurchaseInvoiceReturnOnTheWayExists(int purchaseOrderHeaderId)
		{
			return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
						  from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId && x.IsOnTheWay)
						  select purchaseInvoiceReturnHeader).AnyAsync();
		}

		private async Task<bool> IsStockInBeforeInvoiceExists(int purchaseOrderHeaderId)
        {
            return await _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).AnyAsync();
        }

        private async Task<decimal> GetFinalQuantityReceivedFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            decimal quantityReceived = await (from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                                              from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
                                              select stockInDetail.Quantity + stockInDetail.BonusQuantity).SumAsync();

            decimal quantityReturned = await (from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                                              from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
                                              from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                              select stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity).SumAsync();

            return quantityReceived - quantityReturned;
        }

        private async Task<decimal> GetFinalQuantityReceivedFromPurchaseInvoiceLinkedToPurchaseOrder(int purchaseOrderHeaderId)
        {
            decimal quantityReceived = await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                                              from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                                              from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
                                              select stockInDetail.Quantity + stockInDetail.BonusQuantity).SumAsync();

            decimal quantityReturned = await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                                              from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                                              from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
                                              from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                              select stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity).SumAsync();

            return quantityReceived - quantityReturned;
        }

        private async Task<decimal> GetQuantityReturnedFromPurchaseInvoiceLinkedToPurchaseOrder(int purchaseOrderHeaderId)
        {
            decimal quantityReturned = await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                                              from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                                              from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                              select stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity).SumAsync();

            return quantityReturned;
        }

        private async Task<decimal> GetPurchaseOrderQuantity(int purchaseOrderHeaderId) 
        { 
            return await _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).Select(x => x.Quantity + x.BonusQuantity).SumAsync();
        }

        private async Task<decimal> GetPurchaseInvoiceQuantityLinkedToPurchaseOrder(int purchaseOrderHeaderId)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                          from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
                          select purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity).SumAsync();
        }

        private async Task<decimal> GetPurchaseInvoiceReturnOnTheWayQuantityLinkedToPurchaseOrder(int purchaseOrderHeaderId)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                          from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId && x.IsOnTheWay)
                          from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
                          select purchaseInvoiceReturnDetail.Quantity + purchaseInvoiceReturnDetail.BonusQuantity).SumAsync();
        }
    }
}
