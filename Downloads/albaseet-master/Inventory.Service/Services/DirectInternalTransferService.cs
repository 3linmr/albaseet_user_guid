using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Services
{
	public class DirectInternalTransferService: IDirectInternalTransferService
	{
		private readonly IInternalTransferService _internalTransferService;
		private readonly IInternalTransferReceiveService _internalTransferReceiveService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;

		public DirectInternalTransferService(IInternalTransferService internalTransferService, IInternalTransferReceiveService internalTransferReceiveService, IItemCurrentBalanceService itemCurrentBalanceService)
		{
			_internalTransferService = internalTransferService;
			_internalTransferReceiveService = internalTransferReceiveService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
		}

		public async Task<int> GetInternalTransferIdFromReferenceAndMenuCode(int referenceId, int menuCode)
		{
			return await _internalTransferService.GetInternalTransferHeaderIdFromReferenceAndMenuCode(referenceId, menuCode);
		}

		public async Task<ResponseDto> SaveDirectInternalTransfer(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference)
		{
			List<InternalTransferDetailDto>? oldInternalTransferDetails = null;
			int? oldFromStoreId = null;
			int? oldToStoreId = null;
			if (internalTransfer.InternalTransferHeader!.InternalTransferHeaderId != 0)
			{
				var oldInternalTransfer = await _internalTransferService.GetInternalTransfer(internalTransfer.InternalTransferHeader.InternalTransferHeaderId);
				oldInternalTransferDetails = oldInternalTransfer.InternalTransferDetails;
				oldFromStoreId = oldInternalTransfer.InternalTransferHeader?.FromStoreId;
				oldToStoreId = oldInternalTransfer?.InternalTransferHeader?.ToStoreId;
			}

			var result = await SaveDirectInternalTransferItself(internalTransfer, hasApprove, approved, requestId, documentReference);
			if (result.Success)
			{
				await UpdateCurrentBalances(oldFromStoreId, oldToStoreId, internalTransfer.InternalTransferHeader.FromStoreId, internalTransfer.InternalTransferHeader.ToStoreId, oldInternalTransferDetails, internalTransfer.InternalTransferDetails);
			}
			return result;
		}

		public async Task<ResponseDto> SaveDirectInternalTransferWithoutUpdatingBalances(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference)
		{
			return await SaveDirectInternalTransferItself(internalTransfer, hasApprove, approved, requestId, documentReference);
		}

		private async Task<ResponseDto> SaveDirectInternalTransferItself(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference)
		{
			ResponseDto result;

			result = await _internalTransferService.SaveInternalTransferWithoutUpdatingBalances(internalTransfer, hasApprove, approved, requestId, documentReference);
			if (result.Success == false) return result;

			return await SaveInternalTransferReceive(internalTransfer, result.Id, hasApprove, approved, requestId, documentReference);
		}

		private async Task<ResponseDto> SaveInternalTransferReceive(InternalTransferDto internalTransfer, int resultInternalTransferHeaderId, bool hasApprove, bool approved, int? requestId, string? documentReference)
		{
			var internalTransferHeader = internalTransfer.InternalTransferHeader;

			var internalTransferReceiveHeaderId = await _internalTransferReceiveService.GetInternalTransferReceiveHeaderIdFromInternalTransferHeaderId(internalTransferHeader!.InternalTransferHeaderId);

			var internalTransferReceive = new InternalTransferReceiveDto
			{
				InternalTransferReceiveHeader = new InternalTransferReceiveHeaderDto
				{
					InternalTransferReceiveHeaderId = internalTransferReceiveHeaderId,
					InternalTransferHeaderId = resultInternalTransferHeaderId,
					DocumentDate = internalTransferHeader.DocumentDate,
					FromStoreId = internalTransferHeader.FromStoreId,
					ToStoreId = internalTransferHeader.ToStoreId,
					Reference = internalTransferHeader.Reference,
					TotalConsumerValue = internalTransferHeader.TotalConsumerValue,
					TotalCostValue = internalTransferHeader.TotalCostValue,
					RemarksAr = internalTransferHeader.RemarksAr,
					RemarksEn = internalTransferHeader.RemarksEn,
					IsReturned = internalTransferHeader.IsReturned,
					ReturnReason = internalTransferHeader.ReturnReason,
					MenuCode = internalTransferHeader.MenuCode,
					ReferenceId = internalTransferHeader.ReferenceId,
					ArchiveHeaderId = internalTransferHeader.ArchiveHeaderId,
					IsClosed = true,
				},
				InternalTransferReceiveDetails = (from internalTransferDetail in internalTransfer.InternalTransferDetails
												  select new InternalTransferReceiveDetailDto
												  {
													  InternalTransferReceiveDetailId = 0,
													  InternalTransferReceiveHeaderId = 0,
													  ItemId = internalTransferDetail.ItemId,
													  ItemPackageId = internalTransferDetail.ItemPackageId,
													  BarCode = internalTransferDetail.BarCode,
													  Packing = internalTransferDetail.Packing,
													  Quantity = internalTransferDetail.Quantity,
													  ConsumerPrice = internalTransferDetail.ConsumerPrice,
													  ConsumerValue = internalTransferDetail.ConsumerValue,
													  CostPrice = internalTransferDetail.CostPrice,
													  CostPackage = internalTransferDetail.CostPackage,
													  CostValue = internalTransferDetail.CostValue,
													  ExpireDate = internalTransferDetail.ExpireDate,
													  BatchNumber = internalTransferDetail.BatchNumber,
												  }).ToList()
			};

			return await _internalTransferReceiveService.SaveInternalTransferReceiveWithoutUpdatingBalances(internalTransferReceive, hasApprove, approved, requestId, documentReference);
		}

		public async Task<ResponseDto> DeleteDirectInternalTransfer(int internalTransferHeaderId)
		{
			var internalTransfer = await _internalTransferService.GetInternalTransfer(internalTransferHeaderId);

			var result = await DeleteDirectInternalTransferItself(internalTransferHeaderId);

			if (internalTransfer.InternalTransferHeader is not null && result.Success)
			{
				await DeductCurrentBalances(internalTransfer.InternalTransferHeader!.FromStoreId, internalTransfer.InternalTransferHeader!.ToStoreId, internalTransfer.InternalTransferDetails);
			}

			return result;
        }

		public async Task<ResponseDto> DeleteDirectInternalTransferWithoutUpdatingBalances(int internalTransferHeaderId)
		{
			return await DeleteDirectInternalTransferItself(internalTransferHeaderId);
		}

		private async Task<ResponseDto> DeleteDirectInternalTransferItself(int internalTransferHeaderId)
		{
			var internalTransferReceiveHeaderId = await _internalTransferReceiveService.GetInternalTransferReceiveHeaderIdFromInternalTransferHeaderId(internalTransferHeaderId);
			ResponseDto result;

			result = await _internalTransferReceiveService.DeleteInternalTransferReceiveWithoutUpdatingBalances(internalTransferReceiveHeaderId);
			if (result.Success == false) return result;

			result = await _internalTransferService.DeleteInternalTransferWithoutUpdatingBalances(internalTransferHeaderId);
			return result;
		}

		private async Task UpdateCurrentBalances(int? oldFromStoreId, int? oldToStoreId, int newFromStoreId, int newToStoreId, List<InternalTransferDetailDto>? oldInternalTransferDetails, List<InternalTransferDetailDto> newInternalTransferDetails)
		{
			var newItemBalances = GetItemCurrentBalancesFromInternalTransfer(newFromStoreId, newToStoreId, newInternalTransferDetails);
			if (oldInternalTransferDetails is null || oldFromStoreId is null)
			{
				await InventoryOut(newFromStoreId, newToStoreId, newItemBalances.FromStoreCurrentBalances, newItemBalances.ToStoreCurrentBalances);
			}
			else
			{
				if (oldFromStoreId == newFromStoreId && oldToStoreId == newToStoreId)
				{
					var oldItemBalances = GetItemCurrentBalancesFromInternalTransfer((int)oldFromStoreId, (int)oldToStoreId, oldInternalTransferDetails);
					await InventoryInOut((int)oldFromStoreId, (int)oldToStoreId!, oldItemBalances, newItemBalances);
				}
				else
				{
					await DeductCurrentBalances((int)oldFromStoreId, (int)oldToStoreId!, oldInternalTransferDetails);
					await InventoryOut(newFromStoreId, newToStoreId, newItemBalances.FromStoreCurrentBalances, newItemBalances.ToStoreCurrentBalances);
				}
			}
		}

		private async Task DeductCurrentBalances(int fromStoreId, int toStoreId, List<InternalTransferDetailDto> internalTransferDetails)
		{
			var oldItemCurrentBalances = GetItemCurrentBalancesFromInternalTransfer(fromStoreId, toStoreId, internalTransferDetails);

			var newItemCurrentBalances = new FromAndToStoreItemCurrentBalanceDto
			{
				FromStoreCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances.FromStoreCurrentBalances
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

				ToStoreCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances.ToStoreCurrentBalances
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

			await InventoryInOut(fromStoreId, toStoreId, oldItemCurrentBalances, newItemCurrentBalances);
		}

		private FromAndToStoreItemCurrentBalanceDto GetItemCurrentBalancesFromInternalTransfer(int fromStoreId, int toStoreId, List<InternalTransferDetailDto> internalTransferDetails)
		{
			var currentBalancesFromStoreId = (from internalTransferDetail in internalTransferDetails
								   select new ItemCurrentBalanceDto()
								   {
									   StoreId = fromStoreId,
									   ItemId = internalTransferDetail.ItemId,
									   ItemPackageId = internalTransferDetail.ItemPackageId,
									   ExpireDate = internalTransferDetail.ExpireDate,
									   BatchNumber = internalTransferDetail.BatchNumber,
									   OutQuantity = internalTransferDetail.Quantity
								   }).ToList();

			var currentBalancesToStoreId = (from internalTransferDetail in internalTransferDetails
											  select new ItemCurrentBalanceDto()
											  {
												  StoreId = toStoreId,
												  ItemId = internalTransferDetail.ItemId,
												  ItemPackageId = internalTransferDetail.ItemPackageId,
												  ExpireDate = internalTransferDetail.ExpireDate,
												  BatchNumber = internalTransferDetail.BatchNumber,
												  InQuantity = internalTransferDetail.Quantity
											  }).ToList();

			return new FromAndToStoreItemCurrentBalanceDto { FromStoreCurrentBalances = currentBalancesFromStoreId, ToStoreCurrentBalances = currentBalancesToStoreId };
		}

		private async Task InventoryInOut(int fromStoreId, int toStoreId, FromAndToStoreItemCurrentBalanceDto oldItemCurrentBalances, FromAndToStoreItemCurrentBalanceDto newItemCurrentBalances)
		{
			await _itemCurrentBalanceService.InventoryInOut(fromStoreId, oldItemCurrentBalances.FromStoreCurrentBalances, newItemCurrentBalances.FromStoreCurrentBalances);
			await _itemCurrentBalanceService.InventoryInOut(toStoreId, oldItemCurrentBalances.ToStoreCurrentBalances, newItemCurrentBalances.ToStoreCurrentBalances);
		}

		private async Task InventoryOut(int fromStoreId, int toStoreId, List<ItemCurrentBalanceDto> fromStoreCurrentBalances, List<ItemCurrentBalanceDto> toStoreCurrentBalances)
		{
			await _itemCurrentBalanceService.InventoryOut(fromStoreId, fromStoreCurrentBalances);
			await _itemCurrentBalanceService.InventoryIn(toStoreId, toStoreCurrentBalances);
		}
	}
}
