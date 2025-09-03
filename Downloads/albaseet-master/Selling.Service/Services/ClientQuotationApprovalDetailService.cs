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
	public class ClientQuotationApprovalDetailService : BaseService<ClientQuotationApprovalDetail>, IClientQuotationApprovalDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public ClientQuotationApprovalDetailService(IRepository<ClientQuotationApprovalDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor,IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public IQueryable<ClientQuotationApprovalDetailDto> GetClientQuotationApprovalDetailsAsQueryable(int clientQuotationApprovalHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<ClientQuotationApprovalDetailDto>();

			return from clientQuotationApprovalDetail in _repository.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == clientQuotationApprovalDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == clientQuotationApprovalDetail.CostCenterId).DefaultIfEmpty()
				   select new ClientQuotationApprovalDetailDto
				   {
					   ClientQuotationApprovalDetailId = clientQuotationApprovalDetail.ClientQuotationApprovalDetailId,
					   ClientQuotationApprovalHeaderId = clientQuotationApprovalDetail.ClientQuotationApprovalHeaderId,
					   CostCenterId = clientQuotationApprovalDetail.CostCenterId,
					   CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
					   ItemId = clientQuotationApprovalDetail.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? clientQuotationApprovalDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
					   TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = clientQuotationApprovalDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					   IsItemVatInclusive = clientQuotationApprovalDetail.IsItemVatInclusive,
					   BarCode = clientQuotationApprovalDetail.BarCode,
					   Packing = clientQuotationApprovalDetail.Packing,
					   Quantity = clientQuotationApprovalDetail.Quantity,
					   SellingPrice = clientQuotationApprovalDetail.SellingPrice,
					   TotalValue = clientQuotationApprovalDetail.TotalValue,
					   ItemDiscountPercent = clientQuotationApprovalDetail.ItemDiscountPercent,
					   ItemDiscountValue = clientQuotationApprovalDetail.ItemDiscountValue,
					   TotalValueAfterDiscount = clientQuotationApprovalDetail.TotalValueAfterDiscount,
					   HeaderDiscountValue = clientQuotationApprovalDetail.HeaderDiscountValue,
					   GrossValue = clientQuotationApprovalDetail.GrossValue,
					   VatPercent = clientQuotationApprovalDetail.VatPercent,
					   VatValue = clientQuotationApprovalDetail.VatValue,
					   SubNetValue = clientQuotationApprovalDetail.SubNetValue,
					   OtherTaxValue = clientQuotationApprovalDetail.OtherTaxValue,
					   NetValue = clientQuotationApprovalDetail.NetValue,
					   Notes = clientQuotationApprovalDetail.Notes,
                       ItemNote = clientQuotationApprovalDetail.ItemNote,
					   ConsumerPrice = clientQuotationApprovalDetail.ConsumerPrice,
					   CostPrice = clientQuotationApprovalDetail.CostPrice,
					   CostPackage = clientQuotationApprovalDetail.CostPackage,
					   CostValue = clientQuotationApprovalDetail.CostValue,
					   LastSalesPrice = clientQuotationApprovalDetail.LastSalesPrice,

					   CreatedAt = clientQuotationApprovalDetail.CreatedAt,
					   IpAddressCreated = clientQuotationApprovalDetail.IpAddressCreated,
					   UserNameCreated = clientQuotationApprovalDetail.UserNameCreated,
				   };
		}

		public async Task<List<ClientQuotationApprovalDetailDto>> GetClientQuotationApprovalDetails(int clientQuotationApprovalHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<ClientQuotationApprovalDetailDto>();

			var data = await GetClientQuotationApprovalDetailsAsQueryable(clientQuotationApprovalHeaderId).ToListAsync();

			var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var clientQuotationApprovalDetail in data)
			{
				var model = new ClientQuotationApprovalDetailDto();
				model = clientQuotationApprovalDetail;
                model.Packages = packages.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				model.ItemTaxData = itemTaxData.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
			}
			return modelList;
		}

        public async Task<List<ClientQuotationApprovalDetailDto>> SaveClientQuotationApprovalDetails(int clientQuotationApprovalHeaderId, List<ClientQuotationApprovalDetailDto> clientQuotationApprovalDetails)
        {
            if (clientQuotationApprovalDetails.Any())
            {
                await EditClientQuotationApprovalDetail(clientQuotationApprovalDetails);
                return await AddClientQuotationApprovalDetail(clientQuotationApprovalDetails, clientQuotationApprovalHeaderId);
            }
            return clientQuotationApprovalDetails;
        }
        public async Task<bool> DeleteClientQuotationApprovalDetails(int clientQuotationApprovalHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == clientQuotationApprovalHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteClientQuotationApprovalDetailList(List<ClientQuotationApprovalDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ClientQuotationApprovalHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ClientQuotationApprovalDetailId != p.ClientQuotationApprovalDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<ClientQuotationApprovalDetailDto>> AddClientQuotationApprovalDetail(List<ClientQuotationApprovalDetailDto> details, int headerId)
        {   
            var current = details.Where(x => x.ClientQuotationApprovalDetailId <= 0).ToList();
            var modelList = new List<ClientQuotationApprovalDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ClientQuotationApprovalDetail()
                {
                    ClientQuotationApprovalDetailId = newId,
                    ClientQuotationApprovalHeaderId = headerId,
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

                detail.ClientQuotationApprovalDetailId = newId;
                detail.ClientQuotationApprovalHeaderId = headerId;

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

        private async Task<bool> EditClientQuotationApprovalDetail(List<ClientQuotationApprovalDetailDto> clientQuotationApprovalDetails)
        {   
            var current = clientQuotationApprovalDetails.Where(x => x.ClientQuotationApprovalDetailId > 0).ToList();
            var modelList = new List<ClientQuotationApprovalDetail>();
            foreach (var detail in current)
            {
                var model = new ClientQuotationApprovalDetail()
                {
                    ClientQuotationApprovalDetailId = detail.ClientQuotationApprovalDetailId,
                    ClientQuotationApprovalHeaderId = detail.ClientQuotationApprovalHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientQuotationApprovalDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
