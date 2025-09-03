using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IStockInReturnDetailTaxService: IBaseService<StockInReturnDetailTax>
    {
        IQueryable<StockInReturnDetailTaxDto> GetStockInReturnDetailTaxes(int stockInReturnHeaderId);
        Task<bool> SaveStockInReturnDetailTaxes(int stockInReturnHeaderId, List<StockInReturnDetailTaxDto> stockInReturnDetailTaxes);
        Task<bool> DeleteStockInReturnDetailTaxes(int stockInReturnHeaderId);
    }
}
