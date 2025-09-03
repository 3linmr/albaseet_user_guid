using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Admin;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;

namespace Shared.CoreOne.Contracts.Admin
{
	public interface IApplicationDataService
	{
		Task<ApplicationValidationDataDto> GetApplicationValidationData();
		Task<bool> CreateFirstCompany(CompanyIdentityDto model);
		string GetStructureDatabase();
		SshOptionsDto GetSshOptions();
	}
}
