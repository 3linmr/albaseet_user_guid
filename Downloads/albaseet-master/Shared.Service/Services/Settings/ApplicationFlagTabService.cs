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
	public class ApplicationFlagTabService : BaseService<ApplicationFlagTab>, IApplicationFlagTabService
	{
		public ApplicationFlagTabService(IRepository<ApplicationFlagTab> repository) : base(repository)
		{

		}
	}
}
