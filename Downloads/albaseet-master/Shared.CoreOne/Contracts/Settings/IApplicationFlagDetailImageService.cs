using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Archive;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Settings
{
	public interface IApplicationFlagDetailImageService : IBaseService<ApplicationFlagDetailImage>
	{
		IQueryable<ApplicationFlagDetailImageDto?> GetApplicationFlagImages();
		Task<ApplicationFlagDetailImageDto?> GetApplicationFlagImage(int applicationFlagDetailImageId);
		Task<bool> SaveApplicationFlagImage(ApplicationFlagDetailImageDto model);
	}
}
