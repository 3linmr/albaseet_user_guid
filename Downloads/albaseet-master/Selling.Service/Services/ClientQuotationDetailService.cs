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
using Shared.Service.Logic.Calculation;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json;
using Shared.Helper.Extensions;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
	public class ClientQuotationDetailService : BaseService<ClientQuotationDetail>, IClientQuotationDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public ClientQuotationDetailService(IRepository<ClientQuotationDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor,IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public async Task<List<ClientQuotationDetailDto>> GetClientQuotationDetails(int clientQuotationHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<ClientQuotationDetailDto>();

			var data =
			 await (from clientQuotationDetail in _repository.GetAll().Where(x => x.ClientQuotationHeaderId == clientQuotationHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == clientQuotationDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == clientQuotationDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == clientQuotationDetail.CostCenterId).DefaultIfEmpty()
                   select new ClientQuotationDetailDto
				   {
					   ClientQuotationDetailId = clientQuotationDetail.ClientQuotationDetailId,
                       ClientQuotationHeaderId = clientQuotationDetail.ClientQuotationHeaderId,
					   CostCenterId = clientQuotationDetail.CostCenterId,
                       CostCenterName = costCenter != null? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       ItemId = clientQuotationDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? clientQuotationDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = clientQuotationDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = clientQuotationDetail.IsItemVatInclusive,
					   BarCode = clientQuotationDetail.BarCode,
					   Packing = clientQuotationDetail.Packing,
					   Quantity = clientQuotationDetail.Quantity,
                       SellingPrice = clientQuotationDetail.SellingPrice,
                       TotalValue = clientQuotationDetail.TotalValue,
                       ItemDiscountPercent = clientQuotationDetail.ItemDiscountPercent,
                       ItemDiscountValue = clientQuotationDetail.ItemDiscountValue,
                       TotalValueAfterDiscount = clientQuotationDetail.TotalValueAfterDiscount,
                       HeaderDiscountValue = clientQuotationDetail.HeaderDiscountValue,
                       GrossValue = clientQuotationDetail.GrossValue,
                       VatPercent = clientQuotationDetail.VatPercent,
                       VatValue = clientQuotationDetail.VatValue,
                       SubNetValue = clientQuotationDetail.SubNetValue,
                       OtherTaxValue = clientQuotationDetail.OtherTaxValue,
                       NetValue = clientQuotationDetail.NetValue,
                       Notes = clientQuotationDetail.Notes,
                       ItemNote = clientQuotationDetail.ItemNote,
                       ConsumerPrice = clientQuotationDetail.ConsumerPrice,
                       CostPrice = clientQuotationDetail.CostPrice,
                       CostPackage = clientQuotationDetail.CostPackage,
                       CostValue = clientQuotationDetail.CostValue,
                       LastSalesPrice = clientQuotationDetail.LastSalesPrice,

                       CreatedAt = clientQuotationDetail.CreatedAt,
                       IpAddressCreated = clientQuotationDetail.IpAddressCreated,
                       UserNameCreated = clientQuotationDetail.UserNameCreated,
				   }).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var clientQuotationDetail in data)
			{
				var model = new ClientQuotationDetailDto();
				model = clientQuotationDetail;
                model.Packages = packages.Where(x => x.ItemId == clientQuotationDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				model.ItemTaxData = itemTaxData.Where(x => x.ItemId == clientQuotationDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
			}
			return modelList;
		}

        public async Task<List<ClientQuotationDetailDto>> SaveClientQuotationDetails(int clientQuotationHeaderId, List<ClientQuotationDetailDto> clientQuotationDetails)
        {
            if (clientQuotationDetails.Any())
            {
                await EditClientQuotationDetail(clientQuotationDetails);
                return await AddClientQuotationDetail(clientQuotationDetails, clientQuotationHeaderId);
            }
            return clientQuotationDetails;
        }
        public async Task<bool> DeleteClientQuotationDetails(int clientQuotationHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.ClientQuotationHeaderId == clientQuotationHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteClientQuotationDetailList(List<ClientQuotationDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ClientQuotationHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ClientQuotationDetailId != p.ClientQuotationDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<ClientQuotationDetailDto>> AddClientQuotationDetail(List<ClientQuotationDetailDto> details, int headerId)
        {   
            var current = details.Where(x => x.ClientQuotationDetailId <= 0).ToList();
            var modelList = new List<ClientQuotationDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ClientQuotationDetail()
                {
                    ClientQuotationDetailId = newId,
                    ClientQuotationHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    SellingPrice = detail.SellingPrice,
                    TotalValue = detail.TotalValue,
                    ItemDiscountPercent = detail.ItemDiscountPercent,
                    ItemDiscountValue = detail.ItemDiscountValue,
                    TotalValueAfterDiscount = detail.TotalValueAfterDiscount,
                    HeaderDiscountValue = detail.HeaderDiscountValue,
                    GrossValue = detail.GrossValue,
                    VatPercent = detail.VatPercent,
                    VatValue = detail.VatValue,
                    SubNetValue = detail.SubNetValue,
                    OtherTaxValue = detail.OtherTaxValue,
                    NetValue = detail.NetValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,
                    ConsumerPrice = detail.ConsumerPrice,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    LastSalesPrice = detail.LastSalesPrice,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                detail.ClientQuotationDetailId = newId;
                detail.ClientQuotationHeaderId = headerId;

                modelList.Add(model);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
            }
            return details;
        }

        private async Task<bool> EditClientQuotationDetail(List<ClientQuotationDetailDto> clientQuotationDetails)
        {   
            var current = clientQuotationDetails.Where(x => x.ClientQuotationDetailId > 0).ToList();
            var modelList = new List<ClientQuotationDetail>();
            foreach (var detail in current)
            {
                var model = new ClientQuotationDetail()
                {
                    ClientQuotationDetailId = detail.ClientQuotationDetailId,
                    ClientQuotationHeaderId = detail.ClientQuotationHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    SellingPrice = detail.SellingPrice,
                    TotalValue = detail.TotalValue,
                    ItemDiscountPercent = detail.ItemDiscountPercent,
                    ItemDiscountValue = detail.ItemDiscountValue,
                    TotalValueAfterDiscount = detail.TotalValueAfterDiscount,
                    HeaderDiscountValue = detail.HeaderDiscountValue,
                    GrossValue = detail.GrossValue,
                    VatPercent = detail.VatPercent,
                    VatValue = detail.VatValue,
                    SubNetValue = detail.SubNetValue,
                    OtherTaxValue = detail.OtherTaxValue,
                    NetValue = detail.NetValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,
                    ConsumerPrice = detail.ConsumerPrice,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    LastSalesPrice = detail.LastSalesPrice,

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
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientQuotationDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
