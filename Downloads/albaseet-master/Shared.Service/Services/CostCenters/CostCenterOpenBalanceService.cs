using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Models.Domain.CostCenters;

namespace Shared.Service.Services.CostCenters
{
	public class CostCenterOpenBalanceService : BaseService<CostCenterOpenBalance>, ICostCenterOpenBalanceService
	{
		public CostCenterOpenBalanceService(IRepository<CostCenterOpenBalance> repository) : base(repository)
		{

		}
	}
}
