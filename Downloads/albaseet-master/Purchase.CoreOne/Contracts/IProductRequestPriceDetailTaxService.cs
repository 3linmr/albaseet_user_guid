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
    public interface IProductRequestPriceDetailTaxService: IBaseService<ProductRequestPriceDetailTax>
    {
        IQueryable<ProductRequestPriceDetailTaxDto> GetProductRequestPriceDetailTaxes(int productRequestPriceHeaderId);
        Task<bool> SaveProductRequestPriceDetailTaxes(int productRequestPriceHeaderId, List<ProductRequestPriceDetailTaxDto> productRequestPriceDetailTaxes);
        Task<bool> DeleteProductRequestPriceDetailTaxes(int productRequestPriceHeaderId);
    }
}
