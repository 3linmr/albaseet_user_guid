using DevExtreme.AspNet.Data;
using Inventory.API.Models;
using Inventory.CoreOne.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Newtonsoft.Json;
using Inventory.Service.Services;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Inventory.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class StockTakingsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IStockTakingHeaderService _stockTakingHeaderService;
		private readonly IStockTakingService _stockTakingService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockTakingsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IStockTakingHeaderService stockTakingHeaderService, IStockTakingService stockTakingService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_stockTakingHeaderService = stockTakingHeaderService;
			_stockTakingService = stockTakingService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadStockTakings")]
		public async Task<IActionResult> ReadStockTakings(DataSourceLoadOptions loadOptions, bool isOpenBalance)
		{
			try {
				var data = await DataSourceLoader.LoadAsync(_stockTakingHeaderService.GetUserStockTakingHeaders(isOpenBalance), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _stockTakingHeaderService.GetUserStockTakingHeaders(isOpenBalance);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
		}

		[HttpGet]
		[Route("GetPendingStockTakings")]
		public async Task<IActionResult> GetPendingStockTakings(int storeId,bool isOpenBalance)
		{
			var data = await _stockTakingHeaderService.GetPendingStockTakings(storeId, isOpenBalance);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetStockTakingCode")]
		public async Task<IActionResult> GetStockTakingCode(int storeId, DateTime stockDate, bool isOpenBalance)
		{
			var data = await _stockTakingHeaderService.GetStockTakingCode(storeId, stockDate, isOpenBalance);
			return Ok(data);
		}


		[HttpGet]
		[Route("GetStockTakingCompareData")]
		public async Task<IActionResult> GetStockTakingCompareData(int storeId, bool isOpenBalance, DateTime carryOverDate, string? stockTakingsIds)
		{
			var jsonData = JsonConvert.DeserializeObject<List<int>>(stockTakingsIds?? "") ?? new List<int>();
			var data = await _stockTakingService.GetStockTakingCompareData(isOpenBalance, storeId,carryOverDate, jsonData);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<StockTakingDto>())
				{
					var stockTaking = data.ConvertToType<StockTakingDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, stockTaking?.StockTakingHeader?.StoreId ?? 0);
					return Ok(userCanLook ? stockTaking : new StockTakingDto());
				}
			}
			return Ok(new StockTakingDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetStockTaking(int id)
		{
			var stockTaking = await _stockTakingService.GetStockTaking(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockTaking.StockTakingHeader?.StoreId ?? 0);
			return Ok(userCanLook ? stockTaking : new StockTakingDto());
		}

		[HttpGet]
		[Route("GetStockTakingDetail")]
		public async Task<IActionResult> GetStockTakingDetail(int id)
		{
			var stockTaking = await _stockTakingService.GetStockTaking(id);
			return Ok(stockTaking.StockTakingDetails);
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] StockTakingDto model, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				if (model.StockTakingHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(model.StockTakingHeader.IsOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance);
					if (hasApprove.HasApprove)

					{
						if (hasApprove.OnAdd && model.StockTakingHeader!.StockTakingHeaderId == 0)
						{
							response = await SendApproveRequest(model, null, requestId, 0, false);
						}
						else if (hasApprove.OnEdit && model.StockTakingHeader!.StockTakingHeaderId > 0)
						{
							var oldValue = await _stockTakingService.GetStockTaking(model.StockTakingHeader!.StockTakingHeaderId);
							response = await SendApproveRequest(model, oldValue, requestId, model.StockTakingHeader!.StockTakingHeaderId, false);
						}
						else
						{
							response = await SaveStockTaking(model, hasApprove.HasApprove, false, requestId);
						}
					}
					else
					{
						response = await SaveStockTaking(model, hasApprove.HasApprove, false, requestId);
					}
                }
                else
                {
                    response.Message = "Header should not be null";
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
		[Route("SaveStockTaking")]
		public async Task<ResponseDto> SaveStockTaking(StockTakingDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _stockTakingService.SaveStockTaking(model, hasApprove, approved, requestId);
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

		[HttpDelete]
		[Route("DeleteStockTaking")]
		public async Task<IActionResult> DeleteStockTaking(int stockTakingHeaderId, int requestId)
		{
			var response = new ResponseDto();
			try
			{
				var model = await _stockTakingService.GetStockTaking(stockTakingHeaderId);
				if (model.StockTakingHeader != null)
				{
					var hasApprove = await _approvalSystemService.IsMenuHasApprove(model.StockTakingHeader.IsOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance);
					if (hasApprove.HasApprove && hasApprove.OnDelete)
					{
						response = await SendApproveRequest(null, model, requestId, stockTakingHeaderId, true);
					}
					else
					{
						await _databaseTransaction.BeginTransaction();
						response = await _stockTakingService.DeleteStockTaking(stockTakingHeaderId);
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
				else
				{
					response.Id = stockTakingHeaderId;
					response.Message = await _genericMessageService.GetMessage(MenuCodeData.StockTakingCurrentBalance, GenericMessageData.NotFound);
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
		public async Task<ResponseDto> SendApproveRequest([FromQuery] StockTakingDto? newModel, [FromQuery] StockTakingDto? oldModel, int requestId, int stockTakingHeaderId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var menuCode = 0;
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (stockTakingHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit || requestTypeId == ApproveRequestTypeData.Delete)
			{
				if (oldModel != null)
				{
					if (newModel != null)
					{
						changes = _stockTakingService.GetStockTakingRequestChanges(oldModel, newModel); 
					}

					if (oldModel.StockTakingHeader != null)
					{
						menuCode = oldModel.StockTakingHeader.IsOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance;

					}
				}
			}
			else
			{
				if (newModel != null)
				{
					if (newModel.StockTakingHeader != null)
					{
						menuCode = newModel.StockTakingHeader.IsOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance;

					}
				}
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = (short)menuCode,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = stockTakingHeaderId,
				ReferenceCode = stockTakingHeaderId.ToString(),
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
