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
	public class PurchaseOrderDetailTaxService : BaseService<PurchaseOrderDetailTax>, IPurchaseOrderDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;

		public PurchaseOrderDetailTaxService(IRepository<PurchaseOrderDetailTax> repository,IHttpContextAccessor httpContextAccessor, IPurchaseOrderDetailService purchaseOrderDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _purchaseOrderDetailService = purchaseOrderDetailService;
        }

		public IQueryable<PurchaseOrderDetailTaxDto> GetPurchaseOrderDetailTaxes(int purchaseOrderHeaderId)
		{
			var data =
			        (
                    from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                    from purchaseOrderDetailTax in _repository.GetAll().Where(x => x.PurchaseOrderDetailId == purchaseOrderDetail.PurchaseOrderDetailId)
                    select new PurchaseOrderDetailTaxDto
				    {
                         PurchaseOrderDetailTaxId = purchaseOrderDetailTax.PurchaseOrderDetailTaxId,
                         PurchaseOrderDetailId = purchaseOrderDetailTax.PurchaseOrderDetailId,
                         TaxId = purchaseOrderDetailTax.TaxId,
                         DebitAccountId = purchaseOrderDetailTax.DebitAccountId,
                         TaxAfterVatInclusive = purchaseOrderDetailTax.TaxAfterVatInclusive,
                         TaxPercent = purchaseOrderDetailTax.TaxPercent,
                         TaxValue = purchaseOrderDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SavePurchaseOrderDetailTaxes(int purchaseOrderHeaderId, List<PurchaseOrderDetailTaxDto> purchaseOrderDetailTaxes)
        {
            await DeletePurchaseOrderDetailTax(purchaseOrderDetailTaxes, purchaseOrderHeaderId);
            if (purchaseOrderDetailTaxes.Any())
            {
                await AddPurchaseOrderDetailTax(purchaseOrderDetailTaxes);
                await EditPurchaseOrderDetailTax(purchaseOrderDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseOrderDetailTaxes(int purchaseOrderHeaderId)
        {
            var data = await (
                       from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                       from purchaseOrderDetailTax in _repository.GetAll().Where(x => x.PurchaseOrderDetailId == purchaseOrderDetail.PurchaseOrderDetailId)
                       select purchaseOrderDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeletePurchaseOrderDetailTax(List<PurchaseOrderDetailTaxDto> details, int headerId)
        {
            var current = await (
                from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == headerId)
                from purchaseOrderDetailTax in _repository.GetAll().Where(x => x.PurchaseOrderDetailId == purchaseOrderDetail.PurchaseOrderDetailId)
                select purchaseOrderDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.PurchaseOrderDetailTaxId != p.PurchaseOrderDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddPurchaseOrderDetailTax(List<PurchaseOrderDetailTaxDto> details)
        {
            var current = details.Where(x => x.PurchaseOrderDetailTaxId <= 0).ToList();
            var modelList = new List<PurchaseOrderDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new PurchaseOrderDetailTax()
                {
                    PurchaseOrderDetailTaxId = newId,
                    PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
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

        private async Task<bool> EditPurchaseOrderDetailTax(List<PurchaseOrderDetailTaxDto> purchaseOrderDetailTaxes)
        {            
            var current = purchaseOrderDetailTaxes.Where(x => x.PurchaseOrderDetailTaxId > 0).ToList();
            var modelList = new List<PurchaseOrderDetailTax>();
            foreach (var detail in current)
            {
                var model = new PurchaseOrderDetailTax()
                {
                    PurchaseOrderDetailTaxId = detail.PurchaseOrderDetailTaxId,
                    PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseOrderDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
