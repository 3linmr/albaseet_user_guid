using Inventory.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Contracts.Inventory;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Purchases.CoreOne.Models.Domain;
using Microsoft.Extensions.Localization;
using Shared.Service.Services.Inventory;

namespace Inventory.Service.Services
{
    public class InternalTransferService : IInternalTransferService
    {
        private readonly IInternalTransferHeaderService _internalTransferHeaderService;
        private readonly IInternalTransferDetailService _internalTransferDetailService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IStoreService _storeService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStringLocalizer<InternalTransferHeaderService> _localizer;
        private readonly IZeroStockValidationService _zeroStockValidationService;

        public InternalTransferService(IInternalTransferHeaderService internalTransferHeaderService, IInternalTransferDetailService internalTransferDetailService, IItemCurrentBalanceService itemCurrentBalanceService, IStoreService storeService, IMenuNoteService menuNoteService, IStringLocalizer<InternalTransferHeaderService> localizer, IZeroStockValidationService zeroStockValidationService)
        {
            _internalTransferHeaderService = internalTransferHeaderService;
            _internalTransferDetailService = internalTransferDetailService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _storeService = storeService;
            _menuNoteService = menuNoteService;
            _localizer = localizer;
            _zeroStockValidationService = zeroStockValidationService;
        }

        public List<RequestChangesDto> GetInternalTransferRequestChanges(InternalTransferDto oldItem, InternalTransferDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.InternalTransferHeader, newItem.InternalTransferHeader);
            requestChanges.AddRange(items);

            if (oldItem.InternalTransferDetails.Any() && newItem.InternalTransferDetails.Any())
            {
                var oldCount = oldItem.InternalTransferDetails.Count;
                var newCount = newItem.InternalTransferDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.InternalTransferDetails[i], newItem.InternalTransferDetails[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }

            if (oldItem.MenuNotes != null && oldItem.MenuNotes.Any() && newItem.MenuNotes != null && newItem.MenuNotes.Any())
            {
                var oldCount = oldItem.MenuNotes.Count;
                var newCount = newItem.MenuNotes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }
            return requestChanges;
        }

        public async Task<InternalTransferDto> GetInternalTransfer(int internalTransferHeaderId)
        {
            var header = await _internalTransferHeaderService.GetInternalTransferHeaderById(internalTransferHeaderId);
            if (header == null) return new InternalTransferDto();

            var detail = await _internalTransferDetailService.GetInternalTransferDetails(internalTransferHeaderId);
            await ModifyInternalTransferDetailsWithStoreIdAndAvaialbleBalance(internalTransferHeaderId, header.FromStoreId, detail, false);

            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.InternalTransferItems, internalTransferHeaderId).ToListAsync();
            return new InternalTransferDto() { InternalTransferHeader = header, InternalTransferDetails = detail, MenuNotes = menuNotes };
		}

		public async Task ModifyInternalTransferDetailsWithStoreIdAndAvaialbleBalance(int internalTransferHeaderId, int storeId, List<InternalTransferDetailDto> details, bool isRequestData)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();

			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, false).Where(x => x.StoreId == storeId && itemIds.Contains(x.ItemId)).ToListAsync();
			var currentInternalTransferDetails = isRequestData ? await _internalTransferDetailService.GetInternalTransferDetails(internalTransferHeaderId) : details;

			var filteredAvailableBalances = availableBalances.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.AvailableBalance });
			var filteredCurrentDetails = currentInternalTransferDetails.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.Quantity });

			var finalAvailableBalances = filteredAvailableBalances.Concat(filteredCurrentDetails).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

			foreach (var detail in details)
			{
				detail.StoreId = storeId;
				detail.AvailableBalance = finalAvailableBalances.GetValueOrDefault((detail.ItemId, detail.ItemPackageId, detail.ExpireDate, detail.BatchNumber), 0);
			}
		}

		public async Task<int> GetInternalTransferHeaderIdFromReferenceAndMenuCode(int referenceId, int menuCode)
        {
            return await _internalTransferHeaderService.GetAll().Where(x => x.ReferenceId == referenceId && x.MenuCode == menuCode).Select(x => x.InternalTransferId).FirstOrDefaultAsync();
        }


		public async Task<bool> UpdateReturned(int internalTransferId, bool isReturned, string? returnedReason)
        {
            return await _internalTransferHeaderService.UpdateReturned(internalTransferId, isReturned, returnedReason);
        }

        public async Task<bool> UpdateClosed(int internalTransferId)
        {
            return await _internalTransferHeaderService.UpdateClosed(internalTransferId);
        }

        public async Task<ResponseDto> SaveInternalTransfer(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(internalTransfer.InternalTransferDetails);
            if (internalTransfer.InternalTransferHeader != null)
            {
                List<InternalTransferDetailDto>? oldInternalTransferDetails = [];
                int? oldStoreId = null;
                if (internalTransfer.InternalTransferHeader.InternalTransferHeaderId != 0)
                {
                    var oldInternalTransfer = await GetInternalTransfer(internalTransfer.InternalTransferHeader.InternalTransferHeaderId);
                    oldInternalTransferDetails = oldInternalTransfer.InternalTransferDetails;
                    oldStoreId = oldInternalTransfer.InternalTransferHeader?.FromStoreId;
                }

                var canAcceptDirectInternalTransfer = await _storeService.GetAll().Where(x => x.StoreId == internalTransfer.InternalTransferHeader.ToStoreId).Select(x => (bool?)x.CanAcceptDirectInternalTransfer).FirstOrDefaultAsync();

                if (canAcceptDirectInternalTransfer == false)
                {
                    return new ResponseDto { Id = internalTransfer.InternalTransferHeader.InternalTransferHeaderId, Success = false, Message = _localizer["StoreNotAcceptTransfer"] };
				}

				var zeroStockValidationResult = await CheckInventoryZeroStock(internalTransfer.InternalTransferHeader.FromStoreId, internalTransfer.InternalTransferDetails, oldInternalTransferDetails);
				if (zeroStockValidationResult.Success == false) return zeroStockValidationResult;

				var result = await SaveInternalTransferInternal(internalTransfer, hasApprove, approved, requestId, null, true, false);
                if (result.Success)
                {
                    await UpdateCurrentBalances(oldStoreId, internalTransfer.InternalTransferHeader.FromStoreId, oldInternalTransferDetails, internalTransfer.InternalTransferDetails);
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
		}

		public async Task<ResponseDto> CheckInventoryZeroStock(int storeId, List<InternalTransferDetailDto> newDetails, List<InternalTransferDetailDto> oldDetails)
		{
			return await _zeroStockValidationService.ValidateZeroStock(
				storeId: storeId,
				newDetails: newDetails,
				oldDetails: oldDetails,
				detailKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
				itemIdSelector: x => x.ItemId,
				quantitySelector: x => x.Quantity,
				availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
				isGrouped: false,
				menuCode: MenuCodeData.InternalTransferItems,
				settingMenuCode: MenuCodeData.InternalTransferItems,
				isSave: true
				);
		}

		public async Task<ResponseDto> SaveInternalTransferWithoutUpdatingBalances(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
			TrimDetailStrings(internalTransfer.InternalTransferDetails);
			return await SaveInternalTransferInternal(internalTransfer, hasApprove, approved, requestId, documentReference, false, true);
		}

		private async Task<ResponseDto> SaveInternalTransferInternal(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldValidate, bool shouldInitializeFlags)
		{
			if (internalTransfer.InternalTransferHeader != null)
			{

				var result = await _internalTransferHeaderService.SaveInternalTransferHeader(internalTransfer.InternalTransferHeader, hasApprove, approved, requestId, documentReference, shouldValidate, shouldInitializeFlags);
				if (result.Success)
				{
					await _internalTransferDetailService.SaveInternalTransferDetails(result.Id, internalTransfer.InternalTransferDetails);
					if (internalTransfer.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(internalTransfer.MenuNotes, result.Id);
					}
				}

				return result;
			}
			return new ResponseDto { Message = "Header should not be null" };
		}

		private void TrimDetailStrings(List<InternalTransferDetailDto> internalTransferDetails)
        {
            foreach (var internalTransferDetail in internalTransferDetails)
            {
                internalTransferDetail.BatchNumber = string.IsNullOrWhiteSpace(internalTransferDetail.BatchNumber) ? null : internalTransferDetail.BatchNumber.Trim();
            }
        }

        public async Task<ResponseDto> DeleteInternalTransfer(int internalTransferHeaderId)
        {
            var internalTransfer = await GetInternalTransfer(internalTransferHeaderId);

            var result = await DeleteInternalTransferInternal(internalTransferHeaderId);

            if (internalTransfer.InternalTransferHeader is not null && result.Success)
            {
                await DeductCurrentBalances(internalTransfer.InternalTransferHeader.FromStoreId, internalTransfer.InternalTransferDetails);
            }

            return result;
        }

        public async Task<ResponseDto> DeleteInternalTransferWithoutUpdatingBalances(int internalTransferHeaderId)
        {
            return await DeleteInternalTransferInternal(internalTransferHeaderId);
		}

		private async Task<ResponseDto> DeleteInternalTransferInternal(int internalTransferHeaderId)
		{
			await _menuNoteService.DeleteMenuNotes(MenuCodeData.InternalTransferItems, internalTransferHeaderId);
			await _internalTransferDetailService.DeleteInternalTransferDetails(internalTransferHeaderId);
			return await _internalTransferHeaderService.DeleteInternalTransferHeader(internalTransferHeaderId);
		}

		public async Task<bool> GetInternalTransferPendingItems(int storeId, bool isAllItems, IEnumerable<int> itemIds)
        {
            if (isAllItems)
            {
                return await _internalTransferHeaderService.FindBy(x => x.IsClosed == false && x.FromStoreId == storeId).AsNoTracking().AnyAsync();
            }
            else
            {
                return await (from internalTransferHeader in _internalTransferHeaderService.FindBy(x => x.IsClosed == false && x.FromStoreId == storeId).AsNoTracking()
                              join internalTransferDetail in _internalTransferDetailService.GetAll().AsNoTracking() on internalTransferHeader.InternalTransferId equals internalTransferDetail.InternalTransferHeaderId
                              where itemIds.Contains(internalTransferDetail.ItemId)
                              select internalTransferDetail.ItemId).AnyAsync();
            }
        }

        private async Task UpdateCurrentBalances(int? oldStoreId, int newStoreId, List<InternalTransferDetailDto>? oldInternalTransferDetails, List<InternalTransferDetailDto> newInternalTransferDetails)
        {
            var newItemBalances = GetItemCurrentBalancesFromInternalTransfer(newStoreId, newInternalTransferDetails);
            if (oldInternalTransferDetails is null || oldStoreId is null)
            {
                await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
            }
            else
            {
                if (oldStoreId == newStoreId)
                {
                    var oldItemBalances = GetItemCurrentBalancesFromInternalTransfer(newStoreId, oldInternalTransferDetails);
                    await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
                }
                else
                {
                    await DeductCurrentBalances((int)oldStoreId, oldInternalTransferDetails);
                    await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
                }
            }
        }

        private async Task DeductCurrentBalances(int storeId, List<InternalTransferDetailDto> internalTransferDetails)
        {
            var oldItemCurrentBalances = GetItemCurrentBalancesFromInternalTransfer(storeId, internalTransferDetails);

            var newItemCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances
                                          select new ItemCurrentBalanceDto()
                                          {
                                              ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
                                              StoreId = itemCurrentPrices.StoreId,
                                              ItemId = itemCurrentPrices.ItemId,
                                              ItemPackageId = itemCurrentPrices.ItemPackageId,
                                              ExpireDate = itemCurrentPrices.ExpireDate,
                                              BatchNumber = itemCurrentPrices.BatchNumber,
                                              //Zero
                                          }).ToList();

            await _itemCurrentBalanceService.InventoryInOut(storeId, oldItemCurrentBalances, newItemCurrentBalances);
        }

        private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromInternalTransfer(int storeId, List<InternalTransferDetailDto> internalTransferDetails)
        {
            var currentBalances = (from internalTransferDetail in internalTransferDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = internalTransferDetail.ItemId,
                                       ItemPackageId = internalTransferDetail.ItemPackageId,
                                       ExpireDate = internalTransferDetail.ExpireDate,
                                       BatchNumber = internalTransferDetail.BatchNumber,
                                       PendingOutQuantity = internalTransferDetail.Quantity
                                   }).ToList();

            return currentBalances;
        }
    }
}
