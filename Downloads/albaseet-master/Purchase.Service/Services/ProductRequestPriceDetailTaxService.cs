using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Identity;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Helper.Logic;
using Shared.Service.Logic.Calculation;

namespace Purchases.Service.Services
{
	public class ProductRequestPriceDetailTaxService : BaseService<ProductRequestPriceDetailTax>, IProductRequestPriceDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRequestPriceDetailService _productRequestPriceDetailService;

		public ProductRequestPriceDetailTaxService(IRepository<ProductRequestPriceDetailTax> repository,IHttpContextAccessor httpContextAccessor, IProductRequestPriceDetailService productRequestPriceDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _productRequestPriceDetailService = productRequestPriceDetailService;
        }

		public IQueryable<ProductRequestPriceDetailTaxDto> GetProductRequestPriceDetailTaxes(int productRequestPriceHeaderId)
		{
			var data =
			        (
                    from productRequestPriceDetail in _productRequestPriceDetailService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId)
                    from productRequestPriceDetailTax in _repository.GetAll().Where(x => x.ProductRequestPriceDetailId == productRequestPriceDetail.ProductRequestPriceDetailId)
                    select new ProductRequestPriceDetailTaxDto
				    {
                         ProductRequestPriceDetailTaxId = productRequestPriceDetailTax.ProductRequestPriceDetailTaxId,
                         ProductRequestPriceDetailId = productRequestPriceDetailTax.ProductRequestPriceDetailId,
                         TaxId = productRequestPriceDetailTax.TaxId,
                         DebitAccountId = productRequestPriceDetailTax.DebitAccountId,
                         TaxAfterVatInclusive = productRequestPriceDetailTax.TaxAfterVatInclusive,
                         TaxPercent = productRequestPriceDetailTax.TaxPercent,
                         TaxValue = productRequestPriceDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveProductRequestPriceDetailTaxes(int productRequestPriceHeaderId, List<ProductRequestPriceDetailTaxDto> productRequestPriceDetailTaxes)
        {
            await DeleteProductRequestPriceDetailTax(productRequestPriceDetailTaxes, productRequestPriceHeaderId);
            if (productRequestPriceDetailTaxes.Any())
            {
                await AddProductRequestPriceDetailTax(productRequestPriceDetailTaxes);
                await EditProductRequestPriceDetailTax(productRequestPriceDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProductRequestPriceDetailTaxes(int productRequestPriceHeaderId)
        {
            var data = await (
                       from productRequestPriceDetail in _productRequestPriceDetailService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId)
                       from productRequestPriceDetailTax in _repository.GetAll().Where(x => x.ProductRequestPriceDetailId == productRequestPriceDetail.ProductRequestPriceDetailId)
                       select productRequestPriceDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteProductRequestPriceDetailTax(List<ProductRequestPriceDetailTaxDto> details, int headerId)
        {
            var current = await (
                from productRequestPriceDetail in _productRequestPriceDetailService.GetAll().Where(x => x.ProductRequestPriceHeaderId == headerId)
                from productRequestPriceDetailTax in _repository.GetAll().Where(x => x.ProductRequestPriceDetailId == productRequestPriceDetail.ProductRequestPriceDetailId)
                select productRequestPriceDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.ProductRequestPriceDetailTaxId != p.ProductRequestPriceDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddProductRequestPriceDetailTax(List<ProductRequestPriceDetailTaxDto> details)
        {
            var current = details.Where(x => x.ProductRequestPriceDetailTaxId <= 0).ToList();
            var modelList = new List<ProductRequestPriceDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ProductRequestPriceDetailTax()
                {
                    ProductRequestPriceDetailTaxId = newId,
                    ProductRequestPriceDetailId = detail.ProductRequestPriceDetailId,
                    TaxId = detail.TaxId,
                    DebitAccountId = detail.DebitAccountId,
                    TaxAfterVatInclusive = detail.TaxAfterVatInclusive,
                    TaxPercent = detail.TaxPercent,
                    TaxValue = detail.TaxValue,


                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                modelList.Add(model);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> EditProductRequestPriceDetailTax(List<ProductRequestPriceDetailTaxDto> productRequestPriceDetailTaxes)
        {            
            var current = productRequestPriceDetailTaxes.Where(x => x.ProductRequestPriceDetailTaxId > 0).ToList();
            var modelList = new List<ProductRequestPriceDetailTax>();
            foreach (var detail in current)
            {
                var model = new ProductRequestPriceDetailTax()
                {
                    ProductRequestPriceDetailTaxId = detail.ProductRequestPriceDetailTaxId,
                    ProductRequestPriceDetailId = detail.ProductRequestPriceDetailId,
                    TaxId = detail.TaxId,
                    DebitAccountId = detail.DebitAccountId,
                    TaxAfterVatInclusive = detail.TaxAfterVatInclusive,
                    TaxPercent = detail.TaxPercent,
                    TaxValue = detail.TaxValue,

                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };


                modelList.Add(model);
            }

            if (modelList.Any())
            {
                _repository.UpdateRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;

        }

        private async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ProductRequestPriceDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
