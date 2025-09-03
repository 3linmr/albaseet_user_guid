using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
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

namespace Sales.Service.Services
{
	public class ClientQuotationApprovalDetailTaxService : BaseService<ClientQuotationApprovalDetailTax>, IClientQuotationApprovalDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientQuotationApprovalDetailService _clientQuotationApprovalDetailService;

		public ClientQuotationApprovalDetailTaxService(IRepository<ClientQuotationApprovalDetailTax> repository,IHttpContextAccessor httpContextAccessor, IClientQuotationApprovalDetailService clientQuotationApprovalDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _clientQuotationApprovalDetailService = clientQuotationApprovalDetailService;
        }

		public IQueryable<ClientQuotationApprovalDetailTaxDto> GetClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId)
		{
			var data =
			        (
                    from clientQuotationApprovalDetail in _clientQuotationApprovalDetailService.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeaderId)
                    from clientQuotationApprovalDetailTax in _repository.GetAll().Where(x => x.ClientQuotationApprovalDetailId == clientQuotationApprovalDetail.ClientQuotationApprovalDetailId)
                    select new ClientQuotationApprovalDetailTaxDto
				    {
                         ClientQuotationApprovalDetailTaxId = clientQuotationApprovalDetailTax.ClientQuotationApprovalDetailTaxId,
                         ClientQuotationApprovalDetailId = clientQuotationApprovalDetailTax.ClientQuotationApprovalDetailId,
                         TaxId = clientQuotationApprovalDetailTax.TaxId,
                         CreditAccountId = clientQuotationApprovalDetailTax.CreditAccountId,
                         TaxAfterVatInclusive = clientQuotationApprovalDetailTax.TaxAfterVatInclusive,
                         TaxPercent = clientQuotationApprovalDetailTax.TaxPercent,
                         TaxValue = clientQuotationApprovalDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId, List<ClientQuotationApprovalDetailTaxDto> clientQuotationApprovalDetailTaxes)
        {
            await DeleteClientQuotationApprovalDetailTax(clientQuotationApprovalDetailTaxes, clientQuotationApprovalHeaderId);
            if (clientQuotationApprovalDetailTaxes.Any())
            {
                await AddClientQuotationApprovalDetailTax(clientQuotationApprovalDetailTaxes);
                await EditClientQuotationApprovalDetailTax(clientQuotationApprovalDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId)
        {
            var data = await (
                       from clientQuotationApprovalDetail in _clientQuotationApprovalDetailService.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeaderId)
                       from clientQuotationApprovalDetailTax in _repository.GetAll().Where(x => x.ClientQuotationApprovalDetailId == clientQuotationApprovalDetail.ClientQuotationApprovalDetailId)
                       select clientQuotationApprovalDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteClientQuotationApprovalDetailTax(List<ClientQuotationApprovalDetailTaxDto> details, int headerId)
        {
            var current = await (
                from clientQuotationApprovalDetail in _clientQuotationApprovalDetailService.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == headerId)
                from clientQuotationApprovalDetailTax in _repository.GetAll().Where(x => x.ClientQuotationApprovalDetailId == clientQuotationApprovalDetail.ClientQuotationApprovalDetailId)
                select clientQuotationApprovalDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.ClientQuotationApprovalDetailTaxId != p.ClientQuotationApprovalDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddClientQuotationApprovalDetailTax(List<ClientQuotationApprovalDetailTaxDto> details)
        {
            var current = details.Where(x => x.ClientQuotationApprovalDetailTaxId <= 0).ToList();
            var modelList = new List<ClientQuotationApprovalDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ClientQuotationApprovalDetailTax()
                {
                    ClientQuotationApprovalDetailTaxId = newId,
                    ClientQuotationApprovalDetailId = detail.ClientQuotationApprovalDetailId,
                    TaxId = detail.TaxId,
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

        private async Task<bool> EditClientQuotationApprovalDetailTax(List<ClientQuotationApprovalDetailTaxDto> clientQuotationApprovalDetailTaxes)
        {            
            var current = clientQuotationApprovalDetailTaxes.Where(x => x.ClientQuotationApprovalDetailTaxId > 0).ToList();
            var modelList = new List<ClientQuotationApprovalDetailTax>();
            foreach (var detail in current)
            {
                var model = new ClientQuotationApprovalDetailTax()
                {
                    ClientQuotationApprovalDetailTaxId = detail.ClientQuotationApprovalDetailTaxId,
                    ClientQuotationApprovalDetailId = detail.ClientQuotationApprovalDetailId,
                    TaxId = detail.TaxId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientQuotationApprovalDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
