using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.CoreOne.Models.StaticData.StaticData;

namespace Shared.Service.Services.Journal
{
	public class JournalDetailService : BaseService<JournalDetail>, IJournalDetailService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<JournalDetailService> _localizer;
		private readonly IEntityTypeService _entityTypeService;

		public JournalDetailService(IRepository<JournalDetail> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<JournalDetailService> localizer, IEntityTypeService entityTypeService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_entityTypeService = entityTypeService;
		}

		//public async Task<ResponseDto> SaveJournalDetail(int journalHeaderId, List<JournalDetailDto> detail)
		//{
		//	if (detail.Any())
		//	{
		//		await DeleteJournalDetails(journalHeaderId, detail);
		//		await CreateJournalDetails(journalHeaderId, detail);
		//		await UpdateJournalDetails(detail);
		//		return new ResponseDto() { Success = true, Message = _localizer["JournalDetailsSavedSuccessfully"] };
		//	}
		//	return new ResponseDto() { Success = false, Message = _localizer["NothingToBeSaved"] };
		//}

		//public async Task<bool> CreateJournalDetails(int journalHeaderId, List<JournalDetailDto> detail)
		//{
		//	var newNotes = detail.Where(x => x.JournalDetailId <= 0).ToList();
		//	var modelList = new List<JournalDetail>();
		//	var nextId = await GetNextId();
		//	foreach (var item in newNotes)
		//	{
		//		var model = new JournalDetail()
		//		{
		//			JournalDetailId = nextId,
		//			JournalHeaderId = journalHeaderId,
		//			AccountId = item.AccountId,
		//			CurrencyId = item.CurrencyId,
		//			CurrencyRate = item.CurrencyRate,
		//			DebitValue = item.DebitValue,
		//			DebitValueEvaluated = item.DebitValueEvaluated,
		//			CreditValue = item.CreditValue,
		//			CreditValueEvaluated = item.CreditValueEvaluated,
		//			Serial = item.Serial,
		//			EntityNameAr = item.EntityNameAr,
		//			EntityNameEn = item.EntityNameEn,
		//			TaxCode = item.TaxCode,
		//			EntityEmail = item.EntityEmail,
		//			RemarksAr = item.RemarksAr,
		//			RemarksEn = item.RemarksEn,
		//			AutomaticRemarks = item.AutomaticRemarks,
		//			IsTax = item.IsTax,
		//			TaxId = item.TaxId,
		//			TaxPercent = item.TaxPercent,
		//			TaxParentId = GetTaxParentId(item.TaxParentId,detail, modelList),
		//			TaxReference = item.TaxReference,
		//			TaxDate = item.TaxDate,
		//			CreatedAt = DateHelper.GetDateTimeNow(),
		//			UserNameCreated = await _httpContextAccessor!.GetUserName(),
		//			IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
		//			Hide = false
		//		};
		//		modelList.Add(model);
		//		nextId++;
		//	}

		//	await _repository.InsertRange(modelList);
		//	await _repository.SaveChanges();
		//	return true;
		//}

		//public static int GetTaxParentId(int? taxParentId, List<JournalDetailDto> viewModel, List<JournalDetail> dbModel)
		//{
		//	var originalItem = viewModel.FirstOrDefault(x => x.JournalDetailId == taxParentId);
		//	return originalItem != null ? dbModel.Where(x => x.Serial == originalItem.Serial).Select(x => x.JournalDetailId).FirstOrDefault() : 0;
		//}

		//public async Task<bool> UpdateJournalDetails(List<JournalDetailDto> detail)
		//{
		//	var currentDetails = detail.Where(x => x.JournalDetailId > 0).ToList();
		//	var modelList = new List<JournalDetail>();
		//	foreach (var item in currentDetails)
		//	{
		//		var model = new JournalDetail()
		//		{
		//			JournalDetailId = item.JournalDetailId,
		//			JournalHeaderId = item.JournalHeaderId,
		//			AccountId = item.AccountId,
		//			CurrencyId = item.CurrencyId,
		//			CurrencyRate = item.CurrencyRate,
		//			DebitValue = item.DebitValue,
		//			DebitValueEvaluated = item.DebitValueEvaluated,
		//			CreditValue = item.CreditValue,
		//			CreditValueEvaluated = item.CreditValueEvaluated,
		//			Serial = item.Serial,
		//			EntityNameAr = item.EntityNameAr,
		//			EntityNameEn = item.EntityNameEn,
		//			TaxCode = item.TaxCode,
		//			EntityEmail = item.EntityEmail,
		//			RemarksAr = item.RemarksAr,
		//			RemarksEn = item.RemarksEn,
		//			AutomaticRemarks = item.AutomaticRemarks,
		//			IsTax = item.IsTax,
		//			TaxId = item.TaxId,
		//			TaxPercent = item.TaxPercent,
		//			TaxParentId = item.TaxParentId,
		//			TaxReference = item.TaxReference,
		//			TaxDate = item.TaxDate,
		//			ModifiedAt = DateHelper.GetDateTimeNow(),
		//			UserNameModified = await _httpContextAccessor!.GetUserName(),
		//			IpAddressModified = _httpContextAccessor?.GetIpAddress(),
		//			Hide = false
		//		};
		//		modelList.Add(model);
		//	}
		//	_repository.UpdateRange(modelList);
		//	await _repository.SaveChanges();
		//	return true;
		//}
		//public async Task<bool> DeleteJournalDetails(int journalHeaderId, List<JournalDetailDto> detail)
		//{
		//	if (detail.Any())
		//	{
		//		var currentDetails = _repository.GetAll().Where(x => x.JournalHeaderId == journalHeaderId).AsNoTracking().ToList();
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
			try { id = await _repository.GetAll().MaxAsync(a => a.JournalDetailId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<List<JournalDetailDto>> GetJournalDetailByHeaderId(int headerId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
			  await (from journalDetail in _repository.GetAll().Where(x => x.JournalHeaderId == headerId)
					 from entityType in _entityTypeService.GetAll().Where(x => x.EntityTypeId == journalDetail.EntityTypeId).DefaultIfEmpty()
					 select new JournalDetailDto()
				{
					JournalDetailId = journalDetail.JournalDetailId,
					JournalHeaderId = journalDetail.JournalHeaderId,
					AccountId = journalDetail.AccountId,
					CurrencyId = journalDetail.CurrencyId,
					DebitValue = journalDetail.DebitValue,
					DebitValueAccount = journalDetail.DebitValueAccount,
					CreditValue = journalDetail.CreditValue,
					CreditValueAccount = journalDetail.CreditValueAccount,
					CurrencyRate = journalDetail.CurrencyRate,
					IsTax = journalDetail.IsTax,
					TaxId = journalDetail.TaxId,
					TaxTypeId = journalDetail.TaxTypeId,
					Serial = journalDetail.Serial,
					TaxCode = journalDetail.TaxCode,
					TaxDate = journalDetail.TaxDate,
					TaxParentId = journalDetail.TaxParentId,
					TaxPercent = journalDetail.TaxPercent,
					EntityId = journalDetail.EntityId,
					EntityTypeId = journalDetail.EntityTypeId,
					EntityTypeName = language == LanguageCode.Arabic ? entityType.EntityTypeNameAr : entityType.EntityTypeNameEn,
					EntityEmail = journalDetail.EntityEmail,
					EntityNameAr = journalDetail.EntityNameAr,
					EntityNameEn = journalDetail.EntityNameEn,
					IsCostCenterDistributed = journalDetail.IsCostCenterDistributed,
					IsLinkedToCostCenters = journalDetail.IsLinkedToCostCenters,
					IsSystematic = journalDetail.IsSystematic,
					IsCostRelated = journalDetail.IsCostRelated,
					RemarksAr = journalDetail.RemarksAr,
					RemarksEn = journalDetail.RemarksEn,
					AutomaticRemarks = journalDetail.AutomaticRemarks,
					TaxReference = journalDetail.TaxReference,
					CreatedAt = journalDetail.CreatedAt,
					IpAddressCreated = journalDetail.IpAddressCreated,
					UserNameCreated = journalDetail.UserNameCreated
				}).ToListAsync();
			return data;
		}

		public async Task<JournalDetailReturnDto> BuildJournalDetail(int journalHeaderId, int nextJournalDetailId,int serial, int? taxParentId, JournalDetailDto detailModel,bool hasCostCenter)
		{
			var model = new JournalDetail()
			{
				JournalDetailId = detailModel.JournalDetailId > 0 ? detailModel.JournalDetailId : nextJournalDetailId,
				JournalHeaderId = journalHeaderId,
				AccountId = detailModel.AccountId,
				CurrencyId = detailModel.CurrencyId,
				CurrencyRate = detailModel.CurrencyRate,
				DebitValue = detailModel.DebitValue,
				DebitValueAccount = detailModel.DebitValueAccount,
				CreditValue = detailModel.CreditValue,
				CreditValueAccount = detailModel.CreditValueAccount,
				Serial = detailModel.Serial > 0 ? detailModel.Serial : serial,
				EntityId = detailModel.EntityId,
				EntityTypeId = detailModel.EntityTypeId,
				EntityNameAr = detailModel.EntityNameAr,
				EntityNameEn = detailModel.EntityNameEn,
				TaxCode = detailModel.TaxCode,
				EntityEmail = detailModel.EntityEmail,
				RemarksAr = detailModel.RemarksAr,
				RemarksEn = detailModel.RemarksEn,
				AutomaticRemarks = detailModel.AutomaticRemarks,
				IsTax = detailModel.IsTax,
				TaxId = detailModel.TaxId,
				TaxTypeId = detailModel.TaxTypeId,
				TaxPercent = detailModel.TaxPercent,
				TaxParentId = taxParentId,
				TaxReference = detailModel.TaxReference,
				TaxDate = detailModel.TaxDate,
				CreatedAt = detailModel.JournalDetailId <= 0 ? DateHelper.GetDateTimeNow() : detailModel.CreatedAt,
				UserNameCreated = detailModel.JournalDetailId <= 0 ? await _httpContextAccessor!.GetUserName() : detailModel.UserNameCreated,
				IpAddressCreated = detailModel.JournalDetailId <= 0 ? _httpContextAccessor?.GetIpAddress() : detailModel.IpAddressCreated,
				ModifiedAt = detailModel.JournalDetailId > 0 ? DateHelper.GetDateTimeNow() : null,
				UserNameModified = detailModel.JournalDetailId > 0 ? await _httpContextAccessor!.GetUserName() : null ,
				IpAddressModified = detailModel.JournalDetailId > 0 ? _httpContextAccessor?.GetIpAddress() : null,
				Hide = false,
				IsCostCenterDistributed = detailModel.IsCostCenterDistributed,
				IsSystematic = detailModel.IsSystematic,
				IsCostRelated = detailModel.IsCostRelated,
				IsLinkedToCostCenters = hasCostCenter
			};
			return new JournalDetailReturnDto() { Mode = detailModel.JournalDetailId > 0 ? RequestMode.Update : RequestMode.Create, JournalDetail = model };
		}

		public async Task<List<JournalDetail>> HandleDeletedJournalDetails(int journalHeaderId, List<JournalDetailDto>? journalDetail)
		{
			if (journalDetail != null)
			{
				var detailsDb = await _repository.GetAll().Where(x => x.JournalHeaderId == journalHeaderId).AsNoTracking().ToListAsync();
				var detailsToBeDeleted = detailsDb.Where(p => journalDetail.All(p2 => p2.JournalDetailId != p.JournalDetailId)).ToList();
				return detailsToBeDeleted;
			}
			return new List<JournalDetail>();
		}

		public async Task<bool> CreateJournalDetail(List<JournalDetail> journalDetails)
		{
			if (journalDetails.Any())
			{
				await _repository.InsertRange(journalDetails);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> UpdateJournalDetail(List<JournalDetail> journalDetails)
		{
			if (journalDetails.Any())
			{
				_repository.UpdateRange(journalDetails);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> DeleteJournalDetail(List<JournalDetail> journalDetails)
		{
			if (journalDetails.Any())
			{
				_repository.RemoveRange(journalDetails);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> DeleteJournalDetail(int journalHeaderId)
		{
			var data = await  _repository.GetAll().Where(x => x.JournalHeaderId == journalHeaderId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
			}
			return true;
		}
	}
}
