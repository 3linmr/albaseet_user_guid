using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutDetailTaxService: IBaseService<StockOutDetailTax>
    {
        IQueryable<StockOutDetailTaxDto> GetStockOutDetailTaxes(int stockOutHeaderId);
        Task<bool> SaveStockOutDetailTaxes(int stockOutHeaderId, List<StockOutDetailTaxDto> stockOutDetailTaxes);
        Task<bool> DeleteStockOutDetailTaxes(int stockOutHeaderId);
    }
}
