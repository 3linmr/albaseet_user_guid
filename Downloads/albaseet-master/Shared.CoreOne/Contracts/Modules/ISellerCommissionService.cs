using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface ISellerCommissionService : IBaseService<SellerCommission>
	{ 
		IQueryable<SellerCommissionDto> GetAllSellerCommissions();
		IQueryable<SellerCommissionDto> GetSellerCommissionsByType(int sellerCommissionMethodId);
		Task<List<SellerCommissionDto>> GetSellerCommissionsByCommissionMethodId(int sellerCommissionMethodId);
		Task<SellerCommissionDto?> GetSellerCommissionById(int commissionId);
		Task<ResponseDto> SaveSellerCommission(SellerCommissionDto sellerCommission);
		Task<ResponseDto> DeleteSellerCommission(int id);
	}
}
