using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Archive;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Archive;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;

namespace Shared.Service.Services.Settings
{
	public class ApplicationFlagDetailImageService : BaseService<ApplicationFlagDetailImage>, IApplicationFlagDetailImageService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IBlobImageService _blobImageService;

		public ApplicationFlagDetailImageService(IRepository<ApplicationFlagDetailImage> repository,IHttpContextAccessor httpContextAccessor,IBlobImageService blobImageService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_blobImageService = blobImageService;
		}

		public IQueryable<ApplicationFlagDetailImageDto?> GetApplicationFlagImages()
		{
			return _repository.GetAll().Select(x => new ApplicationFlagDetailImageDto
			{
				ApplicationFlagDetailCompanyId = x.ApplicationFlagDetailCompanyId,
				ApplicationFlagDetailImageId = x.ApplicationFlagDetailImageId,
				FileName = x.FileName,
				FileType = x.FileType,
				ImageBinary = x.Image,
				UserNameCreated = x.UserNameCreated,
				CreatedAt = x.CreatedAt
			});
		}

		public async Task<ApplicationFlagDetailImageDto?> GetApplicationFlagImage(int applicationFlagDetailImageId)
		{
			return await GetApplicationFlagImages().FirstOrDefaultAsync(x => x != null && x.ApplicationFlagDetailImageId == applicationFlagDetailImageId);
		}

		public async Task<bool> SaveApplicationFlagImage(ApplicationFlagDetailImageDto model)
		{
			var image = _blobImageService.UploadImageAsync(model.Image!);
			await DeleteImage(model.ApplicationFlagDetailCompanyId);
			var modelDb = new ApplicationFlagDetailImage
			{
				ApplicationFlagDetailImageId = await GetNextId(),
				ApplicationFlagDetailCompanyId = model.ApplicationFlagDetailCompanyId,
				FileName = image.FileName,
				FileType = image.ContentType,
				Image = image.Content,
				UserNameCreated = await _httpContextAccessor.GetUserName(),
				CreatedAt = DateHelper.GetDateTimeNow(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
			};
			await _repository.Insert(modelDb);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<bool> DeleteImage(int applicationFlagDetailCompanyId)
		{
			var data = await _repository.GetAll().Where(x=>x.ApplicationFlagDetailCompanyId == applicationFlagDetailCompanyId).ToListAsync();
			_repository.RemoveRange(data);
			await _repository.SaveChanges();
			return true;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ApplicationFlagDetailImageId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
