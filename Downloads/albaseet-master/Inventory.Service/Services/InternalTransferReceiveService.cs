using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Contracts.Inventory;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Service.Services
{
    public class InternalTransferReceiveService : IInternalTransferReceiveService
    {
        private readonly IInternalTransferReceiveHeaderService _internalTransferReceiveHeaderService;
        private readonly IInternalTransferReceiveDetailService _internalTransferReceiveDetailService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IInternalTransferService _internalTransferService;
        private readonly IStoreService _storeService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStringLocalizer<InternalTransferReceiveService> _localizer;

        public InternalTransferReceiveService(IInternalTransferReceiveHeaderService internalTransferReceiveHeaderService, IInternalTransferReceiveDetailService internalTransferReceiveDetailService, IItemCurrentBalanceService itemCurrentBalanceService, IInternalTransferService internalTransferService, IStringLocalizer<InternalTransferReceiveService> localizer, IStoreService storeService, IMenuNoteService menuNoteService)
        {
            _internalTransferReceiveHeaderService = internalTransferReceiveHeaderService;
            _internalTransferReceiveDetailService = internalTransferReceiveDetailService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _internalTransferService = internalTransferService;
            _localizer = localizer;
            _storeService = storeService;
            _menuNoteService = menuNoteService;
        }

        public async Task<InternalTransferReceiveDto> GetInternalTransferReceive(int internalTransferReceiveHeaderId)
        {
            var header = await _internalTransferReceiveHeaderService.GetInternalTransferReceiveHeaderById(internalTransferReceiveHeaderId);
            var detail = await _internalTransferReceiveDetailService.GetInternalTransferReceiveDetails(internalTransferReceiveHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.InternalReceiveItems, internalTransferReceiveHeaderId).ToListAsync();
            return new InternalTransferReceiveDto() { InternalTransferReceiveHeader = header, InternalTransferReceiveDetails = detail, MenuNotes = menuNotes };
        }

        public async Task<int> GetInternalTransferReceiveHeaderIdFromInternalTransferHeaderId(int internalTransferHeaderId)
        {
            return await _internalTransferReceiveHeaderService.GetAll().Where(x => x.InternalTransferHeaderId == internalTransferHeaderId).Select(x => x.InternalTransferReceiveHeaderId).FirstOrDefaultAsync();
        }

		public async Task<ResponseDto> SaveInternalTransferReceive(InternalTransferReceiveDto internalTransferReceive, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(internalTransferReceive.InternalTransferReceiveDetails);
            if (internalTransferReceive.InternalTransferReceiveHeader == null)
            {
                return new ResponseDto{Message = "Header should not be null"};
            }

            if (internalTransferReceive.InternalTransferReceiveHeader.InternalTransferReceiveHeaderId != 0)
            {
				return new ResponseDto { Message = "You cannot update an internalTransferReceive" };
			}

			int internalTransferId = internalTransferReceive.InternalTransferReceiveHeader.InternalTransferHeaderId;
            bool isReturned = internalTransferReceive.InternalTransferReceiveHeader.IsReturned;
            string? returnReason = internalTransferReceive.InternalTransferReceiveHeader.ReturnReason;

            bool internalTransferExists = await _internalTransferService.UpdateReturned(internalTransferId, isReturned, returnReason);
            
            if (!internalTransferExists)
            {
                return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NotFound"] };
            }

            await DeductFromStore(internalTransferReceive.InternalTransferReceiveHeader.FromStoreId, internalTransferReceive.InternalTransferReceiveDetails, isReturned);

            if (!isReturned)
            {
                await IncreaseToStore(internalTransferReceive.InternalTransferReceiveHeader.ToStoreId, internalTransferReceive.InternalTransferReceiveDetails);
                return await SaveInternalTransferReceiveToDb(internalTransferReceive, hasApprove, approved, requestId, null, false);
            }
            else
            {
                return new ResponseDto() { Id = internalTransferId, Success = true, Message = _localizer["ReturnSuccess"] };
            }
        }

        public async Task<ResponseDto> SaveInternalTransferReceiveWithoutUpdatingBalances(InternalTransferReceiveDto internalTransferReceive, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            TrimDetailStrings(internalTransferReceive.InternalTransferReceiveDetails);
            return await SaveInternalTransferReceiveToDb(internalTransferReceive, hasApprove, approved, requestId, documentReference, true);
		}

		private async Task<ResponseDto> SaveInternalTransferReceiveToDb(InternalTransferReceiveDto internalTransferReceive, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
        {
	        if (internalTransferReceive.InternalTransferReceiveHeader != null)
	        {
                var result = await _internalTransferReceiveHeaderService.SaveInternalTransferReceiveHeader(internalTransferReceive.InternalTransferReceiveHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
		        if (result.Success)
		        {
			        await _internalTransferReceiveDetailService.SaveInternalTransferReceiveDetails(result.Id, internalTransferReceive.InternalTransferReceiveDetails);
                    if (internalTransferReceive.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(internalTransferReceive.MenuNotes, result.Id);
                    }
                }
		        return result;
	        }
	        return new ResponseDto { Message = "Header should not be null"};
        }

        private void TrimDetailStrings(List<InternalTransferReceiveDetailDto> internalTransferReceiveDetails)
        {
            foreach (var internalTransferReceiveDetail in internalTransferReceiveDetails)
            {
                internalTransferReceiveDetail.BatchNumber = string.IsNullOrWhiteSpace(internalTransferReceiveDetail.BatchNumber) ? null : internalTransferReceiveDetail.BatchNumber.Trim();
            }
        }

		public async Task<ResponseDto> DeleteInternalTransferReceiveWithoutUpdatingBalances(int internalTransferReceiveHeaderId)
		{
			await _menuNoteService.DeleteMenuNotes(MenuCodeData.InternalReceiveItems, internalTransferReceiveHeaderId);
			await _internalTransferReceiveDetailService.DeleteInternalTransferReceiveDetails(internalTransferReceiveHeaderId);
			return await _internalTransferReceiveHeaderService.DeleteInternalTransferReceiveHeader(internalTransferReceiveHeaderId);
		}

		private async Task DeductFromStore(int fromStoreId, List<InternalTransferReceiveDetailDto> internalTransferReceiveDetails, bool isReturned)
        {
            List<ItemCurrentBalanceDto>? oldFromBalances = (from internalTransferReceiveDetail in internalTransferReceiveDetails
                                                            select new ItemCurrentBalanceDto()
                                                            {
                                                                StoreId = fromStoreId,
                                                                ItemId = internalTransferReceiveDetail.ItemId,
                                                                ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
                                                                ExpireDate = internalTransferReceiveDetail.ExpireDate,
                                                                BatchNumber = internalTransferReceiveDetail.BatchNumber,
                                                                PendingOutQuantity = internalTransferReceiveDetail.Quantity
                                                            }).ToList(); ;


            List<ItemCurrentBalanceDto>? newFromBalances = (from internalTransferReceiveDetail in internalTransferReceiveDetails
                                                            select new ItemCurrentBalanceDto()
                                                            {
                                                                StoreId = fromStoreId,
                                                                ItemId = internalTransferReceiveDetail.ItemId,
                                                                ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
                                                                ExpireDate = internalTransferReceiveDetail.ExpireDate,
                                                                BatchNumber = internalTransferReceiveDetail.BatchNumber,
                                                                PendingOutQuantity = 0,
                                                                OutQuantity = isReturned? 0 : internalTransferReceiveDetail.Quantity
                                                            }).ToList();

            await _itemCurrentBalanceService.InventoryInOut(fromStoreId, oldFromBalances, newFromBalances);
        }

        private async Task IncreaseToStore(int toStoreId, List<InternalTransferReceiveDetailDto> internalTransferReceiveDetails)
        {
            List<ItemCurrentBalanceDto>? newToBalances = (from internalTransferReceiveDetail in internalTransferReceiveDetails
                                                            select new ItemCurrentBalanceDto()
                                                            {
                                                                StoreId = toStoreId,
                                                                ItemId = internalTransferReceiveDetail.ItemId,
                                                                ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
                                                                ExpireDate = internalTransferReceiveDetail.ExpireDate,
                                                                BatchNumber = internalTransferReceiveDetail.BatchNumber,
                                                                PendingOutQuantity = 0,
                                                                InQuantity = internalTransferReceiveDetail.Quantity
                                                            }).ToList();

            await _itemCurrentBalanceService.InventoryIn(toStoreId, newToBalances);
        }

		public async Task<InternalTransferReceiveDto> GetTransferReceiveFromInternalTransfer(int internalTransferId)
		{
			var internalTransfer = await _internalTransferService.GetInternalTransfer(internalTransferId);
            if (internalTransfer.InternalTransferHeader == null) return new InternalTransferReceiveDto();

			var receive = new InternalTransferReceiveDto
			{
				InternalTransferReceiveHeader = new InternalTransferReceiveHeaderDto
				{
					InternalTransferReceiveHeaderId = 0,
                    Prefix = null,
                    InternalTransferReceiveCode = 0,
                    Suffix = null,
                    InternalTransferReceiveFullCode = internalTransfer.InternalTransferHeader.InternalTransferFullCode,
                    DocumentReference = internalTransfer.InternalTransferHeader.DocumentReference,
					InternalTransferHeaderId = internalTransferId,
					DocumentDate = internalTransfer.InternalTransferHeader.DocumentDate,
                    EntryDate = internalTransfer.InternalTransferHeader.EntryDate,
					FromStoreId = internalTransfer.InternalTransferHeader.FromStoreId,
                    FromStoreName = internalTransfer.InternalTransferHeader.FromStoreName,
					ToStoreId = internalTransfer.InternalTransferHeader.ToStoreId,
                    ToStoreName = internalTransfer.InternalTransferHeader.ToStoreName,
					Reference = internalTransfer.InternalTransferHeader.Reference,
					TotalConsumerValue = internalTransfer.InternalTransferHeader.TotalConsumerValue,
					TotalCostValue = internalTransfer.InternalTransferHeader.TotalCostValue,
					RemarksAr = internalTransfer.InternalTransferHeader.RemarksAr,
					RemarksEn = internalTransfer.InternalTransferHeader.RemarksEn,
					IsReturned = false,
					ReturnReason = null,
                    MenuCode = internalTransfer.InternalTransferHeader.MenuCode,
                    ReferenceId = internalTransfer.InternalTransferHeader.ReferenceId,
                    ArchiveHeaderId = internalTransfer.InternalTransferHeader.ArchiveHeaderId,
					IsClosed = internalTransfer.InternalTransferHeader.IsClosed,
				},
				InternalTransferReceiveDetails = (from detail in internalTransfer.InternalTransferDetails
												  select new InternalTransferReceiveDetailDto
												  {
                                                      //InternalTransferReceiveDetailId = to be serialized
                                                      InternalTransferReceiveHeaderId = 0,
													  ItemId = detail.ItemId,
													  ItemCode = detail.ItemCode,
													  ItemName = detail.ItemName,
                                                      ItemTypeId = detail.ItemTypeId,
													  ItemPackageId = detail.ItemPackageId,
													  ItemPackageName = detail.ItemPackageName,
													  BarCode = detail.BarCode,
													  Packing = detail.Packing,
													  Quantity = detail.Quantity,
													  ConsumerPrice = detail.ConsumerPrice,
													  ConsumerValue = detail.ConsumerValue,
													  CostPrice = detail.CostPrice,
													  CostPackage = detail.CostPackage,
													  CostValue = detail.CostValue,
													  ExpireDate = detail.ExpireDate,
													  BatchNumber = detail.BatchNumber,
													  CreatedAt = detail.CreatedAt,
													  UserNameCreated = detail.UserNameCreated,
													  IpAddressCreated = detail.IpAddressCreated,
													  Packages = detail.Packages
												  }).ToList()
			};

            var index = -1;
            receive.InternalTransferReceiveDetails.ForEach(x => x.InternalTransferReceiveDetailId = index--);

			return receive;
		}
	}
}
