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
	public class PurchaseInvoiceReturnDetailTaxService : BaseService<PurchaseInvoiceReturnDetailTax>, IPurchaseInvoiceReturnDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;

		public PurchaseInvoiceReturnDetailTaxService(IRepository<PurchaseInvoiceReturnDetailTax> repository,IHttpContextAccessor httpContextAccessor, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
        }

		public IQueryable<PurchaseInvoiceReturnDetailTaxDto> GetPurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId)
		{
			var data =
			        (
                    from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId)
                    from purchaseInvoiceReturnDetailTax in _repository.GetAll().Where(x => x.PurchaseInvoiceReturnDetailId == purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId)
                    select new PurchaseInvoiceReturnDetailTaxDto
				    {
                         PurchaseInvoiceReturnDetailTaxId = purchaseInvoiceReturnDetailTax.PurchaseInvoiceReturnDetailTaxId,
                         PurchaseInvoiceReturnDetailId = purchaseInvoiceReturnDetailTax.PurchaseInvoiceReturnDetailId,
                         TaxId = purchaseInvoiceReturnDetailTax.TaxId,
                         TaxTypeId = purchaseInvoiceReturnDetailTax.TaxTypeId,
                         CreditAccountId = purchaseInvoiceReturnDetailTax.CreditAccountId,
                         TaxAfterVatInclusive = purchaseInvoiceReturnDetailTax.TaxAfterVatInclusive,
                         TaxPercent = purchaseInvoiceReturnDetailTax.TaxPercent,
                         TaxValue = purchaseInvoiceReturnDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SavePurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId, List<PurchaseInvoiceReturnDetailTaxDto> purchaseInvoiceReturnDetailTaxes)
        {
            await DeletePurchaseInvoiceReturnDetailTax(purchaseInvoiceReturnDetailTaxes, purchaseInvoiceReturnHeaderId);
            if (purchaseInvoiceReturnDetailTaxes.Any())
            {
                await AddPurchaseInvoiceReturnDetailTax(purchaseInvoiceReturnDetailTaxes);
                await EditPurchaseInvoiceReturnDetailTax(purchaseInvoiceReturnDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId)
        {
            var data = await (
                       from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId)
                       from purchaseInvoiceReturnDetailTax in _repository.GetAll().Where(x => x.PurchaseInvoiceReturnDetailId == purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId)
                       select purchaseInvoiceReturnDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeletePurchaseInvoiceReturnDetailTax(List<PurchaseInvoiceReturnDetailTaxDto> details, int headerId)
        {
            var current = await (
                from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == headerId)
                from purchaseInvoiceReturnDetailTax in _repository.GetAll().Where(x => x.PurchaseInvoiceReturnDetailId == purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId)
                select purchaseInvoiceReturnDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.PurchaseInvoiceReturnDetailTaxId != p.PurchaseInvoiceReturnDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddPurchaseInvoiceReturnDetailTax(List<PurchaseInvoiceReturnDetailTaxDto> details)
        {
            var current = details.Where(x => x.PurchaseInvoiceReturnDetailTaxId <= 0).ToList();
            var modelList = new List<PurchaseInvoiceReturnDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceReturnDetailTax()
                {
                    PurchaseInvoiceReturnDetailTaxId = newId,
                    PurchaseInvoiceReturnDetailId = detail.PurchaseInvoiceReturnDetailId,
                    TaxId = detail.TaxId,
                    TaxTypeId = detail.TaxTypeId,
                    CreditAccountId = detail.CreditAccountId,
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

        private async Task<bool> EditPurchaseInvoiceReturnDetailTax(List<PurchaseInvoiceReturnDetailTaxDto> purchaseInvoiceReturnDetailTaxes)
        {            
            var current = purchaseInvoiceReturnDetailTaxes.Where(x => x.PurchaseInvoiceReturnDetailTaxId > 0).ToList();
            var modelList = new List<PurchaseInvoiceReturnDetailTax>();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceReturnDetailTax()
                {
                    PurchaseInvoiceReturnDetailTaxId = detail.PurchaseInvoiceReturnDetailTaxId,
                    PurchaseInvoiceReturnDetailId = detail.PurchaseInvoiceReturnDetailId,
                    TaxId = detail.TaxId,
                    TaxTypeId = detail.TaxTypeId,
                    CreditAccountId = detail.CreditAccountId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceReturnDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
