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
    public interface IStockOutReturnDetailTaxService: IBaseService<StockOutReturnDetailTax>
    {
        IQueryable<StockOutReturnDetailTaxDto> GetStockOutReturnDetailTaxes(int stockOutReturnHeaderId);
        Task<bool> SaveStockOutReturnDetailTaxes(int stockOutReturnHeaderId, List<StockOutReturnDetailTaxDto> stockOutReturnDetailTaxes);
        Task<bool> DeleteStockOutReturnDetailTaxes(int stockOutReturnHeaderId);
    }
}
