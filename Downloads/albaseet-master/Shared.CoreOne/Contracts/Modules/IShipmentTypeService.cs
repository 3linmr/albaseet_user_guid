using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IShipmentTypeService : IBaseService<ShipmentType>
	{
		IQueryable<ShipmentTypeDto> GetShipmentTypes();
		Task<ShipmentTypeDto?> GetShipmentTypeById(int id);
		Task<ResponseDto> SaveShipmentType(ShipmentTypeDto shipmentType);
		Task<ResponseDto> DeleteShipmentType(int id);
	}
}
