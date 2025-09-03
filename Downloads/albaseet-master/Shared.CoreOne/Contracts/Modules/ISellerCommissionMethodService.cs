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
	public interface ISellerCommissionMethodService : IBaseService<SellerCommissionMethod>
	{
		IQueryable<SellerCommissionMethodDto> GetAllSellerCommissionMethods();
		IQueryable<SellerCommissionMethodDto> GetCompanySellerCommissionMethods();
		IQueryable<SellerCommissionMethodDropDownDto> GetAllSellerCommissionMethodsDropDown();
		IQueryable<SellerCommissionMethodDropDownDto> GetActiveSellerCommissionMethodsDropDown();
		Task<SellerCommissionMethodDto?> GetSellerCommissionMethodById(int id);
		Task<ResponseDto> SaveSellerCommissionMethod(SellerCommissionMethodDto sellerCommissionMethod);
		Task<ResponseDto> DeleteSellerCommissionMethod(int id);
	}
}
