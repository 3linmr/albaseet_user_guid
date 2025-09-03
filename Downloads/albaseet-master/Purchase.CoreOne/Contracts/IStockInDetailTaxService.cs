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
    public interface IStockInDetailTaxService: IBaseService<StockInDetailTax>
    {
        IQueryable<StockInDetailTaxDto> GetStockInDetailTaxes(int stockInHeaderId);
        Task<bool> SaveStockInDetailTaxes(int stockInHeaderId, List<StockInDetailTaxDto> stockInDetailTaxes);
        Task<bool> DeleteStockInDetailTaxes(int stockInHeaderId);
    }
}
