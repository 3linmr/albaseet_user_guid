using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Approval
{
	public interface IRedirectDeclinedRequestService
	{
		Task<ResponseDto> RedirectDeclinedRequest(ApproveResponseDto request);
	}
}
