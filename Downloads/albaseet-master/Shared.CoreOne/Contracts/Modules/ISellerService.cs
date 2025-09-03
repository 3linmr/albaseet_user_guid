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
	public interface ISellerService :IBaseService<Seller>
	{
		IQueryable<SellerDto> GetAllSellers();
		IQueryable<SellerDto> GetUserSellers();
        IQueryable<SellerDropDownDto> GetSellersDropDownByCompanyId(int companyId);
		IQueryable<SellerDropDownDto> GetSellersDropDownByStoreId(int storeId);
		Task<SellerCommissionMethodChangeDto?> GetSellerCommissionMethodChangeData(int commissionMethodId);
		Task<SellerDto?> GetSellerById(int id);
		Task<List<SellerAutoCompleteDto>> GetSellersAutoComplete(string term);
		Task<List<SellerAutoCompleteDto>> GetSellersAutoCompleteByStoreIds(string term, List<int> storeIds);
		Task<ResponseDto> SaveSeller(SellerDto seller);
		Task<ResponseDto> DeleteSeller(int id);
	}
}
