using DevExtreme.AspNet.Data;
using Purchases.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ProductRequestPricesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
        private readonly IProductRequestPriceHeaderService _productRequestPriceHeaderService;
        private readonly IProductRequestPriceService _productRequestPriceService;
        private readonly IProductRequestPriceDetailService _productRequestPriceDetailService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApprovalSystemService _approvalSystemService;
        private readonly IGenericMessageService _genericMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductRequestPricesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IProductRequestPriceHeaderService productRequestPriceHeaderService, IProductRequestPriceService productRequestPriceService, IProductRequestPriceDetailService productRequestPriceDetailService, IGenericMessageService genericMessageService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exLocalizer = exLocalizer;
            _productRequestPriceHeaderService = productRequestPriceHeaderService;
            _productRequestPriceService = productRequestPriceService;
            _productRequestPriceDetailService = productRequestPriceDetailService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approvalSystemService = approvalSystemService;
            _genericMessageService = genericMessageService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadProductRequestPrices")]
        public async Task<IActionResult> ReadProductRequestPrices(DataSourceLoadOptions loadOptions)
        {
	        try
	        {
		        var data = await DataSourceLoader.LoadAsync(_productRequestPriceHeaderService.GetUserProductRequestPriceHeaders(), loadOptions);
		        return Ok(data);
	        }
	        catch
	        {
		        var model = _productRequestPriceHeaderService.GetUserProductRequestPriceHeaders();
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
        }

        [HttpGet]
        [Route("ReadProductRequestPriceHeadersByStoreId")]
		public IActionResult ReadProductRequestPriceHeadersByStoreId(DataSourceLoadOptions loadOptions,int storeId, int? supplierId, int productRequestPriceHeaderId)
        {
            try
            {
                var data = DataSourceLoader.Load(_productRequestPriceHeaderService.GetProductRequestPriceHeadersByStoreId(storeId, supplierId, productRequestPriceHeaderId), loadOptions);
                return Ok(data);
            }catch
            {
                var model = _productRequestPriceHeaderService.GetProductRequestPriceHeadersByStoreId(storeId, supplierId, productRequestPriceHeaderId);
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("GetProductRequestPriceCode")]
        public async Task<IActionResult> GetProductRequestPriceCode(int storeId, DateTime documentDate)
        {
            var data = await _productRequestPriceHeaderService.GetProductRequestPriceCode(storeId, documentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetProductRequestPriceFromProductRequest")]
        public async Task<IActionResult> GetProductRequestPriceFromProductRequest(int productRequestHeaderId, DateTime? currentDate = null)
        {
            var data = await _productRequestPriceService.GetProductRequestPriceFromProductRequest(productRequestHeaderId, currentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetRequestData")]
        public async Task<IActionResult> GetRequestData(int requestId)
        {
            var data = await _handleApprovalRequestService.GetRequestData(requestId);
            if (data != null)
            {
                if (data.CanBeConverted<ProductRequestPriceDto>())
                {
					var productRequestPrice = data.ConvertToType<ProductRequestPriceDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(0, productRequestPrice?.ProductRequestPriceHeader?.StoreId ?? 0);
                    return Ok(userCanLook ? productRequestPrice : new ProductRequestPriceDto());
                }
            }
            return Ok(new ProductRequestPriceDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductRequestPrice(int id)
        {
            var productRequestPrice = await _productRequestPriceService.GetProductRequestPrice(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, productRequestPrice.ProductRequestPriceHeader?.StoreId ?? 0);
            return Ok(userCanLook ? productRequestPrice : new ProductRequestPriceDto());
        }

        [HttpGet]
        [Route("GetProductRequestPriceDetail")]
        public async Task<IActionResult> GetProductRequestPriceDetail(int id)
        {
            var productRequestPriceDetail = await _productRequestPriceDetailService.GetProductRequestPriceDetails(id);
            return Ok(productRequestPriceDetail);
        }

        [HttpGet]
        [Route("GetProductRequestPriceHeader")]
        public async Task<IActionResult> GetProductRequestPriceHeader(int id)
        {
            var productRequestPriceHeader = await _productRequestPriceHeaderService.GetProductRequestPriceHeaderById(id);
            return Ok(productRequestPriceHeader);
        }

        [HttpPost]
        [Route("UpdateIsClosed")]
        public async Task<IActionResult> UpdateIsClosed(int productRequestPriceHeaderId, bool isClosed)
        {
            return Ok(await _productRequestPriceHeaderService.UpdateClosed(productRequestPriceHeaderId, isClosed));
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save([FromBody] ProductRequestPriceDto model, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                if (model.ProductRequestPriceHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ProductRequestPrice);
                    if (hasApprove.HasApprove)
                    {
                        if (hasApprove.OnAdd && model.ProductRequestPriceHeader!.ProductRequestPriceHeaderId == 0)
                        {
                            response = await SendApproveRequest(model, null, requestId, 0, false);
                        }
                        else if (hasApprove.OnEdit && model.ProductRequestPriceHeader!.ProductRequestPriceHeaderId > 0)
                        {
                            var oldValue = await _productRequestPriceService.GetProductRequestPrice(model.ProductRequestPriceHeader!.ProductRequestPriceHeaderId);
                            response = await SendApproveRequest(model, oldValue, requestId, model.ProductRequestPriceHeader!.ProductRequestPriceHeaderId, false);
                        }
                        else
                        {
                            response = await SaveProductRequestPrice(model, hasApprove.HasApprove, false, requestId);
                        }
                    }
                    else
                    {
                        response = await SaveProductRequestPrice(model, hasApprove.HasApprove, false, requestId);
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
        [Route("SaveProductRequestPrice")]
        public async Task<ResponseDto> SaveProductRequestPrice(ProductRequestPriceDto model, bool hasApprove, bool approved, int? requestId)
        {
            await _databaseTransaction.BeginTransaction();
            var response = await _productRequestPriceService.SaveProductRequestPrice(model, hasApprove, approved, requestId);
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
        [Route("DeleteProductRequestPrice")]
        public async Task<IActionResult> DeleteProductRequestPrice(int productRequestPriceHeaderId, int requestId)
        {
            var response = new ResponseDto();
            try
            {
                var model = await _productRequestPriceService.GetProductRequestPrice(productRequestPriceHeaderId);
                if (model.ProductRequestPriceHeader != null)
                {
                    var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.ProductRequestPrice);
                    if (hasApprove.HasApprove && hasApprove.OnDelete)
                    {
                        response = await SendApproveRequest(null, model, requestId, productRequestPriceHeaderId, true);
                    }
                    else
                    {
                        await _databaseTransaction.BeginTransaction();
                        response = await _productRequestPriceService.DeleteProductRequestPrice(productRequestPriceHeaderId);
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
                    response.Id = productRequestPriceHeaderId;
                    response.Message = await _genericMessageService.GetMessage(MenuCodeData.ProductRequestPrice, GenericMessageData.NotFound);
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
        public async Task<ResponseDto> SendApproveRequest([FromQuery] ProductRequestPriceDto? newModel, [FromQuery] ProductRequestPriceDto? oldModel, int requestId, int productRequestPriceHeaderId, bool isDelete)
        {
            var changes = new List<RequestChangesDto>();
            var menuCode = MenuCodeData.ProductRequestPrice;
            var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (productRequestPriceHeaderId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
            if (requestTypeId == ApproveRequestTypeData.Edit && oldModel != null)
            {
                changes = _productRequestPriceService.GetProductRequestPriceRequestChanges(oldModel, newModel!);
            }

            var request = new NewApproveRequestDto()
            {
                RequestId = requestId,
                MenuCode = (short)menuCode,
                ApproveRequestTypeId = requestTypeId,
                ReferenceId = productRequestPriceHeaderId,
                ReferenceCode = productRequestPriceHeaderId.ToString(),
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
