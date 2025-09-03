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
	public class SupplierQuotationDetailTaxService : BaseService<SupplierQuotationDetailTax>, ISupplierQuotationDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISupplierQuotationDetailService _supplierQuotationDetailService;

		public SupplierQuotationDetailTaxService(IRepository<SupplierQuotationDetailTax> repository,IHttpContextAccessor httpContextAccessor, ISupplierQuotationDetailService supplierQuotationDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _supplierQuotationDetailService = supplierQuotationDetailService;
        }

		public IQueryable<SupplierQuotationDetailTaxDto> GetSupplierQuotationDetailTaxes(int supplierQuotationHeaderId)
		{
			var data =
			        (
                    from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId)
                    from supplierQuotationDetailTax in _repository.GetAll().Where(x => x.SupplierQuotationDetailId == supplierQuotationDetail.SupplierQuotationDetailId)
                    select new SupplierQuotationDetailTaxDto
				    {
                         SupplierQuotationDetailTaxId = supplierQuotationDetailTax.SupplierQuotationDetailTaxId,
                         SupplierQuotationDetailId = supplierQuotationDetailTax.SupplierQuotationDetailId,
                         TaxId = supplierQuotationDetailTax.TaxId,
                         DebitAccountId = supplierQuotationDetailTax.DebitAccountId,
                         TaxAfterVatInclusive = supplierQuotationDetailTax.TaxAfterVatInclusive,
                         TaxPercent = supplierQuotationDetailTax.TaxPercent,
                         TaxValue = supplierQuotationDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveSupplierQuotationDetailTaxes(int supplierQuotationHeaderId, List<SupplierQuotationDetailTaxDto> supplierQuotationDetailTaxes)
        {
            await DeleteSupplierQuotationDetailTax(supplierQuotationDetailTaxes, supplierQuotationHeaderId);
            if (supplierQuotationDetailTaxes.Any())
            {
                await AddSupplierQuotationDetailTax(supplierQuotationDetailTaxes);
                await EditSupplierQuotationDetailTax(supplierQuotationDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSupplierQuotationDetailTaxes(int supplierQuotationHeaderId)
        {
            var data = await (
                       from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId)
                       from supplierQuotationDetailTax in _repository.GetAll().Where(x => x.SupplierQuotationDetailId == supplierQuotationDetail.SupplierQuotationDetailId)
                       select supplierQuotationDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteSupplierQuotationDetailTax(List<SupplierQuotationDetailTaxDto> details, int headerId)
        {
            var current = await (
                from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == headerId)
                from supplierQuotationDetailTax in _repository.GetAll().Where(x => x.SupplierQuotationDetailId == supplierQuotationDetail.SupplierQuotationDetailId)
                select supplierQuotationDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.SupplierQuotationDetailTaxId != p.SupplierQuotationDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddSupplierQuotationDetailTax(List<SupplierQuotationDetailTaxDto> details)
        {
            var current = details.Where(x => x.SupplierQuotationDetailTaxId <= 0).ToList();
            var modelList = new List<SupplierQuotationDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new SupplierQuotationDetailTax()
                {
                    SupplierQuotationDetailTaxId = newId,
                    SupplierQuotationDetailId = detail.SupplierQuotationDetailId,
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

        private async Task<bool> EditSupplierQuotationDetailTax(List<SupplierQuotationDetailTaxDto> supplierQuotationDetailTaxes)
        {            
            var current = supplierQuotationDetailTaxes.Where(x => x.SupplierQuotationDetailTaxId > 0).ToList();
            var modelList = new List<SupplierQuotationDetailTax>();
            foreach (var detail in current)
            {
                var model = new SupplierQuotationDetailTax()
                {
                    SupplierQuotationDetailTaxId = detail.SupplierQuotationDetailTaxId,
                    SupplierQuotationDetailId = detail.SupplierQuotationDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.SupplierQuotationDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
