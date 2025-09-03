using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Basics
{
    public interface ISystemTaskService : IBaseService<SystemTask>
    {
	    Task<bool> CanImportAll();
        IQueryable<SystemTaskDto> GetAllSystemTasks();
        SystemTaskDto GetSystemTask(int id);
        Task<ResponseDto> UpdateSystemTaskToBeCompleted(int taskId);
        Task<ResponseDto> ResetSystemTasks();
    }
}
