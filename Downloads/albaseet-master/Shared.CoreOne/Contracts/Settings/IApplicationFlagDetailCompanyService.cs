using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Settings
{
	public interface IApplicationFlagDetailCompanyService:IBaseService<ApplicationFlagDetailCompany>
	{
		Task<string?> GetApplicationFlagDetailCompanyValue(int companyId,int applicationFlagDetailId);
		Task<string?> GetApplicationFlagDetailCompanyValueByStoreId(int storeId,int applicationFlagDetailId);
		Task<ResponseDto> SaveApplicationCompanySetting(SaveApplicationSettingDto model);
	}
}
