using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Items;
using System.ComponentModel.Design;
using Shared.Helper.Identity;
using System.Text.Json;

namespace Shared.API.Controllers.Items
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemService _itemService;
		private readonly IItemAttributeService _itemAttributeService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IItemCostCalculationTypeService _itemCostCalculationTypeService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IItemService itemService, IItemAttributeService itemAttributeService, IItemBarCodeService itemBarCodeService, IItemCostCalculationTypeService itemCostCalculationTypeService, IItemTypeService itemTypeService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IItemPackingService itemPackingService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_itemService = itemService;
			_itemAttributeService = itemAttributeService;
			_itemBarCodeService = itemBarCodeService;
			_itemCostCalculationTypeService = itemCostCalculationTypeService;
			_itemTypeService = itemTypeService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_itemPackingService = itemPackingService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadItems")]
		public IActionResult ReadItems(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_itemService.GetUserItems(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllItems")]
		public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllItems()
		{
			var allItems = await _itemService.GetAllItems().ToListAsync();
			return Ok(allItems);
		}
		
		[HttpGet]
		[Route("GetItemPackages")]
		public async Task<ActionResult<List<int>>> GetItemPackages(int itemId)
		{
			var data = await _itemBarCodeService.GetItemPackages(itemId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemPackagesDropDown")]
		public async Task<ActionResult<List<ItemPackageDropDownDto>>> GetItemPackagesDropDown(int itemId)
		{
			var data = await _itemBarCodeService.GetItemPackagesDropDown(itemId);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetItemPackagesWithoutSingularDropDown")]
		public async Task<ActionResult<List<ItemPackageDropDownDto>>> GetItemPackagesWithoutSingularDropDown(int itemId)
		{
			var data = await _itemBarCodeService.GetItemPackagesWithoutSingularDropDown(itemId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetSingularItemPacking")]
		public async Task<ActionResult<decimal>> GetSingularItemPacking(int itemId,int fromPackageId)
		{
			var data = await _itemBarCodeService.GetSingularItemPacking(itemId,fromPackageId);
			return Ok(data);
		}


		[HttpGet]
		[Route("GetItemTypesDropDown")]
		public async Task<ActionResult<IEnumerable<ItemTypeDropDownDto>>> GetItemTypesDropDown()
		{
			var data = await _itemTypeService.GetItemTypesDropDown().ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemCostCalculationTypesDropDown")]
		public async Task<ActionResult<IEnumerable<ItemCostCalculationTypeDropDownDto>>> GetItemCostCalculationTypesDropDown()
		{
			var data = await _itemCostCalculationTypeService.GetItemCostCalculationTypesDropDown().ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemsAutoComplete")]
		public async Task<ActionResult<IEnumerable<ItemAutoCompleteVm>>> GetItemsAutoComplete(string term)
		{
			var data = await _itemService.GetItemsAutoComplete(term);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemsAutoCompleteByStoreIds")]
		public async Task<ActionResult<IEnumerable<ItemAutoCompleteVm>>> GetItemsAutoCompleteByStoreIds(string term, string? storeIds)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]")!;
			var data = await _itemService.GetItemsAutoCompleteByStoreIds(term, storeIdsList);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemById")]
		public async Task<IActionResult> GetItemById(int id)
		{
			var itemDb = await _itemService.GetItemById(id) ?? new ItemDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(itemDb.CompanyId, 0);
			return Ok(userCanLook ? itemDb : new ItemDto());
		}

		[HttpGet]
		[Route("GetFullItemById")]
		public async Task<IActionResult> GetFullItemById(int id)
		{
			var itemDb = await _itemService.GetItem(id) ?? new ItemVm();
			var userCanLook = await _httpContextAccessor.UserCanLook(itemDb.Item.CompanyId, 0);
			return Ok(userCanLook ? itemDb : new ItemVm());
		}

		[HttpGet]
		[Route("GetItemBarCodesByItemId")]
		public async Task<ActionResult<IEnumerable<ItemBarCodeDto>>> GetItemBarCodesByItemId(int itemId)
		{
			var data = await _itemService.GetItemBarCodeByItemId(itemId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemAttributesByItemId")]
		public async Task<ActionResult<IEnumerable<ItemBarCodeDto>>> GetItemAttributesByItemId(int itemId)
		{
			var data = await _itemAttributeService.GetItemAttributesByItemId(itemId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("IsItemsVatInclusive")]
		public async Task<IActionResult> IsItemsVatInclusive(int storeId)
		{
			var itemDb = await _itemService.IsItemsVatInclusive(storeId);
			return Ok(itemDb);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if ( data.CanBeConverted<ItemVm>())
				{
					var convertedData = data.ConvertToType<ItemVm>();
					var userCanLook = await _httpContextAccessor.UserCanLook(convertedData?.Item?.CompanyId ?? 0, 0);
					return Ok(userCanLook ? convertedData : new ItemVm());
				}
			}
			return Ok(new ItemVm());
		}

		
		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] ItemVm model, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.Item);
				if (hasApprove.HasApprove)
				{
					if (hasApprove.OnAdd && model.Item.ItemId == 0)
					{
						response = await SendApproveRequest(model,null, requestId,0, false);
					}
					else if (hasApprove.OnEdit && model.Item.ItemId > 0)
					{
						var oldValue = await _itemService.GetItem(model.Item.ItemId);
						response = await SendApproveRequest(model, oldValue,requestId,model.Item.ItemId,false);
					}
					else
					{
						response = await SaveItem(model);
					}
				}
				else
				{
					response = await SaveItem(model);
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("UpdateItemPackigns/{id:int}")]
		public async Task<IActionResult> UpdateItemPackings(int id)
		{
			var itemBarCodes = await _itemBarCodeService.GetItemBarCodesByItemId(id);
			var result = await _itemPackingService.UpdateItemPackings(id, itemBarCodes);
			return Ok(result);
		} 


		[HttpPost]
		[Route("SaveItem")]
		public async Task<ResponseDto> SaveItem(ItemVm model)
		{
			await _databaseTransaction.BeginTransaction();

			var response = await _itemService.SaveItemInFull(model);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				response.Success = false;
				response.Message = response.Message;
				await _databaseTransaction.Rollback();
			}

			return response;
		}

		[HttpDelete]
		[Route("DeleteItem")]
		public async Task<IActionResult> DeleteItem(int itemId,int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.Item);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _itemService.GetItem(itemId);
					response = await SendApproveRequest(null,model, requestId, itemId,true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _itemService.DeleteItemInFull(itemId);
					if (response.Success)
					{
						await _databaseTransaction.Commit();
					}
					else
					{
						await _databaseTransaction.Rollback();
					}
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("SendApproveRequest")]
		public async Task<ResponseDto> SendApproveRequest([FromQuery] ItemVm? newModel, [FromQuery]  ItemVm? oldModel,int requestId,int itemId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (itemId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _itemService.GetItemRequestChanges(oldModel!, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = MenuCodeData.Item,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = itemId,
				ReferenceCode = itemId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var response = await _handleApprovalRequestService.SaveRequest(request);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}

	}
}
