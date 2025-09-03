using Shared.CoreOne.Contracts.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Settings
{
    public class ApplicationFlagDetailService : BaseService<ApplicationFlagDetail>, IApplicationFlagDetailService
	{
		private readonly IStringLocalizer<ApplicationFlagDetailService> _localizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ApplicationFlagDetailService(IRepository<ApplicationFlagDetail> repository ,IStringLocalizer<ApplicationFlagDetailService> localizer,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_localizer = localizer;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<string?> GetApplicationFlagDetailValue(int applicationFlagDetailId)
		{
			return await _repository.GetAll().Where(x => x.ApplicationFlagDetailId == applicationFlagDetailId)
				.Select(x => x.FlagValue).FirstOrDefaultAsync();
		}

		public async Task<ResponseDto> SaveApplicationSetting(SaveApplicationSettingDto model)
		{
			if (model.ApplicationFlagDetailId != 0 && !string.IsNullOrEmpty(model.FlagValue) && !string.IsNullOrWhiteSpace(model.FlagValue))
			{
				var modelDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ApplicationFlagDetailId == model.ApplicationFlagDetailId);
				if (modelDb != null)
				{
					modelDb.FlagValue = model.FlagValue;
					modelDb.ModifiedAt = DateHelper.GetDateTimeNow();
					modelDb.UserNameModified = await _httpContextAccessor!.GetUserName();
					modelDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();
					_repository.Update(modelDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = model.ApplicationFlagDetailId, Message = _localizer["SaveSuccess", (model.FlagName ) ?? ""], Success = true };
				}
			}
			return new ResponseDto() { Id = model.ApplicationFlagDetailId, Message = "Error", Success = false };
		}
	}
}
