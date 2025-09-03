using Compound.CoreOne.Contracts.Approval;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Extensions;

namespace Compound.Service.Services.Approval
{
	public class RedirectDeclinedRequestService : IRedirectDeclinedRequestService
	{
		private readonly IInternalTransferReceiveService _internalTransferReceiveService;
		private readonly IStringLocalizer<RedirectApproveRequestService> _localizer;

		public RedirectDeclinedRequestService(IStringLocalizer<RedirectApproveRequestService> localizer, IInternalTransferReceiveService internalTransferReceiveService)
		{
			_internalTransferReceiveService = internalTransferReceiveService;
			_localizer = localizer;
		}
		public async Task<ResponseDto> RedirectDeclinedRequest(ApproveResponseDto request)
		{
			return request.MenuCode switch
			{
				MenuCodeData.InternalReceiveItems => await HandleInternalTransferReceive(request),
				_ => new ResponseDto() { Success = request.Success, Message = request.Message }
			};
		}

		
		public async Task<ResponseDto> HandleInternalTransferReceive(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<InternalTransferReceiveDto>())
				{
					var sendModel = data.ConvertToType<InternalTransferReceiveDto>();
					if (sendModel != null)
					{
						sendModel.InternalTransferReceiveHeader!.IsReturned = true;
						sendModel.InternalTransferReceiveHeader!.ReturnReason = _localizer["InternalTransferReturnReason"];
						return await _internalTransferReceiveService.SaveInternalTransferReceive(sendModel, true, true, request.RequestId);
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}
	}
}
