using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Settings;

namespace Shared.Service.Services.Settings
{
    public class ApplicationFlagTypeService : BaseService<ApplicationFlagType>, IApplicationFlagTypeService
	{
		public ApplicationFlagTypeService(IRepository<ApplicationFlagType> repository) : base(repository)
		{

		}
	}
}
