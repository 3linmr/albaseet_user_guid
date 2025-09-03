using Shared.CoreOne.Contracts.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.StaticData;
using Shared.CoreOne.Models.Domain.Items;
using System.Diagnostics.Metrics;
using Shared.Helper.Logic;

namespace Shared.Service.Services.Journal
{
	public class CostCenterJournalDetailService : BaseService<CostCenterJournalDetail>, ICostCenterJournalDetailService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<CostCenterJournalDetailService> _localizer;
		private readonly IJournalDetailService _journalDetailService;

		public CostCenterJournalDetailService(IRepository<CostCenterJournalDetail> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<CostCenterJournalDetailService> localizer, IJournalDetailService journalDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_journalDetailService = journalDetailService;
		}

		//public async Task<ResponseDto> SaveCostCenterJournalDetail(int journalDetailId, List<CostCenterJournalDetailDto> detail)
		//{
		//	if (detail.Any())
		//	{
		//		await DeleteCostCenterJournalDetails(journalDetailId, detail);
		//		await CreateCostCenterJournalDetails(journalDetailId, detail);
		//		await UpdateCostCenterJournalDetails(detail);
		//		return new ResponseDto() { Success = true, Message = _localizer["CostCenterJournalDetailsSavedSuccessfully"] };
		//	}
		//	return new ResponseDto() { Success = false, Message = _localizer["NothingToBeSaved"] };
		//}

		//public async Task<bool> CreateCostCenterJournalDetails(int journalDetailId, List<CostCenterJournalDetailDto> detail)
		//{
		//	var newNotes = detail.Where(x => x.JournalDetailId <= 0).ToList();
		//	var modelList = new List<CostCenterJournalDetail>();
		//	var nextId = await GetNextId();
		//	foreach (var item in newNotes)
		//	{
		//		var model = new CostCenterJournalDetail()
		//		{
		//			CostCenterJournalDetailId = nextId,
		//			JournalDetailId = GetJournalDetailId(journalDetailId,detail,modelList),
		//			CostCenterId = item.CostCenterId,
		//			CurrencyId = item.CurrencyId,
		//			CurrencyRate = item.CurrencyRate,
		//			DebitValue = item.DebitValue,
		//			DebitValueEvaluated = item.DebitValueEvaluated,
		//			CreditValue = item.CreditValue,
		//			CreditValueEvaluated = item.CreditValueEvaluated,
		//			Serial = item.Serial,
		//			RemarksAr = item.RemarksAr,
		//			RemarksEn = item.RemarksEn,
		//			CreatedAt = DateHelper.GetDateTimeNow(),
		//			UserNameCreated = await _httpContextAccessor!.GetUserName(),
		//			IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
		//			Hide = false,
		//		};
		//		modelList.Add(model);
		//		nextId++;
		//	}

		//	await _repository.InsertRange(modelList);
		//	await _repository.SaveChanges();
		//	return true;
		//}

		//public static int GetJournalDetailId(int? journalDetailId, List<CostCenterJournalDetailDto> viewModel, List<CostCenterJournalDetail> dbModel)
		//{
		//	var originalItem = viewModel.FirstOrDefault(x => x.JournalDetailId == journalDetailId);
		//	return originalItem != null ? dbModel.Where(x => x.Serial == originalItem.Serial).Select(x => x.JournalDetailId).FirstOrDefault() : 0;
		//}

		//public async Task<bool> UpdateCostCenterJournalDetails(List<CostCenterJournalDetailDto> detail)
		//{
		//	var currentDetails = detail.Where(x => x.JournalDetailId > 0).ToList();
		//	var modelList = new List<CostCenterJournalDetail>();
		//	foreach (var item in currentDetails)
		//	{
		//		var model = new CostCenterJournalDetail()
		//		{
		//			CostCenterJournalDetailId = item.CostCenterJournalDetailId,
		//			JournalDetailId = item.JournalDetailId,
		//			CostCenterId = item.CostCenterId,
		//			CurrencyId = item.CurrencyId,
		//			CurrencyRate = item.CurrencyRate,
		//			DebitValue = item.DebitValue,
		//			DebitValueEvaluated = item.DebitValueEvaluated,
		//			CreditValue = item.CreditValue,
		//			CreditValueEvaluated = item.CreditValueEvaluated,
		//			Serial = item.Serial,
		//			RemarksAr = item.RemarksAr,
		//			RemarksEn = item.RemarksEn,
		//			ModifiedAt = DateHelper.GetDateTimeNow(),
		//			UserNameModified = await _httpContextAccessor!.GetUserName(),
		//			IpAddressModified = _httpContextAccessor?.GetIpAddress(),
		//			Hide = false,
		//		};
		//		modelList.Add(model);
		//	}
		//	_repository.UpdateRange(modelList);
		//	await _repository.SaveChanges();
		//	return true;
		//}
		//public async Task<bool> DeleteCostCenterJournalDetails(int journalDetailId, List<CostCenterJournalDetailDto> detail)
		//{
		//	if (detail.Any())
		//	{
		//		var currentDetails = _repository.GetAll().Where(x => x.JournalDetailId == journalDetailId).AsNoTracking().ToList();
		//		var detailsToBeDeleted = currentDetails.Where(p => detail.All(p2 => p2.JournalDetailId != p.JournalDetailId)).ToList();
		//		if (detailsToBeDeleted.Any())
		//		{
		//			_repository.RemoveRange(detailsToBeDeleted);
		//			await _repository.SaveChanges();
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.CostCenterJournalDetailId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<List<CostCenterJournalDetailDto>> GetCostCenterJournalDetails(int journalHeaderId)
		{
			var data =
				await (from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeaderId)
					   from costCenterJournalDetail in _repository.GetAll().Where(x => x.JournalDetailId == journalDetail.JournalDetailId)
					   select new CostCenterJournalDetailDto()
					   {
						   CostCenterJournalDetailId = costCenterJournalDetail.CostCenterJournalDetailId,
						   JournalDetailId = costCenterJournalDetail.JournalDetailId,
						   DebitValue = costCenterJournalDetail.DebitValue,
						   CreditValue = costCenterJournalDetail.CreditValue,
						   CostCenterId = costCenterJournalDetail.CostCenterId,
						   Serial = costCenterJournalDetail.Serial,
						   RemarksAr = costCenterJournalDetail.RemarksAr,
						   RemarksEn = costCenterJournalDetail.RemarksEn,
						   IpAddressCreated = costCenterJournalDetail.IpAddressCreated,
						   CreatedAt = costCenterJournalDetail.CreatedAt,
						   UserNameCreated = costCenterJournalDetail.UserNameCreated,
						   Distributed = costCenterJournalDetail.IsCostCenterDistributed.GetValueOrDefault()
					   }).ToListAsync();
			return data;
		}

		public async Task<CostCenterJournalDetailReturnDto?> BuildCostCenterJournalDetail(int journalDetailId, int nextCostCenterJournalDetailId, List<CostCenterJournalDetailDto> details)
		{
			var modelsToBeCreated = details.Where(x => x.CostCenterJournalDetailId <= 0).ToList();
			var modelsToBeUpdated = details.Where(x => x.CostCenterJournalDetailId > 0).ToList();

			var modelListCreate = await CreateCostCenterJournalDetail(journalDetailId, nextCostCenterJournalDetailId, modelsToBeCreated);
			var modelListUpdate = await UpdateCostCenterJournalDetail(journalDetailId, modelsToBeUpdated);

			return new CostCenterJournalDetailReturnDto() { JournalDetailCreate = modelListCreate, JournalDetailUpdate = modelListUpdate };
		}

		public async Task<List<CostCenterJournalDetail>> CreateCostCenterJournalDetail(int journalDetailId, int nextCostCenterJournalDetailId, List<CostCenterJournalDetailDto> details)
		{
			var modelList = new List<CostCenterJournalDetail>();
			var counter = 0;
			foreach (var detail in details)
			{
				var model = new CostCenterJournalDetail()
				{
					CostCenterJournalDetailId = nextCostCenterJournalDetailId,
					JournalDetailId = journalDetailId,
					CostCenterId = detail.CostCenterId,
					DebitValue = detail.DebitValue,
					CreditValue = detail.CreditValue,
					IsCostCenterDistributed = detail.Distributed,
					Serial = detail.Serial,
					RemarksAr = detail.RemarksAr,
					RemarksEn = detail.RemarksEn,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				};
				modelList.Add(model);
				nextCostCenterJournalDetailId++;
			}

			if (modelList.All(x => x.Serial == 0))
			{
				foreach (var x in modelList) x.Serial = counter++;
			}
			return modelList;
		}

		public async Task<List<CostCenterJournalDetail>> UpdateCostCenterJournalDetail(int journalDetailId, List<CostCenterJournalDetailDto> details)
		{
			var modelList = new List<CostCenterJournalDetail>();
			foreach (var detail in details)
			{
				var model = new CostCenterJournalDetail()
				{
					CostCenterJournalDetailId = detail.CostCenterJournalDetailId,
					JournalDetailId = journalDetailId,
					CostCenterId = detail.CostCenterId,
					DebitValue = detail.DebitValue,
					CreditValue = detail.CreditValue,
					IsCostCenterDistributed = detail.Distributed,
					Serial = detail.Serial,
					RemarksAr = detail.RemarksAr,
					RemarksEn = detail.RemarksEn,
					CreatedAt = detail.CreatedAt,
					UserNameCreated = detail.UserNameCreated,
					IpAddressCreated = detail.IpAddressCreated,
					Hide = false
				};
				modelList.Add(model);
			}
			return modelList;
		}

		public async Task<List<CostCenterJournalDetail>> HandleDeletedCostCenterJournalDetails(int journalHeaderId, List<CostCenterJournalDetailDto>? costCenterJournalDetail)
		{
			if (costCenterJournalDetail != null)
			{
				var detailsDb =
					await (from journalDetail in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == journalHeaderId)
						   from costCenter in _repository.GetAll().Where(x => x.JournalDetailId == journalDetail.JournalDetailId)
						   select costCenter).AsNoTracking().ToListAsync();
				var detailsToBeDeleted = detailsDb.Where(p => costCenterJournalDetail.All(p2 => p2.CostCenterJournalDetailId != p.CostCenterJournalDetailId)).ToList();
				return detailsToBeDeleted;
			}
			return new List<CostCenterJournalDetail>();
		}

		public async Task<bool> CreateCostCenterJournalDetail(List<CostCenterJournalDetail> journalDetails)
		{
			if (journalDetails.Any())
			{
				await _repository.InsertRange(journalDetails);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> UpdateCostCenterJournalDetail(List<CostCenterJournalDetail> journalDetails)
		{
			if (journalDetails.Any())
			{
				_repository.UpdateRange(journalDetails);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> DeleteCostCenterJournalDetail(List<CostCenterJournalDetail> journalDetails)
		{
			if (journalDetails.Any())
			{
				_repository.RemoveRange(journalDetails);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> DeleteCostCenterJournalDetail(int journalHeaderId)
		{
			var detailsDb =
				await (
					   from costCenter in _repository.GetAll().AsNoTracking()
					from journalDetail in _journalDetailService.GetAll().AsNoTracking().Where(x => x.JournalHeaderId == journalHeaderId && x.JournalDetailId == costCenter.JournalDetailId)
					   select costCenter).ToListAsync();
			if (detailsDb.Any())
			{
				_repository.RemoveRange(detailsDb);
				await _repository.SaveChanges();
			}
			return true;
		}

        public async Task<List<CostCenterJournalDetailDto>> GetCostCenterJournalDetails(int referenceHeaderId, short? menuCode)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var results = await (from costCenterJournalDetail in _repository.FindBy(x => x.ReferenceHeaderId == referenceHeaderId && x.MenuCode == menuCode)
                                 select new CostCenterJournalDetailDto()
                                 {
                                     CostCenterJournalDetailId = costCenterJournalDetail.CostCenterJournalDetailId,
                                     JournalDetailId = costCenterJournalDetail.JournalDetailId,
                                     ItemId = costCenterJournalDetail.ItemId,
                                     MenuCode = costCenterJournalDetail.MenuCode,
                                     ReferenceHeaderId = costCenterJournalDetail.ReferenceHeaderId,
                                     ReferenceDetailId = costCenterJournalDetail.ReferenceDetailId,
                                     CostCenterId = costCenterJournalDetail.CostCenterId,
                                     Distributed = costCenterJournalDetail.IsCostCenterDistributed.GetValueOrDefault(),
                                     Serial = costCenterJournalDetail.Serial,
                                     DebitValue = costCenterJournalDetail.DebitValue,
                                     CreditValue = costCenterJournalDetail.CreditValue,
                                     RemarksAr = costCenterJournalDetail.RemarksAr,
                                     RemarksEn = costCenterJournalDetail.RemarksEn,

                                     CreatedAt = costCenterJournalDetail.CreatedAt,
                                     UserNameCreated = costCenterJournalDetail.UserNameCreated,
                                     IpAddressCreated = costCenterJournalDetail.IpAddressCreated
                                 }).ToListAsync();

            return results;
        }

        public async Task<bool> SaveCostCenterJournalDetails(int referenceHeaderId, List<CostCenterJournalDetailDto> costCenterJournalDetails, short? menuCode)
        {
            await DeleteCostCenterJournalDetails(costCenterJournalDetails, referenceHeaderId, menuCode);
            if (costCenterJournalDetails.Any())
            {
                await AddCostCenterJournalDetails(costCenterJournalDetails, referenceHeaderId, menuCode);
                await EditCostCenterJournalDetails(costCenterJournalDetails, referenceHeaderId, menuCode);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCostCenterJournalDetails(int referenceHeaderId, short? menuCode)
        {
            var data = await _repository.GetAll().Where(x => x.ReferenceHeaderId == referenceHeaderId && x.MenuCode == menuCode).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteCostCenterJournalDetails(List<CostCenterJournalDetailDto> costCenterJournalDetails, int referenceHeaderId, short? menuCode)
        {
            var current = _repository.GetAll().Where(x => x.ReferenceHeaderId == referenceHeaderId && x.MenuCode == menuCode).AsNoTracking().ToList();
            var toBeDeleted = current.Where(p => costCenterJournalDetails.All(p2 => p2.CostCenterJournalDetailId != p.CostCenterJournalDetailId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddCostCenterJournalDetails(List<CostCenterJournalDetailDto> costCenterJournalDetails, int referenceHeaderId, short? menuCode)
        {
            var current = costCenterJournalDetails.Where(x => x.CostCenterJournalDetailId <= 0).ToList();
            var modelList = new List<CostCenterJournalDetail>();
            var newId = await GetNextId();
            var newSerial = await GetNextSerial(referenceHeaderId, menuCode);
            foreach (var detail in current)
            {
                var newNote = new CostCenterJournalDetail()
                {
                    CostCenterJournalDetailId = newId,
                    JournalDetailId = detail.JournalDetailId,
                    ItemId = detail.ItemId,
                    CostCenterId = detail.CostCenterId,
                    MenuCode = menuCode,
                    ReferenceHeaderId = referenceHeaderId,
                    ReferenceDetailId = detail.ReferenceDetailId,
                    Serial = newSerial,
                    DebitValue = detail.DebitValue,
                    CreditValue = detail.CreditValue,
                    IsCostCenterDistributed = detail.Distributed,
					RemarksAr = detail.RemarksAr,
                    RemarksEn = detail.RemarksEn,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };
                modelList.Add(newNote);
                newId++;
                newSerial++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
                return true;
            }

            return true;
        }

        private async Task<bool> EditCostCenterJournalDetails(List<CostCenterJournalDetailDto> costCenterJournalDetails, int referenceHeaderId, short? menuCode)
        {
            var current = costCenterJournalDetails.Where(x => x.CostCenterJournalDetailId > 0).ToList();
            var modelList = new List<CostCenterJournalDetail>();
            foreach (var detail in current)
            {
                var newNote = new CostCenterJournalDetail()
                {
                    CostCenterJournalDetailId = detail.CostCenterJournalDetailId,
                    JournalDetailId = detail.JournalDetailId,
                    ItemId = detail.ItemId,
                    CostCenterId = detail.CostCenterId,
                    MenuCode = menuCode,
                    ReferenceHeaderId = referenceHeaderId,
                    ReferenceDetailId = detail.ReferenceDetailId,
                    Serial = detail.Serial,
                    DebitValue = detail.DebitValue,
                    CreditValue = detail.CreditValue,
                    IsCostCenterDistributed = detail.Distributed,
					RemarksAr = detail.RemarksAr,
                    RemarksEn = detail.RemarksEn,

                    IpAddressCreated = detail.IpAddressCreated,
                    CreatedAt = detail.CreatedAt,
                    UserNameCreated = detail.UserNameCreated,
                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };
                modelList.Add(newNote);
            }

            if (modelList.Any())
            {
                _repository.UpdateRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<int> GetNextSerial(int referenceHeaderId, short? menuCode)
        {
            int serial = 1;
            try { serial = await _repository.GetAll().Where(x => x.ReferenceHeaderId == referenceHeaderId && x.MenuCode == menuCode).MaxAsync(a => a.Serial) + 1; } catch { serial = 1; }
            return serial;
        }

		public async Task UpdateCostCenterJournalDetailsBasedOnInvoiceDetails<DetailType>(int invoiceHeaderId, List<DetailType> invoiceDetails, Func<DetailType, int> detailIdSelector, Func<DetailType,int> itemIdSelector, Func<DetailType, int?> costCenterIdSelector, Func<DetailType, decimal> creditValueSelector, Func<DetailType, decimal> debitValueSelector, Func<DetailType, string?> remarksSelector, short menuCode)
		{
			var currentCostCenterJournalDetails = await _repository.GetAll().Where(x => x.ReferenceHeaderId == invoiceHeaderId && x.MenuCode == menuCode).AsNoTracking().ToListAsync();
			var filteredInvoiceDetails = invoiceDetails.Where(x => costCenterIdSelector(x) != null).ToList();

			var costCenterJournalDetailsToSave = from invoiceDetail in filteredInvoiceDetails
												 from currentCostCenterJournalDetail in currentCostCenterJournalDetails.Where(x => x.ReferenceDetailId == detailIdSelector(invoiceDetail)).DefaultIfEmpty()
												 select new CostCenterJournalDetailDto
												 {
													 CostCenterJournalDetailId = currentCostCenterJournalDetail != null ? currentCostCenterJournalDetail.CostCenterJournalDetailId : 0,
													 JournalDetailId = null,
													 ItemId = itemIdSelector(invoiceDetail),
													 CostCenterId = (int)costCenterIdSelector(invoiceDetail)!,
													 MenuCode = menuCode,
													 ReferenceHeaderId = invoiceHeaderId,
													 ReferenceDetailId = detailIdSelector(invoiceDetail),
													 Serial = currentCostCenterJournalDetail?.Serial ?? 0,
													 DebitValue = debitValueSelector(invoiceDetail),
									 				 CreditValue = creditValueSelector(invoiceDetail),
									 				 Distributed = false,
													 RemarksAr = remarksSelector(invoiceDetail),
													 RemarksEn = remarksSelector(invoiceDetail),
													 CreatedAt = currentCostCenterJournalDetail?.CreatedAt,
													 UserNameCreated = currentCostCenterJournalDetail?.UserNameCreated,
													 IpAddressCreated = currentCostCenterJournalDetail?.IpAddressCreated,
												 };

			await SaveCostCenterJournalDetails(invoiceHeaderId, costCenterJournalDetailsToSave.ToList(), menuCode);
		}
	}
}
