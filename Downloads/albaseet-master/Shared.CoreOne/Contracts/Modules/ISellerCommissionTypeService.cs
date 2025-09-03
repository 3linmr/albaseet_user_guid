using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface ISellerCommissionTypeService : IBaseService<SellerCommissionType>
	{
		IQueryable<SellerCommissionTypeDto> GetSellerCommissionTypes();
	}
}
