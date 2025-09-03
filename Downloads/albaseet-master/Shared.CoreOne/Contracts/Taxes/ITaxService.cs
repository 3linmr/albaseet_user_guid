using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Taxes
{
	public interface ITaxService : IBaseService<Tax>
	{
		IQueryable<TaxDto> GetAllTaxes();
		IQueryable<TaxDto> GetUserTaxes();
        IQueryable<TaxDto> GetCompanyTaxes(int companyId);
		IQueryable<TaxDto> GetAllStoreTaxes();
		IQueryable<TaxDto> GetStoreTaxes(int storeId);
		IQueryable<TaxDto> GetAllStoreVatTaxes();
		IQueryable<TaxDropDownDto> GetAllTaxesDropDown();
		IQueryable<TaxDropDownDto> GeVatTaxDropDown();
		IQueryable<TaxDropDownDto> GetCompanyTaxesDropDown(int companyId);
		IQueryable<TaxDropDownDto> GetCompanyOtherTaxesDropDown(int companyId);
		IQueryable<TaxDropDownDto> GetStoreTaxesDropDown(int storeId);
		IQueryable<TaxDropDownDto> GetStoreOtherTaxesDropDown(int storeId);
		IQueryable<TaxDto> GetAllTaxesByTypeId(int taxTypeId);
		Task<TaxDto?> GetTaxById(int id);
		Task<ResponseDto> IsVatTaxExist();
		Task<ResponseDto> SaveTax(TaxDto tax);
		Task<ResponseDto> DeleteTax(int id);
	}
}
