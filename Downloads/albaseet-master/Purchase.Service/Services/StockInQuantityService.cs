using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.Service.Services
{
    public class StockInQuantityService: IStockInQuantityService
    {
        private readonly IStockInDetailService _stockInDetailService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInReturnDetailService _stockInReturnDetailService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;

        public StockInQuantityService(IStockInDetailService stockInDetailService, IStockInHeaderService stockInHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService)
        {
            _purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _stockInDetailService = stockInDetailService;
            _stockInHeaderService = stockInHeaderService;
            _stockInReturnDetailService = stockInReturnDetailService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
        }

        public IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseOrderInternal(purchaseOrderHeaderId, null, null, false);
        }
        public IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderWithAllKeys(int purchaseOrderHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseOrderInternal(purchaseOrderHeaderId, null, null, true);
        }

        public IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderExceptStockInHeaderId(int purchaseOrderHeaderId, int exceptStockInHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseOrderInternal(purchaseOrderHeaderId, exceptStockInHeaderId, null, false);
        }

        public IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderExceptStockInReturnHeaderId(int purchaseOrderHeaderId, int exceptStockInReturnHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseOrderInternal(purchaseOrderHeaderId, null, exceptStockInReturnHeaderId, false);
        }

        private IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderInternal(int purchaseOrderHeaderId, int? exceptStockInHeaderId = null, int? exceptStockInReturnHeaderId = null, bool useAllKeys = false)
        {
            var stockInReceived = (
                   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId != exceptStockInHeaderId)
                   from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInDetail.StockInHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId).Select(x => x.PurchaseOrderHeaderId)
                   select new StockReceivedFromPurchaseOrderDto
                   {
                       ItemId = stockInDetail.ItemId,
                       ItemPackageId = stockInDetail.ItemPackageId,
                       CostCenterId = useAllKeys ? stockInDetail.CostCenterId : null,
                       BatchNumber = useAllKeys ? stockInDetail.BatchNumber : null,
                       ExpireDate = useAllKeys ? stockInDetail.ExpireDate : null,
                       BarCode = stockInDetail.BarCode,
                       PurchasePrice = stockInDetail.PurchasePrice,
                       ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                       QuantityReceived = stockInDetail.Quantity,
                       BonusQuantityReceived = stockInDetail.BonusQuantity
                   }
            ).Concat(
                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId != exceptStockInReturnHeaderId)
                   from stockInHeaderId in _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnDetail.StockInReturnHeaderId && x.StockInHeaderId != exceptStockInHeaderId).Select(x => x.StockInHeaderId)
                   from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId && x.PurchaseOrderHeaderId == purchaseOrderHeaderId).Select(x => x.PurchaseOrderHeaderId)
                   select new StockReceivedFromPurchaseOrderDto
                   {
                       ItemId = stockInReturnDetail.ItemId,
                       ItemPackageId = stockInReturnDetail.ItemPackageId,
                       BatchNumber = useAllKeys ? stockInReturnDetail.BatchNumber : null,
                       ExpireDate = useAllKeys ? stockInReturnDetail.ExpireDate : null,
                       CostCenterId = useAllKeys ? stockInReturnDetail.CostCenterId : null,
                       BarCode = stockInReturnDetail.BarCode,
                       PurchasePrice = stockInReturnDetail.PurchasePrice,
                       ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                       QuantityReceived = -stockInReturnDetail.Quantity,
                       BonusQuantityReceived = -stockInReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
                    x => new StockReceivedFromPurchaseOrderDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        PurchasePrice = x.Key.PurchasePrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityReceived = x.Sum(y => y.QuantityReceived),
                        BonusQuantityReceived = x.Sum(y => y.BonusQuantityReceived)
                    }
                );

            return stockInReceived;
        }

        public IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseInvoiceInternal(purchaseInvoiceHeaderId);
        }

        public IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInHeaderId(int purchaseInvoiceHeaderId, int exceptStockInHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseInvoiceInternal(purchaseInvoiceHeaderId, exceptStockInHeaderId, null);
        }

        public IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInReturnHeaderId(int purchaseInvoiceHeaderId, int exceptStockInReturnHeaderId)
        {
            return GetFinalStocksReceivedFromPurchaseInvoiceInternal(purchaseInvoiceHeaderId, null, exceptStockInReturnHeaderId);
        }

        private IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoiceInternal(int purchaseInvoiceHeaderId, int? exceptStockInHeaderId = null, int? exceptStockInReturnHeaderId = null)
        {
            var stockInReceived = (
                   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId != exceptStockInHeaderId)
                   from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInDetail.StockInHeaderId && x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.PurchaseInvoiceHeaderId)
                   select new StockReceivedFromPurchaseInvoiceDto
                   {
                       ItemId = stockInDetail.ItemId,
                       ItemPackageId = stockInDetail.ItemPackageId,
                       BatchNumber = stockInDetail.BatchNumber,
                       ExpireDate = stockInDetail.ExpireDate,
                       CostCenterId = stockInDetail.CostCenterId,
                       BarCode = stockInDetail.BarCode,
                       PurchasePrice = stockInDetail.PurchasePrice,
                       ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                       QuantityReceived = stockInDetail.Quantity,
                       BonusQuantityReceived = stockInDetail.BonusQuantity
                   }
            ).Concat(
                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId != exceptStockInReturnHeaderId)
                   from stockInHeaderId in _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnDetail.StockInReturnHeaderId && x.StockInHeaderId != exceptStockInHeaderId).Select(x => x.StockInHeaderId)
                   from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId && x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.PurchaseInvoiceHeaderId)
                   select new StockReceivedFromPurchaseInvoiceDto
                   {
                       ItemId = stockInReturnDetail.ItemId,
                       ItemPackageId = stockInReturnDetail.ItemPackageId,
                       BatchNumber = stockInReturnDetail.BatchNumber,
                       ExpireDate = stockInReturnDetail.ExpireDate,
                       CostCenterId = stockInReturnDetail.CostCenterId,
                       BarCode = stockInReturnDetail.BarCode,
                       PurchasePrice = stockInReturnDetail.PurchasePrice,
                       ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                       QuantityReceived = -stockInReturnDetail.Quantity,
                       BonusQuantityReceived = -stockInReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
                    x => new StockReceivedFromPurchaseInvoiceDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        PurchasePrice = x.Key.PurchasePrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityReceived = x.Sum(y => y.QuantityReceived),
                        BonusQuantityReceived = x.Sum(y => y.BonusQuantityReceived)
                    }
                );

            return stockInReceived;
        }

        public IQueryable<StockReturnedDto> GetStocksReturnedFromStockIn(int stockInHeaderId)
        {
            return GetStocksReturnedFromStockInInternal(stockInHeaderId);
        }

        public IQueryable<StockReturnedDto> GetStocksReturnedFromStockInExceptStockInReturnHeaderId(int stockInHeaderId, int exceptStockInReturnHeaderId)
        {
            return GetStocksReturnedFromStockInInternal(stockInHeaderId, exceptStockInReturnHeaderId);
        }

        private IQueryable<StockReturnedDto> GetStocksReturnedFromStockInInternal(int stockInHeaderId, int? exceptStockInReturnHeaderId = null)
        {
            var stockInReturned = (
                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId != exceptStockInReturnHeaderId)
                   from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnDetail.StockInReturnHeaderId && x.StockInHeaderId == stockInHeaderId).Select(x => x.StockInHeaderId)
                   select new StockReturnedDto
                   {
                       ItemId = stockInReturnDetail.ItemId,
                       ItemPackageId = stockInReturnDetail.ItemPackageId,
                       BatchNumber = stockInReturnDetail.BatchNumber,
                       ExpireDate = stockInReturnDetail.ExpireDate,
                       CostCenterId = stockInReturnDetail.CostCenterId,
                       BarCode = stockInReturnDetail.BarCode,
                       PurchasePrice = stockInReturnDetail.PurchasePrice,
                       ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                       QuantityReturned = stockInReturnDetail.Quantity,
                       BonusQuantityReturned = stockInReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
                    x => new StockReturnedDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        PurchasePrice = x.Key.PurchasePrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityReturned = x.Sum(y => y.QuantityReturned),
                        BonusQuantityReturned = x.Sum(y => y.BonusQuantityReturned)
                    }
                );

            return stockInReturned;
        }

        public IQueryable<StockReturnedDto> GetStocksReturnedFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            return GetStocksReturnedFromPurchaseInvoiceInternal(purchaseInvoiceHeaderId);
        }

        public IQueryable<StockReturnedDto> GetStocksReturnedFromPurchaseInvoiceExceptStockInReturnHeaderId(int purchaseInvoiceHeaderId, int? exceptStockInReturnHeaderId)
        {
            return GetStocksReturnedFromPurchaseInvoiceInternal(purchaseInvoiceHeaderId, exceptStockInReturnHeaderId);
        }

        private IQueryable<StockReturnedDto> GetStocksReturnedFromPurchaseInvoiceInternal(int purchaseInvoiceHeaderId, int? exceptStockInReturnHeaderId = null)
        {
            var stockInReturned = (
                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId != exceptStockInReturnHeaderId)
                   from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnDetail.StockInReturnHeaderId && x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.StockInHeaderId)
                   select new StockReturnedDto
                   {
                       ItemId = stockInReturnDetail.ItemId,
                       ItemPackageId = stockInReturnDetail.ItemPackageId,
                       BatchNumber = stockInReturnDetail.BatchNumber,
                       ExpireDate = stockInReturnDetail.ExpireDate,
                       CostCenterId = stockInReturnDetail.CostCenterId,
                       BarCode = stockInReturnDetail.BarCode,
                       PurchasePrice = stockInReturnDetail.PurchasePrice,
                       ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                       QuantityReturned = stockInReturnDetail.Quantity,
                       BonusQuantityReturned = stockInReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
                    x => new StockReturnedDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        PurchasePrice = x.Key.PurchasePrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityReturned = x.Sum(y => y.QuantityReturned),
                        BonusQuantityReturned = x.Sum(y => y.BonusQuantityReturned)
                    }
                );

            return stockInReturned;
        }

        //This function is for purchase invoices that are on the way only
        public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableFromPurchaseInvoices()
        {
            //formula: PurchaseInvoice quantities - StockIn quantities  + StockInReturn quantities
            var quantityReceived = from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId != null)
                                   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
                                   select new ParentQuantityDto { ParentId = (int)stockInHeader.PurchaseInvoiceHeaderId!, Quantity = -(stockInDetail.Quantity + stockInDetail.BonusQuantity) };

            var quantityReturned = from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId != null)
                                   from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)
                                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                   select new ParentQuantityDto { ParentId = (int)stockInHeader.PurchaseInvoiceHeaderId!, Quantity = (stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity) };


            var purchaseInvoiceQuantity = from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll()
                                          select new ParentQuantityDto { ParentId = purchaseInvoiceDetail.PurchaseInvoiceHeaderId, Quantity = purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity };

            var overallAvailableQuantity = purchaseInvoiceQuantity.Concat(quantityReceived).Concat(quantityReturned).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

            return overallAvailableQuantity;
        }

        public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromStockIns()
        {
            //formula: stockIn quantities - stockInReturn quantities 
            var quantityReturned = from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId != null)
                                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                   select new ParentQuantityDto { ParentId = (int)stockInReturnHeader.StockInHeaderId!, Quantity = -(stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity) };


            var stockInQuantity = from stockInDetail in _stockInDetailService.GetAll()
                                  select new ParentQuantityDto { ParentId = stockInDetail.StockInHeaderId, Quantity = stockInDetail.Quantity + stockInDetail.BonusQuantity };

            var overallAvailableQuantityReturn = stockInQuantity.Concat(quantityReturned).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

            return overallAvailableQuantityReturn;
        }

        public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromPurchaseInvoices()
        {
            //formula: PurchaseInvoice quantities - purchaseInvoiceReturnOnTheWay - StockInReturn quantities 
            var quantityReturned = from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId != null)
                                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                   select new ParentQuantityDto { ParentId = (int)stockInReturnHeader.PurchaseInvoiceHeaderId!, Quantity = -(stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity) };


            var purchaseInvoiceQuantity = from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll()
                                  select new ParentQuantityDto { ParentId = purchaseInvoiceDetail.PurchaseInvoiceHeaderId, Quantity = purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity };

            var purchaseInvoiceReturnsQuantity = from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll()
                                                 from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnDetail.PurchaseInvoiceReturnHeaderId && x.IsOnTheWay)
                                                 select new ParentQuantityDto
                                                 {
                                                     ParentId = purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId,
                                                     Quantity = - (purchaseInvoiceReturnDetail.Quantity + purchaseInvoiceReturnDetail.BonusQuantity)
                                                 };

            var overallAvailableQuantityReturn = purchaseInvoiceQuantity.Concat(quantityReturned).Concat(purchaseInvoiceReturnsQuantity).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

            return overallAvailableQuantityReturn;
        }

        public IQueryable<ParentQuantityDto> GetOverallQuantityReturnedFromPurchaseInvoices()
        {
            //formula: StockInReturn quantities 
            var quantityReturned = from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId != null)
                                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)
                                   select new ParentQuantityDto { ParentId = (int)stockInReturnHeader.PurchaseInvoiceHeaderId!, Quantity = (stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity) };

            var overallQuantityReturned = quantityReturned.GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

            return overallQuantityReturned;
        }
    }
}
