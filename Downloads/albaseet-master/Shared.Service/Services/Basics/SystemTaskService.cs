using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;

namespace Shared.Service.Services.Basics
{
    public class SystemTaskService : BaseService<SystemTask>, ISystemTaskService
    {
        private readonly IStringLocalizer<SystemTask> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemTaskService(IRepository<SystemTask> repository,IStringLocalizer<SystemTask> localizer,IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CanImportAll()
        {
	        var result = await _repository.GetAll().AnyAsync(x => x.IsCompleted);
	        return !result;
        }

        public IQueryable<SystemTaskDto> GetAllSystemTasks()
        {
            return _repository.GetAll().Select(x => new SystemTaskDto()
            {
                TaskId = x.TaskId,
                TaskNameAr = x.TaskNameAr,
                TaskNameEn = x.TaskNameEn,
                IsCompleted = x.IsCompleted,
                Loading = false
            });
        }

        public SystemTaskDto GetSystemTask(int id)
        {
            return GetAllSystemTasks().FirstOrDefault(x => x.TaskId == id) ?? new SystemTaskDto();
        }

        public async Task<ResponseDto> UpdateSystemTaskToBeCompleted(int taskId)
        {
            var taskDb = _repository.GetAll().AsQueryable().FirstOrDefault(x => x.TaskId == taskId);
            if (taskDb != null)
            {
                taskDb.IsCompleted = true;
                taskDb.CreatedAt = DateHelper.GetDateTimeNow();
                taskDb.UserNameCreated = await _httpContextAccessor!.GetUserName();
                taskDb.IpAddressCreated = _httpContextAccessor?.GetIpAddress();
                _repository.Update(taskDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = taskId, Success = true, Message = _localizer["UpdateSystemTaskSuccess"] };
            }
            else
            {
                return new ResponseDto() { Id = taskId, Success = false, Message = _localizer["UpdateSystemTaskNotFound"] };
            }
        }

        public async Task<ResponseDto> ResetSystemTasks()
        {
	        var data = _repository.GetAll().ToList();
			foreach (var item in data)
			{
				item.IsCompleted = false;
				_repository.Update(item);
			}
			await _repository.SaveChanges();
			return new ResponseDto() { Id = 0, Success = true, Message = _localizer["UpdateSystemTaskSuccess"] };
		}
	}
}
