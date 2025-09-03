using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.Service.Services.Inventory
{
	public class ItemDisassembleHandlerService : IItemDisassembleHandlerService
	{
		private readonly IItemDisassembleHeaderService _itemDisassembleHeaderService;
		private readonly IItemDisassembleDetailService _itemDisassembleDetailService;
		private readonly IItemDisassembleSerialService _itemDisassembleSerialService;
		private readonly IItemDisassembleLogicService _itemDisassembleLogicService;
		private readonly IStoreService _storeService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemDisassembleService _itemDisassembleService;
		private readonly IStringLocalizer<ItemDisassembleHandlerService> _localizer;

		public ItemDisassembleHandlerService(IItemDisassembleHeaderService itemDisassembleHeaderService,IItemDisassembleDetailService itemDisassembleDetailService,IItemDisassembleSerialService itemDisassembleSerialService,IItemDisassembleLogicService itemDisassembleLogicService,IStoreService storeService,IItemPackageService itemPackageService,IItemService itemService,IHttpContextAccessor httpContextAccessor,IItemDisassembleService itemDisassembleService,IStringLocalizer<ItemDisassembleHandlerService> localizer)
		{
			_itemDisassembleHeaderService = itemDisassembleHeaderService;
			_itemDisassembleDetailService = itemDisassembleDetailService;
			_itemDisassembleSerialService = itemDisassembleSerialService;
			_itemDisassembleLogicService = itemDisassembleLogicService;
			_storeService = storeService;
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
			_itemDisassembleService = itemDisassembleService;
			_localizer = localizer;
		}

		public IQueryable<ItemConversionDto> GetItemDisassembleHeaderWithOneDetail(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from itemDisassembleHeader in _itemDisassembleHeaderService.GetAll().Where(x=>x.IsAutomatic == false && x.StoreId == storeId)
				from store in _storeService.GetAll().Where(x => x.StoreId == itemDisassembleHeader.StoreId)
				from itemDisassembleDetail in _itemDisassembleDetailService.GetAll().Where(x=>x.ItemDisassembleHeaderId == itemDisassembleHeader.ItemDisassembleHeaderId)
				from formPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemDisassembleDetail.FromPackageId)
				from toPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemDisassembleDetail.ToPackageId)
				from item in _itemService.GetAll().Where(x => x.ItemId == itemDisassembleDetail.ItemId)
				select new ItemConversionDto
				{
					FromPackageId = itemDisassembleDetail.FromPackageId,
					ItemId = itemDisassembleDetail.ItemId,
					Packing = itemDisassembleDetail.Packing,
					ToPackageId = itemDisassembleDetail.ToPackageId,
					StoreId = itemDisassembleHeader.StoreId,
					RemarksAr = itemDisassembleHeader.RemarksAr,
					MenuCode = itemDisassembleHeader.MenuCode,
					BatchNumber = itemDisassembleDetail.BatchNumber,
					Quantity = itemDisassembleDetail.Quantity,
					ExpireDate = itemDisassembleDetail.ExpireDate,
					CreatedAt = itemDisassembleHeader.CreatedAt,
					DocumentDate = itemDisassembleHeader.DocumentDate,
					EntryDate = itemDisassembleHeader.EntryDate,
					FromPackageQuantityAfter = itemDisassembleDetail.FromPackageQuantityAfter,
					FromPackageQuantityBefore = itemDisassembleDetail.FromPackageQuantityBefore,
					IpAddressCreated = itemDisassembleHeader.IpAddressCreated,
					IsAutomatic = itemDisassembleHeader.IsAutomatic,
					IsSerialConversion = itemDisassembleDetail.IsSerialConversion,
					ItemDisassembleCode = itemDisassembleHeader.ItemDisassembleCode,
					ItemDisassembleHeaderId = itemDisassembleHeader.ItemDisassembleHeaderId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					RemarksEn = itemDisassembleHeader.RemarksEn,
					ToPackageQuantityAfter = itemDisassembleDetail.ToPackageQuantityAfter,
					ToPackageQuantityBefore = itemDisassembleDetail.ToPackageQuantityBefore,
					UserNameCreated = itemDisassembleHeader.UserNameCreated,
					FromPackageName = language == LanguageCode.Arabic ? formPackage.PackageNameAr : formPackage.PackageNameEn,
					ToPackageName = language == LanguageCode.Arabic ? toPackage.PackageNameAr : toPackage.PackageNameEn,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ReferenceDetailCode = itemDisassembleHeader.ReferenceHeaderId,
					ReferenceHeaderCode = itemDisassembleHeader.ReferenceDetailId
				};
			return data;
		}

		public async Task<ItemDisassembleInfoDto> GetItemDisassembleInfo(int itemDisassembleHeaderId)
		{
			var header = await _itemDisassembleHeaderService.GetItemDisassembleHeader(itemDisassembleHeaderId);
			var detail = await _itemDisassembleDetailService.GetItemDisassembleDetails(itemDisassembleHeaderId);
			var serial = await _itemDisassembleSerialService.GetItemDisassembleSerial(itemDisassembleHeaderId);

			var serialModel = serial.Select(x => new ItemPackageTreeDto()
			{
				ItemId = x.ItemId,
				ItemPackageId = x.ItemPackageId,
				MainItemPackageId = x.MainItemPackageId,
				ItemPackageName = x.ItemPackageName,
				ItemName = x.ItemName,
				ItemPackageBalanceAfter = x.ItemPackageBalanceAfter,
				ItemPackageBalanceBefore = x.ItemPackageBalanceBefore,
				ItemPackageNameWithBalanceAfter = $"{x.ItemPackageName} ({x.ItemPackageBalanceAfter})",
				ItemPackageNameWithBalanceBefore = $"{x.ItemPackageName} ({x.ItemPackageBalanceBefore})",
			}).ToList();

			var detailModel = detail.FirstOrDefault();

			if (detailModel!= null)
			{
				var itemDisassemble = new ItemConversionDto()
				{
					FromPackageId = detailModel.FromPackageId,
					ItemId = detailModel.ItemId,
					Packing = detailModel.Packing,
					ToPackageId = detailModel.ToPackageId,
					StoreId = header.StoreId,
					RemarksAr = header.RemarksAr,
					MenuCode = header.MenuCode,
					BatchNumber = detailModel.BatchNumber,
					Quantity = detailModel.Quantity,
					ExpireDate = detailModel.ExpireDate,
					CreatedAt = header.CreatedAt,
					DocumentDate = header.DocumentDate,
					EntryDate = header.EntryDate,
					FromPackageQuantityAfter = detailModel.FromPackageQuantityAfter,
					FromPackageQuantityBefore = detailModel.FromPackageQuantityBefore,
					IpAddressCreated = header.IpAddressCreated,
					IsAutomatic = header.IsAutomatic,
					IsSerialConversion = detailModel.IsSerialConversion,
					ItemDisassembleCode = header.ItemDisassembleCode,
					ItemDisassembleHeaderId = header.ItemDisassembleHeaderId,
					StoreName = header.StoreName,
					RemarksEn = header.RemarksEn,
					ItemName = detailModel.ItemName,
					MenuCodeName = header.MenuCodeName,
					ReferenceDetailCode = header.ReferenceDetailCode,
					ReferenceHeaderCode = header.ReferenceHeaderCode,
					ToPackageQuantityAfter = detailModel.ToPackageQuantityAfter,
					ToPackageQuantityBefore = detailModel.ToPackageQuantityBefore,
					UserNameCreated = header.UserNameCreated
				};

				return new ItemDisassembleInfoDto()
				{
					ItemDisassemble = itemDisassemble,
					ItemDisassembleSerial = serialModel
				};
			}
			return new ItemDisassembleInfoDto();
		}

		public async Task<ResponseDto> SaveItemDisassembleDocument(ItemConversionDto model)
		{
			var itemConversion = await _itemDisassembleLogicService.GetItemPackageConversion(model.ItemId, model.StoreId, model.FromPackageId, model.ExpireDate, model.BatchNumber, model.ToPackageId);
			var detailList = new List<ItemDisassembleDetailDto>();

			var header = new ItemDisassembleHeaderDto()
			{
				StoreId = model.StoreId,
				RemarksAr = model.RemarksAr,
				MenuCode = model.MenuCode,
				RemarksEn = model.RemarksEn,
				EntryDate = model.EntryDate,
				IsAutomatic = model.IsAutomatic,
				ItemDisassembleCode = model.ItemDisassembleCode,
				DocumentDate = model.DocumentDate,
				ItemDisassembleHeaderId = model.ItemDisassembleHeaderId
			};

			var detail = new ItemDisassembleDetailDto()
			{
				FromPackageId = model.FromPackageId,
				ItemId = model.ItemId,
				Packing = itemConversion.Packing,
				ToPackageId = model.ToPackageId,
				Quantity = model.Quantity,
				BatchNumber = model.BatchNumber,
				ExpireDate = model.ExpireDate,
				ItemDisassembleHeaderId = model.ItemDisassembleHeaderId,
				FromPackageQuantityBefore = itemConversion.FromPackageAvailableBalance,
				ToPackageQuantityBefore = itemConversion.ToPackageAvailableBalance,
				FromPackageQuantityAfter = itemConversion.FromPackageAvailableBalance - model.Quantity,
				ToPackageQuantityAfter = itemConversion.ToPackageAvailableBalance + (model.Quantity * itemConversion.Packing),
				IsSerialConversion = model.IsSerialConversion,
			};
			detailList.Add(detail);


			var serial = await _itemDisassembleLogicService.GetItemPackageBalanceAfterDisassembleQuantity(detail.ItemId,
				header.StoreId, detail.FromPackageId, detail.ToPackageId, detail.BatchNumber, detail.ExpireDate,
				detail.Quantity);

			var serialModel = serial.Select(x => new ItemDisassembleSerialDto()
			{
				ItemPackageId = x.ItemPackageId,
				ItemId = x.ItemId,
				MainItemPackageId = x.MainItemPackageId,
				ItemDisassembleHeaderId = model.ItemDisassembleHeaderId,
				ItemPackageBalanceAfter = x.ItemPackageBalanceAfter,
				ItemPackageBalanceBefore = x.ItemPackageBalanceBefore,
			}).ToList();


			var result = await _itemDisassembleHeaderService.SaveItemDisassembleHeader(header);
			if (result.Success)
			{
				await _itemDisassembleDetailService.SaveItemDisassembleDetails(result.Id,detailList);
				await _itemDisassembleSerialService.SaveItemDisassembleSerial(result.Id, serialModel);
				return await _itemDisassembleLogicService.SetItemPackageConversion(result.Id,model);
			}
			return new ResponseDto() { Success = false, Message = _localizer["ItemDisassembleDocumentFailed"] };
		}

		public async Task<ResponseDto> DeleteItemDisassembleDocument(int itemDisassembleHeaderId)
		{
			var result = await _itemDisassembleLogicService.ReverseItemPackageConversion(itemDisassembleHeaderId);
			if (result.Success)
			{
				await _itemDisassembleSerialService.DeleteItemDisassembleSerial(itemDisassembleHeaderId);
				await _itemDisassembleDetailService.DeleteItemDisassembleDetails(itemDisassembleHeaderId);
				await _itemDisassembleService.DeleteItemDisassemble(itemDisassembleHeaderId);
				await _itemDisassembleHeaderService.DeleteItemDisassembleHeader(itemDisassembleHeaderId);
				return result;
			}
			else
			{
				return result;
			}
		}
	}
}
