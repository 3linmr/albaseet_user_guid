using DevExtreme.AspNet.Data;
using Inventory.API.Models;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Inventory.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Inventory.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class StockTakingCarryOverController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStockTakingCarryOverHeaderService _stockTakingCarryOverHeaderService;
		private readonly IStockTakingCarryOverService _stockTakingCarryOverService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockTakingCarryOverController(IDatabaseTransaction databaseTransaction,IStockTakingCarryOverHeaderService stockTakingCarryOverHeaderService,IStockTakingCarryOverService stockTakingCarryOverService,IHandleApprovalRequestService handleApprovalRequestService,IApprovalSystemService approvalSystemService, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_stockTakingCarryOverHeaderService = stockTakingCarryOverHeaderService;
			_stockTakingCarryOverService = stockTakingCarryOverService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadStockTakingsCarryOvers")]
		public async Task<IActionResult> ReadStockTakingsCarryOvers(DataSourceLoadOptions loadOptions, bool isOpenBalance)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_stockTakingCarryOverHeaderService.GetUserStockTakingCarryOverHeaders(isOpenBalance), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _stockTakingCarryOverHeaderService.GetUserStockTakingCarryOverHeaders(isOpenBalance);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
		}

		[HttpGet]
		[Route("GetStockTakingCarryOverCode")]
		public async Task<IActionResult> GetStockTakingCarryOverCode(int storeId, DateTime stockDate, bool isOpenBalance)
		{
			var data = await _stockTakingCarryOverHeaderService.GetStockTakingCarryOverCode(storeId, stockDate, isOpenBalance);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<StockTakingCarryOverDto>())
				{
					var stockTakingCarryOver = data.ConvertToType<StockTakingCarryOverDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, stockTakingCarryOver?.StockTakingCarryOverHeader?.StoreId ?? 0);
					return Ok(userCanLook ? stockTakingCarryOver : new StockTakingCarryOverDto());
				}
			}
			return Ok(new StockTakingCarryOverDto());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetStockTakingCarryOver(int id)
		{
			var stockTakingCarryOver = await _stockTakingCarryOverService.GetCarryOver(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, stockTakingCarryOver.StockTakingCarryOverHeader?.StoreId ?? 0);
			return Ok(userCanLook ? stockTakingCarryOver : new StockTakingCarryOverDto());
		}

		[HttpGet]
		[Route("GetStockTakingCarryOverDetails")]
		public async Task<IActionResult> GetStockTakingCarryOverDetails(int id)
		{
			var stockTaking = await _stockTakingCarryOverService.GetCarryOver(id);
			return Ok(stockTaking.StockTakingCarryOverDetails);
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] StockTakingCarryOverDto model, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(model.StockTakingCarryOverHeader.IsOpenBalance ? MenuCodeData.ApprovalStockTakingAsOpenBalance : MenuCodeData.ApprovalStockTakingAsCurrentBalance);
				if (hasApprove.HasApprove)
				{
					if (hasApprove.OnAdd && model.StockTakingCarryOverHeader!.StockTakingCarryOverHeaderId == 0)
					{
						response = await SendApproveRequest(model, null, requestId, 0, false);
					}
					else if (hasApprove.OnEdit && model.StockTakingCarryOverHeader!.StockTakingCarryOverHeaderId > 0)
					{
						var oldValue = await _stockTakingCarryOverService.GetCarryOver(model.StockTakingCarryOverHeader!.StockTakingCarryOverHeaderId);
						response = await SendApproveRequest(model, oldValue, requestId, model.StockTakingCarryOverHeader!.StockTakingCarryOverHeaderId, false);
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
		public async Task<ResponseDto> SaveStockTaking(StockTakingCarryOverDto model, bool hasApprove, bool approved, int? requestId)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _stockTakingCarryOverService.SaveStockTaking(model, hasApprove, approved, requestId);
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
		[Route("DeleteStockTakingCarryOver")]
		public async Task<IActionResult> DeleteStockTakingCarryOver(int stockTakingHeaderId, int requestId)
		{
			ResponseDto response;
			try
			{
				var model = await _stockTakingCarryOverService.GetCarryOver(stockTakingHeaderId);
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(model.StockTakingCarryOverHeader.IsOpenBalance ? MenuCodeData.ApprovalStockTakingAsOpenBalance : MenuCodeData.ApprovalStockTakingAsCurrentBalance);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					response = await SendApproveRequest(null, model, requestId, stockTakingHeaderId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _stockTakingCarryOverService.DeleteStockTaking(stockTakingHeaderId);
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
		public async Task<ResponseDto> SendApproveRequest([FromQuery] StockTakingCarryOverDto? newModel, [FromQuery] StockTakingCarryOverDto? oldModel, int requestId, int stockTakingHeaderId, bool isDelete)
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
						changes = _stockTakingCarryOverService.GetCarryOverRequestChanges(oldModel, newModel);
					}

					menuCode = oldModel.StockTakingCarryOverHeader.IsOpenBalance ? MenuCodeData.ApprovalStockTakingAsOpenBalance : MenuCodeData.ApprovalStockTakingAsCurrentBalance;
				}
			}
			else
			{
				if (newModel != null)
				{
					menuCode = newModel.StockTakingCarryOverHeader.IsOpenBalance ? MenuCodeData.ApprovalStockTakingAsOpenBalance : MenuCodeData.ApprovalStockTakingAsCurrentBalance;
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