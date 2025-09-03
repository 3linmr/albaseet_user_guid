using DevExtreme.AspNet.Data;
using Purchases.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Contracts;
using Purchases.Service.Services;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;

namespace Purchases.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductRequestController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IProductRequestHeaderService _productRequestHeaderService;
        private readonly IProductRequestService _productRequestService;
        private readonly IProductRequestDetailService _productRequestDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductRequestController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IProductRequestHeaderService productRequestHeaderService, IProductRequestService productRequestService, IProductRequestDetailService productRequestDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _productRequestHeaderService = productRequestHeaderService;
            _productRequestService = productRequestService;
            _productRequestDetailService = productRequestDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadProductRequests")]
        public async Task<IActionResult> ReadProductRequests(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_productRequestHeaderService.GetUserProductRequestHeaders(), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _productRequestHeaderService.GetUserProductRequestHeaders();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("ReadProductRequestsByStoreId")]
        public async Task<IActionResult> ReadProductRequestsByStoreId(DataSourceLoadOptions loadOptions, int storeId, int productRequestHeaderId)
        {
            try
            {
                var data = await DataSourceLoader.LoadAsync(_productRequestHeaderService.GetProductRequestHeadersByStoreId(storeId, productRequestHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _productRequestHeaderService.GetProductRequestHeadersByStoreId(storeId, productRequestHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetProductRequestCode")]
        public async Task<IActionResult> GetProductRequestCode(int storeId, DateTime documentDate)
        {
            var data = await _productRequestHeaderService.GetProductRequestCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ProductRequestDto>())
                {
					var productRequest = data.ConvertToType<ProductRequestDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, productRequest?.ProductRequestHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? productRequest : new ProductRequestDto());
                }
            }
            return Ok(new ProductRequestDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductRequest(int id)
        {
            var productRequest = await _productRequestService.GetProductRequest(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, productRequest.ProductRequestHeader?.StoreId ?? 0);
            return Ok(userCanLook ? productRequest : new ProductRequestDto());
        }

        [HttpGet]
        [Route("GetProductRequestDetail")]
        public async Task<IActionResult> GetProductRequestDetail(int id)
        {
            var productRequestDetail = await _productRequestDetailService.GetProductRequestDetails(id);
            return Ok(productRequestDetail);
        }

        [HttpGet]
        [Route("GetProductRequestHeader")]
        public async Task<IActionResult> GetProductRequestHeader(int id)
        {
            var productRequestHeader = await _productRequestHeaderService.GetProductRequestHeaderById(id);
            return Ok(productRequestHeader);
        }

        [HttpPost]
        [Route("UpdateIsClosed")]
        public async Task<IActionResult> UpdateIsClosed(int productRequestHeaderId, bool isClosed)
        {
            return Ok(await _productRequestHeaderService.UpdateClosed(productRequestHeaderId, isClosed));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ProductRequestDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ProductRequestHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ProductRequest);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ProductRequestHeader!.ProductRequestHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ProductRequestHeader!.ProductRequestHeaderId > 0)
                        {
                            var oldValue = await _productRequestService.GetProductRequest(model.ProductRequestHeader!.ProductRequestHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ProductRequestHeader!.ProductRequestHeaderId, false);
                        }
                        else
                        {
                            response = await SaveProductRequest(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveProductRequest(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveProductRequest")]
        public async Task<ResponseDto> SaveProductRequest(ProductRequestDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _productRequestService.SaveProductRequest(model, hasApprove, approved, requestId);
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
        [Route("DeleteProductRequest")]
        public async Task<IActionResult> DeleteProductRequest(int productRequestHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _productRequestService.GetProductRequest(productRequestHeaderId);
                if (model.ProductRequestHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ProductRequest);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, productRequestHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _productRequestService.DeleteProductRequest(productRequestHeaderId);
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
                    response.Id = productRequestHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequest, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] ProductRequestDto? newModel, [FromQuery] ProductRequestDto? oldModel, int requestId, int productRequestHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ProductRequest;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (productRequestHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _productRequestService.GetProductRequestRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = productRequestHeaderId,
                ReferenceCode = productRequestHeaderId.ToString(),
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
