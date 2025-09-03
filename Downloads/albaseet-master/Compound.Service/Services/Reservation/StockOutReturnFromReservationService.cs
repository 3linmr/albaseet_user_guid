using Compound.CoreOne.Contracts.Reservation;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reservation
{
	public class StockOutReturnFromReservationService: IStockOutReturnFromReservationService
	{
		private readonly IStockOutReturnHandlingService _stockOutReturnHandlingService;
		private readonly IDirectInternalTransferService _directInternalTransferService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IStoreService _storeService;
		private readonly IStockOutReturnService _stockOutReturnService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IGenericMessageService _genericMessageService;

		public StockOutReturnFromReservationService(IStockOutReturnHandlingService stockOutReturnHandlingService, IDirectInternalTransferService directInternalTransferService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStoreService storeService, IStockOutReturnService stockOutReturnService, IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService)
		{
			_stockOutReturnHandlingService = stockOutReturnHandlingService;
			_directInternalTransferService = directInternalTransferService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_storeService = storeService;
			_stockOutReturnService = stockOutReturnService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_genericMessageService = genericMessageService;
		}

		public async Task<ResponseDto> SaveStockOutReturnFromReservation(StockOutReturnDto stockOutReturn, bool hasApprove, bool isApproved, int? requestId)
		{

			List<StockOutReturnDetailDto>? oldStockOutReturnDetails = null;
			int? oldParentStoreId = null;
			int? oldReservationStoreId = null;
			if (stockOutReturn.StockOutReturnHeader!.StockOutReturnHeaderId != 0)
			{
				var oldStockOutReturn = await _stockOutReturnService.GetStockOutReturn(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId);
				oldStockOutReturnDetails = oldStockOutReturn.StockOutReturnDetails;
				oldParentStoreId = oldStockOutReturn.StockOutReturnHeader?.StoreId;
				oldReservationStoreId = await _storeService.GetReservedStoreByParentStoreId((int)oldParentStoreId!);
			}

			var newReservationStoreId = await _storeService.GetReservedStoreByParentStoreId(stockOutReturn.StockOutReturnHeader.StoreId);
			var documentReference = await GetStockOutReturnDocumentReference(stockOutReturn.StockOutReturnHeader!.StockOutReturnHeaderId, hasApprove, requestId);

			var stockOutReturnResult = await _stockOutReturnHandlingService.SaveStockOutReturn(stockOutReturn, hasApprove, isApproved, requestId, false, documentReference, false);
			if (stockOutReturnResult.Success == false) return stockOutReturnResult;

			var internalTransferResult = await SaveRelatedInternalTransferAndReceive(stockOutReturn, stockOutReturnResult.Id, newReservationStoreId, documentReference);
			if (internalTransferResult.Success == false) return internalTransferResult;


			await UpdateCurrentBalances(oldParentStoreId, oldReservationStoreId, stockOutReturn.StockOutReturnHeader.StoreId, newReservationStoreId, oldStockOutReturnDetails, stockOutReturn.StockOutReturnDetails);

			return stockOutReturnResult;
		}

		private async Task UpdateCurrentBalances(int? oldParentStoreId, int? oldReservationStoreId, int newParentStoreId, int newReservationStoreId, List<StockOutReturnDetailDto>? oldStockOutReturnDetails, List<StockOutReturnDetailDto> newStockOutReturnDetails)
		{
			var newItemBalances = GetItemCurrentBalancesFromStockOutReturn(newParentStoreId, newReservationStoreId, newStockOutReturnDetails);
			if (oldStockOutReturnDetails is null || oldParentStoreId is null)
			{
				await InventoryOut(newParentStoreId, newReservationStoreId, newItemBalances.ParentStoreCurrentBalances, newItemBalances.ReservationStoreCurrentBalances);
			}
			else
			{
				if (oldParentStoreId == newParentStoreId && oldReservationStoreId == newReservationStoreId)
				{
					var oldItemBalances = GetItemCurrentBalancesFromStockOutReturn((int)oldParentStoreId, (int)oldReservationStoreId, oldStockOutReturnDetails);
					await InventoryInOut((int)oldParentStoreId, (int)oldReservationStoreId!, oldItemBalances, newItemBalances);
				}
				else
				{
					await DeductCurrentBalances((int)oldParentStoreId, (int)oldReservationStoreId!, oldStockOutReturnDetails);
					await InventoryOut(newParentStoreId, newReservationStoreId, newItemBalances.ParentStoreCurrentBalances, newItemBalances.ReservationStoreCurrentBalances);
				}
			}
		}

		private async Task DeductCurrentBalances(int parentStoreId, int reservationStoreId, List<StockOutReturnDetailDto> stockOutReturnDetails)
		{
			var oldItemCurrentBalances = GetItemCurrentBalancesFromStockOutReturn(parentStoreId, reservationStoreId, stockOutReturnDetails);

			var newItemCurrentBalances = new ReservationStoreAndParentStoreCurrentBalancesDto
			{
				ParentStoreCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances.ParentStoreCurrentBalances
											  select new ItemCurrentBalanceDto()
											  {
												  ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
												  StoreId = itemCurrentPrices.StoreId,
												  ItemId = itemCurrentPrices.ItemId,
												  ItemPackageId = itemCurrentPrices.ItemPackageId,
												  ExpireDate = itemCurrentPrices.ExpireDate,
												  BatchNumber = itemCurrentPrices.BatchNumber,
												  //Zero
											  }).ToList(),

				ReservationStoreCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances.ReservationStoreCurrentBalances
												   select new ItemCurrentBalanceDto()
												   {
													   ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
													   StoreId = itemCurrentPrices.StoreId,
													   ItemId = itemCurrentPrices.ItemId,
													   ItemPackageId = itemCurrentPrices.ItemPackageId,
													   ExpireDate = itemCurrentPrices.ExpireDate,
													   BatchNumber = itemCurrentPrices.BatchNumber,
													   //Zero
												   }).ToList()
			};

			await InventoryInOut(parentStoreId, reservationStoreId, oldItemCurrentBalances, newItemCurrentBalances);
		}

		private ReservationStoreAndParentStoreCurrentBalancesDto GetItemCurrentBalancesFromStockOutReturn(int parentStoreId, int reservationStoreId, List<StockOutReturnDetailDto> stockOutReturnDetails)
		{
			var currentBalancesParentStoreId = (from stockOutReturnDetail in stockOutReturnDetails
												select new ItemCurrentBalanceDto()
												{
													StoreId = parentStoreId,
													ItemId = stockOutReturnDetail.ItemId,
													ItemPackageId = stockOutReturnDetail.ItemPackageId,
													ExpireDate = stockOutReturnDetail.ExpireDate,
													BatchNumber = stockOutReturnDetail.BatchNumber,
													OutQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity,
													InQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity
												}).ToList();

			var currentBalancesReservationStoreId = (from stockOutReturnDetail in stockOutReturnDetails
													 select new ItemCurrentBalanceDto()
													 {
														 StoreId = reservationStoreId,
														 ItemId = stockOutReturnDetail.ItemId,
														 ItemPackageId = stockOutReturnDetail.ItemPackageId,
														 ExpireDate = stockOutReturnDetail.ExpireDate,
														 BatchNumber = stockOutReturnDetail.BatchNumber,
														 InQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity
													 }).ToList();

			return new ReservationStoreAndParentStoreCurrentBalancesDto { ParentStoreCurrentBalances = currentBalancesParentStoreId, ReservationStoreCurrentBalances = currentBalancesReservationStoreId };
		}

		private async Task InventoryInOut(int parentStoreId, int reservationStoreId, ReservationStoreAndParentStoreCurrentBalancesDto oldItemCurrentBalances, ReservationStoreAndParentStoreCurrentBalancesDto newItemCurrentBalances)
		{
			await _itemCurrentBalanceService.InventoryInOut(parentStoreId, oldItemCurrentBalances.ParentStoreCurrentBalances, newItemCurrentBalances.ParentStoreCurrentBalances);
			await _itemCurrentBalanceService.InventoryInOut(reservationStoreId, oldItemCurrentBalances.ReservationStoreCurrentBalances, newItemCurrentBalances.ReservationStoreCurrentBalances);
		}

		private async Task InventoryOut(int parentStoreId, int reservationStoreId, List<ItemCurrentBalanceDto> parentStoreCurrentBalances, List<ItemCurrentBalanceDto> reservationStoreCurrentBalances)
		{
			var oldItemCurrentBalances = new ReservationStoreAndParentStoreCurrentBalancesDto
			{
				ParentStoreCurrentBalances = (from itemCurrentPrices in parentStoreCurrentBalances
											  select new ItemCurrentBalanceDto()
											  {
												  ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
												  StoreId = itemCurrentPrices.StoreId,
												  ItemId = itemCurrentPrices.ItemId,
												  ItemPackageId = itemCurrentPrices.ItemPackageId,
												  ExpireDate = itemCurrentPrices.ExpireDate,
												  BatchNumber = itemCurrentPrices.BatchNumber,
												  //Zero
											  }).ToList(),

				ReservationStoreCurrentBalances = (from itemCurrentPrices in reservationStoreCurrentBalances
												   select new ItemCurrentBalanceDto()
												   {
													   ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
													   StoreId = itemCurrentPrices.StoreId,
													   ItemId = itemCurrentPrices.ItemId,
													   ItemPackageId = itemCurrentPrices.ItemPackageId,
													   ExpireDate = itemCurrentPrices.ExpireDate,
													   BatchNumber = itemCurrentPrices.BatchNumber,
													   //Zero
												   }).ToList()
			};

			await _itemCurrentBalanceService.InventoryInOut(parentStoreId, oldItemCurrentBalances.ParentStoreCurrentBalances, parentStoreCurrentBalances);
			await _itemCurrentBalanceService.InventoryInOut(reservationStoreId, oldItemCurrentBalances.ReservationStoreCurrentBalances, reservationStoreCurrentBalances);
		}

		private async Task<string?> GetStockOutReturnDocumentReference(int stockOutReturnHeaderId, bool hasApprove, int? requestId)
		{
			if (stockOutReturnHeaderId != 0)
			{
				return null;
			}
			else if (hasApprove)
			{
				return $"{DocumentReferenceData.Approval}{requestId}";
			}
			else
			{
				return $"{DocumentReferenceData.StockOutReturnFromReservation}{await _stockOutReturnHeaderService.GetNextId()}";
			}
		}

		private async Task<ResponseDto> SaveRelatedInternalTransferAndReceive(StockOutReturnDto stockOutReturn, int resultStockOutReturnHeaderId, int reservationStoreId, string? documentReference)
		{
			var stockOutReturnHeader = stockOutReturn.StockOutReturnHeader!;

			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(stockOutReturnHeader.StockOutReturnHeaderId, MenuCodeData.StockOutReturnFromReservation);

			var internalTransfer = new InternalTransferDto
			{
				InternalTransferHeader = new InternalTransferHeaderDto
				{
					InternalTransferHeaderId = internalTransferHeaderId,
					DocumentDate = stockOutReturnHeader.DocumentDate,
					FromStoreId = stockOutReturnHeader.StoreId,
					ToStoreId = reservationStoreId,
					Reference = stockOutReturnHeader.Reference,
					TotalConsumerValue = stockOutReturn.StockOutReturnDetails.Sum(x => x.TotalValue),
					TotalCostValue = stockOutReturnHeader.TotalCostValue,
					RemarksAr = stockOutReturnHeader.RemarksAr,
					RemarksEn = stockOutReturnHeader.RemarksEn,
					IsReturned = false,
					ReturnReason = null,
					MenuCode = MenuCodeData.StockOutReturnFromReservation,
					ReferenceId = resultStockOutReturnHeaderId,
					IsClosed = true,
					ArchiveHeaderId = null,
				},
				InternalTransferDetails = (from stockOutReturnDetail in stockOutReturn.StockOutReturnDetails
										   select
										  new InternalTransferDetailDto
										  {
											  InternalTransferDetailId = 0,
											  InternalTransferHeaderId = internalTransferHeaderId,
											  ItemId = stockOutReturnDetail.ItemId,
											  ItemPackageId = stockOutReturnDetail.ItemPackageId,
											  BarCode = stockOutReturnDetail.BarCode,
											  Packing = stockOutReturnDetail.Packing,
											  Quantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity,
											  ConsumerPrice = stockOutReturnDetail.ConsumerPrice,
											  CostPrice = stockOutReturnDetail.CostPrice,
											  CostValue = stockOutReturnDetail.CostValue,
											  ExpireDate = stockOutReturnDetail.ExpireDate,
											  BatchNumber = stockOutReturnDetail.BatchNumber,
										  }).ToList()
			};

			return await _directInternalTransferService.SaveDirectInternalTransferWithoutUpdatingBalances(internalTransfer, false, true, null, documentReference);
		}

		public async Task<ResponseDto> DeleteStockOutReturnFromReservation(int stockOutReturnHeaderId, int menuCode)
		{
			ResponseDto result;
			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(stockOutReturnHeaderId, menuCode);
			var stockOutReturn = await _stockOutReturnService.GetStockOutReturn(stockOutReturnHeaderId);
			if (stockOutReturn.StockOutReturnHeader == null) return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

			var reservationStoreId = await _storeService.GetReservedStoreByParentStoreId(stockOutReturn.StockOutReturnHeader!.StoreId);

			result = await _directInternalTransferService.DeleteDirectInternalTransferWithoutUpdatingBalances(internalTransferHeaderId);
			if (result.Success == false) return result;

			result = await _stockOutReturnHandlingService.DeleteStockOutReturn(stockOutReturnHeaderId, menuCode, false);
			if (result.Success == false) return result;

			await DeductCurrentBalances(stockOutReturn.StockOutReturnHeader.StoreId, reservationStoreId, stockOutReturn.StockOutReturnDetails);

			return result;
		}
	}
}
