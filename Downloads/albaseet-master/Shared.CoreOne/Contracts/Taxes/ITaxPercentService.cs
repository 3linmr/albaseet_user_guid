using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Taxes
{
	public interface ITaxPercentService : IBaseService<TaxPercent>
	{
		IQueryable<TaxPercentDto> GetAllTaxPercents();
		IQueryable<TaxPercentDto> GetTaxPercentsByTaxId(int taxId);
		Task<TaxPercentDto?> GetTaxPercentById(int id);
		Task<TaxPercentDto> GetCurrentTaxPercent(int taxId,DateTime currentDate);
		IQueryable<CompanyVatPercentDto> GetAllCompanyVatPercents(DateTime currentDate);
		IQueryable<TaxPercentDto> GetAllCurrentTaxPercents(DateTime currentDate);
        Task<decimal> GetCurrentTaxPercentValue(int taxId,DateTime currentDate);
		Task<decimal> GetVatTaxByCompanyId(int companyId, DateTime documentDate);
		Task<decimal> GetVatTaxByStoreId(int storeId, DateTime documentDate);
		Task<TaxDto> GetVatByCompanyId(int companyId, DateTime currentDate);
		Task<TaxDto> GetVatByStoreId(int storeId, DateTime currentDate);
		Task<ResponseDto> SaveTaxPercents(List<TaxPercentDto> percents,int taxId);
		Task<ResponseDto> DeleteTaxByTaxId(int id);
	}
}
