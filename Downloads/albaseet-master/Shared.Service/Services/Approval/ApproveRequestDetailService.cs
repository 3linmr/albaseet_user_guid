using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Identity;
using Shared.Helper.Logic;

namespace Shared.Service.Services.Approval
{
	public class ApproveRequestDetailService : BaseService<ApproveRequestDetail>, IApproveRequestDetailService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ApproveRequestDetailService(IRepository<ApproveRequestDetail> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<object?> GetRequestOldData(int requestId)
		{
			var request = await _repository.GetAll().Where(x=>x.ApproveRequestId == requestId).Select(x=> x.OldValue).FirstOrDefaultAsync();
			return request != null ? JsonConvert.DeserializeObject<object>(request) : new object();
		}

		public async Task<object?> GetRequestNewData(int requestId)
		{
			var request = await _repository.GetAll().Where(x => x.ApproveRequestId == requestId).Select(x => x.NewValue).FirstOrDefaultAsync();
			return request != null ? JsonConvert.DeserializeObject<object>(request) : new object();
		}

		public async Task<List<RequestChangesDto>?> GetRequestChanges(int requestId)
		{
			var request = await _repository.GetAll().Where(x => x.ApproveRequestId == requestId).Select(x => x.Changes).FirstOrDefaultAsync();
			return request != null ? JsonConvert.DeserializeObject<List<RequestChangesDto>>(request) : new List<RequestChangesDto>();
		}

		public async Task<bool> SaveApproveRequestDetail(ApproveRequestDetailDto requestDetail)
		{
			if (requestDetail.ApproveRequestDetailId == 0)
			{
				await CreateApproveRequestDetail(requestDetail);
			}
			else
			{
				await UpdateApproveRequestDetail(requestDetail);
			}
			return true;
		}

		public async Task<bool> CreateApproveRequestDetail(ApproveRequestDetailDto requestDetail)
		{
			var model = new ApproveRequestDetail()
			{
				ApproveRequestDetailId = requestDetail.ApproveRequestId,
				ApproveRequestId = requestDetail.ApproveRequestId,
				OldValue = requestDetail.OldValue,
				NewValue = requestDetail.NewValue,
				Changes = requestDetail.Changes,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};
			await _repository.Insert(model);
			await _repository.SaveChanges();
			return true;
		}
		public async Task<bool> UpdateApproveRequestDetail(ApproveRequestDetailDto requestDetail)
		{
			var requestDb = await _repository.GetAll().Where(x=>x.ApproveRequestId == requestDetail.ApproveRequestId).FirstOrDefaultAsync();
			if (requestDb != null)
			{
				requestDb.OldValue = requestDetail.OldValue;
				requestDb.NewValue = requestDetail.NewValue;
				requestDb.Changes = requestDetail.Changes;
				 _repository.Update(requestDb);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
		public async Task<bool> DeleteApproveRequestDetail(int requestId)
		{
			var data = await _repository.GetAll().Where(x => x.ApproveRequestId == requestId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
	}
}
