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
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
	public class ClientPriceRequestDetailService : BaseService<ClientPriceRequestDetail>, IClientPriceRequestDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;

		public ClientPriceRequestDetailService(IRepository<ClientPriceRequestDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor): base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
        }

		public async Task<List<ClientPriceRequestDetailDto>> GetClientPriceRequestDetails(int clientPriceRequestHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<ClientPriceRequestDetailDto>();

			var data =
			 await (from clientPriceRequestDetail in _repository.GetAll().Where(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == clientPriceRequestDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == clientPriceRequestDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == clientPriceRequestDetail.CostCenterId).DefaultIfEmpty()
                   select new ClientPriceRequestDetailDto
				   {
					   ClientPriceRequestDetailId = clientPriceRequestDetail.ClientPriceRequestDetailId,
                       ClientPriceRequestHeaderId = clientPriceRequestDetail.ClientPriceRequestHeaderId,
                       ItemId = clientPriceRequestDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? clientPriceRequestDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = clientPriceRequestDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = clientPriceRequestDetail.IsItemVatInclusive,
					   CostCenterId = clientPriceRequestDetail.CostCenterId,
                       CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
					   BarCode = clientPriceRequestDetail.BarCode,
					   Packing = clientPriceRequestDetail.Packing,
					   Quantity = clientPriceRequestDetail.Quantity,
					   ConsumerPrice = clientPriceRequestDetail.ConsumerPrice,
					   ConsumerValue = clientPriceRequestDetail.ConsumerValue,
					   CostPrice = clientPriceRequestDetail.CostPrice,
					   CostPackage = clientPriceRequestDetail.CostPackage,
					   CostValue = clientPriceRequestDetail.CostValue,
                       Notes = clientPriceRequestDetail.Notes,
                       ItemNote = clientPriceRequestDetail.ItemNote,
                       CreatedAt = clientPriceRequestDetail.CreatedAt,
					   IpAddressCreated = clientPriceRequestDetail.IpAddressCreated,
					   UserNameCreated = clientPriceRequestDetail.UserNameCreated
				   }).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

			foreach (var clientPriceRequestDetail in data)
			{
				var model = new ClientPriceRequestDetailDto();
				model = clientPriceRequestDetail;
                model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == clientPriceRequestDetail.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<bool> SaveClientPriceRequestDetails(int clientPriceRequestHeaderId, List<ClientPriceRequestDetailDto> clientPriceRequestDetails)
        {
            if (clientPriceRequestDetails.Any())
            {
                await DeleteClientPriceRequestDetail(clientPriceRequestDetails, clientPriceRequestHeaderId);
                await AddClientPriceRequestDetail(clientPriceRequestDetails, clientPriceRequestHeaderId);
                await EditClientPriceRequestDetail(clientPriceRequestDetails);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteClientPriceRequestDetails(int clientPriceRequestHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.ClientPriceRequestHeaderId == clientPriceRequestHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteClientPriceRequestDetail(List<ClientPriceRequestDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ClientPriceRequestHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ClientPriceRequestDetailId != p.ClientPriceRequestDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> AddClientPriceRequestDetail(List<ClientPriceRequestDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.ClientPriceRequestDetailId <= 0).ToList();
            var modelList = new List<ClientPriceRequestDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ClientPriceRequestDetail()
                {
                    ClientPriceRequestDetailId = newId,
                    ClientPriceRequestHeaderId = headerId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    CostCenterId = detail.CostCenterId,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ConsumerPrice = detail.ConsumerPrice,
                    ConsumerValue = detail.ConsumerValue,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,

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

        private async Task<bool> EditClientPriceRequestDetail(List<ClientPriceRequestDetailDto> clientPriceRequestDetails)
        {
            var current = clientPriceRequestDetails.Where(x => x.ClientPriceRequestDetailId > 0).ToList();
            var modelList = new List<ClientPriceRequestDetail>();
            foreach (var detail in current)
            {
                var model = new ClientPriceRequestDetail()
                {
                    ClientPriceRequestDetailId = detail.ClientPriceRequestDetailId,
                    ClientPriceRequestHeaderId = detail.ClientPriceRequestHeaderId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    CostCenterId = detail.CostCenterId,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ConsumerPrice = detail.ConsumerPrice,
                    ConsumerValue = detail.ConsumerValue,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,

                    CreatedAt = detail.CreatedAt,
                    UserNameCreated = detail.UserNameCreated,
                    IpAddressCreated = detail.IpAddressCreated,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientPriceRequestDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
