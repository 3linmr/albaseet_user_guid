using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shared.Helper.Data;
using Shared.Helper.Models.UserDetail;
using Microsoft.Extensions.Configuration;
using Shared.Helper.Database;
using Shared.Helper.Models.StaticData;
using static Newtonsoft.Json.JsonConvert;
using System.ComponentModel.Design;
using Shared.Helper.Models.Dtos;



namespace Shared.Helper.Identity
{
	public static class IdentityHelper
	{
		private static IConfiguration? _configuration;
		private static IHttpContextAccessor? _httpContextAccessor;
		private const int ApplicationId = ApplicationData.ApplicationId;
		private const int StoreIdentifier = ApplicationIdentifierData.Store;
		private const int BusinessIdentifier = ApplicationIdentifierData.Business;

		public static void IdentityHelperConfigure(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

		public static string? GetUserId(this IHttpContextAccessor httpContextAccessor)
		{
			var claims = httpContextAccessor?.HttpContext?.User?.Claims?.ToList();
			var claimsData = claims?.Select(x => new IdentityClaimsDto
			{
				Value = x.Value,
				ValueType = x.Type,
				Issuer = x.Issuer
			}).ToList();
			var userId = claimsData?.Where(x => x.ValueType!.Contains("nameidentifier")).Select(x => x.Value).FirstOrDefault();
			return userId;
		}

		public static async Task<string?> GetUserName(this IHttpContextAccessor httpContextAccessor)
		{
			var userName = httpContextAccessor?.HttpContext?.Request.Headers["preferred_username"].FirstOrDefault();
			if (String.IsNullOrEmpty(userName))
			{
				if (httpContextAccessor != null)
				{
					var userId = GetUserId(httpContextAccessor);
					if (userId != null)
					{
						return await GetUserNameFromDb(userId);
					}
				}
			}
			return userName;
		}

		public static async Task<bool> UserHasRight(this IHttpContextAccessor httpContextAccessor, int flagId, int storeId)
		{
			return await UserCanDo(httpContextAccessor, flagId);
		}

		public static async Task<bool> UserCanLook(this IHttpContextAccessor httpContextAccessor, int companyId, int storeId)
		{
			var userId = httpContextAccessor.GetUserId();
			if (userId != null)
			{
				if (companyId > 0)
				{
					var userCompanies = await httpContextAccessor.GetUserCompanies();
					return userCompanies.Contains(companyId);
				}
				if (storeId > 0)
				{
					var userStores = await httpContextAccessor.GetUserStores();
					return userStores.Contains(storeId);
				}
			}
			return false;
		}

		public static int GetCurrentUserCompany(this IHttpContextAccessor httpContextAccessor)
		{
			return Convert.ToInt32(httpContextAccessor?.HttpContext?.Request.Headers["companyId"].FirstOrDefault());
		}

		public static int GetCurrentUserBranch(this IHttpContextAccessor httpContextAccessor)
		{
			return Convert.ToInt32(httpContextAccessor?.HttpContext?.Request.Headers["branchId"].FirstOrDefault());
		}

		public static int GetCurrentUserStore(this IHttpContextAccessor httpContextAccessor)
		{
			return Convert.ToInt32(httpContextAccessor?.HttpContext?.Request.Headers["storeId"].FirstOrDefault());
		}

		public static async Task<string> GetUserConnectionString(int? companyId)
		{
			var isAuthenticated = _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated;
			if (isAuthenticated == true)
			{
				var userCompany = await GetCompanyId(companyId);
				var connectionString = GetConnectionString();
				if (connectionString != null)
				{
					string[] words = connectionString.Split(';');
					return string.Join(" ", words.Select((word, index) => index == 3 ? $"database=albaseet{userCompany};" : $"{word};"));
				}
				return "";
			}
			else
			{
				return "";
			}
		}

		public static async Task<string?> GetCompanyId(int? companyId)
		{
			if (companyId.GetValueOrDefault(0) > 0)
			{
				return companyId.ToString();
			}
			else
			{
				var company = _httpContextAccessor?.HttpContext?.Request.Headers["company"].FirstOrDefault();
				if (String.IsNullOrEmpty(company))
				{
					if (_httpContextAccessor != null)
					{
						var userId = _httpContextAccessor.GetUserId();
						if (userId != null)
						{
							return await GetUserCompany(userId);
						}
					}
				}
				return company;
			}

		}

		public static async Task<string> GetUserNameFromDb(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var link = $"{admin}/api/AdminAPI/GetUserName?userId={userId}";
			return await ApiHelper.GetAPI(link);
		}

		public static string? GetConnectionString()
		{
			return _configuration?.GetConnectionString("albaseet");
		}
		public static async Task<string> GetUserCompany(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var link = $"{admin}/api/AdminAPI/GetUserCompany?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<string>(apiResponse) ?? "";
			return data;
		}

		public static async Task<int?> GetEmployeeId(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var link = $"{admin}/api/AdminAPI/GetEmployeeId?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<int?>(apiResponse);
			return data;
		}
		public static async Task<int?> GetUserLanguageId(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var link = $"{admin}/api/AdminAPI/GetUserLanguageId?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<int?>(apiResponse);
			return data;
		}

		public static string GetProgramCurrentLanguage(this IHttpContextAccessor httpContextAccessor)
		{
			var language = httpContextAccessor?.HttpContext?.Request.Headers["Accept-Language"].FirstOrDefault();
			if (language != null)
			{
				if (language?.Trim() == "ar" || (bool)language?.Trim().Contains("ar"))
				{
					return "ar";
				}
				else if (language?.Trim() == "en" || (bool)language?.Trim().Contains("en"))
				{
					return "en";
				}
				else
				{
					return "en";
				}
			}
			return "en";
		}

		public static async Task<SubscriptionBusinessCountDto> GetSubscriptionBusinessCount()
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var tenantId = Convert.ToInt32(await GetCompanyId(0));
			var link = $"{admin}/api/AdminAPI/GetSubscriptionBusinessCount?applicationId={ApplicationData.ApplicationId}&companyId={tenantId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<SubscriptionBusinessCountDto>(apiResponse);
			return data ?? new SubscriptionBusinessCountDto();
		}
		
		public static async Task<List<int>> GetUserCompanies(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var link = $"{admin}/api/AdminAPI/GetUserCompanies?userId={userId}&applicationId={ApplicationId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<int>>(apiResponse);
			return data ?? new List<int>();
		}
		public static async Task<List<int>> GetUserBranches(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var link = $"{admin}/api/AdminAPI/GetUserBranches?userId={userId}&applicationId={ApplicationId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<int>>(apiResponse);
			return data ?? new List<int>();
		}

		public static async Task<List<int>> GetUserStores(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var link = $"{admin}/api/AdminAPI/GetUserStores?userId={userId}&applicationId={ApplicationId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<int>>(apiResponse);
			return data ?? new List<int>();
		}

		public static async Task<List<int>> GetUserApplications(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var link = $"{admin}/api/AdminAPI/GetUserApplications?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<int>>(apiResponse);
			return data ?? new List<int>();
		}

		public static async Task<List<int>> GetUserSteps(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var companyId = _httpContextAccessor?.GetCurrentUserCompany();
			var link = $"{admin}/api/AdminAPI/GetUserSteps?userId={userId}&applicationId={ApplicationId}&applicationIdentifierId={BusinessIdentifier}&applicationIdentifierValue={companyId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<int>>(apiResponse);
			return data ?? new List<int>();
		}

		public static async Task<List<MenuDto>> GetUserMenus(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/GetUserMenus?userId={userId}&applicationId={ApplicationId}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			if (!string.IsNullOrWhiteSpace(apiResponse))
			{
				var data = JsonConvert.DeserializeObject<List<MenuDto>>(apiResponse);
				return data ?? new List<MenuDto>();
			}
			else
			{
				return new List<MenuDto>();
			}
		}

		public static async Task<List<int>> GetMenuUserFlags(this IHttpContextAccessor httpContextAccessor, int menuCode)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value; ;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/GetUserFlags?userId={userId}&applicationId={ApplicationId}&menuCode={menuCode}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<int>>(apiResponse) ?? [];
			return data;
		}

		public static async Task<List<UserFlagDto>> GetUserFlagsValues(this IHttpContextAccessor httpContextAccessor, int menuCode)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value; ;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/GetUserFlagsValues?userId={userId}&applicationId={ApplicationId}&menuCode={menuCode}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<List<UserFlagDto>>(apiResponse) ?? [];
			return data;
		}

		public static async Task<bool> UserCanRoute(this IHttpContextAccessor httpContextAccessor, string routingUrl)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var companyId = await GetCompanyId(null);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/UserCanRoute?applicationId={ApplicationId}&userId={userId}&companyId={companyId}&routingUrl={routingUrl}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<bool>(apiResponse);
			return data;
		}

		public static async Task<UserDataDto> GetUserData(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/GetUserData?userId={userId}&applicationId={ApplicationId}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<UserDataDto>(apiResponse);
			return data ?? new UserDataDto();
		}

		public static async Task<UserBasicInfoDto> GetUserBasicData(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var link = $"{admin}/api/AdminAPI/GetUserBasicData?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<UserBasicInfoDto>(apiResponse);
			return data ?? new UserBasicInfoDto();
		}

		public static async Task<bool> ChangeUserLanguage(this IHttpContextAccessor httpContextAccessor, string languageCode)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var link = $"{admin}/api/AdminAPI/ChangeUserLanguage";
			var model = new UserLanguageDto { UserId = userId, LanguageCode = languageCode };
			var apiResponse = await ApiHelper.PostAPI(link, model);
			var data = JsonConvert.DeserializeObject<bool>(apiResponse);
			return data;
		}

		public static async Task<bool> UserHasAccess(this IHttpContextAccessor httpContextAccessor, string? pinCode, int flagId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/UserHasAccess?applicationId={ApplicationId}&userId={userId}&pinCode={pinCode}&flagId={flagId}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<bool>(apiResponse);
			return data;
		}

		public static async Task<bool> UserCanDo(this IHttpContextAccessor httpContextAccessor, int flag)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/UserCanDo?applicationId={ApplicationId}&userId={userId}&flagId={flag}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<bool>(apiResponse);
			return data;
		}

		public static async Task<string> GetUserFlagValue(this IHttpContextAccessor httpContextAccessor, int flag)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = GetUserId(httpContextAccessor);
			var storeId = _httpContextAccessor?.GetCurrentUserStore();
			var link = $"{admin}/api/AdminAPI/GetUserFlagValue?applicationId={ApplicationId}&userId={userId}&flagId={flag}&applicationIdentifierId={StoreIdentifier}&applicationIdentifierValue={storeId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<string>(apiResponse);
			return data ?? "";
		}

		public static async Task<bool> IsUserValid(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var companyId = await GetCompanyId(null);
			var link = $"{admin}/api/AdminAPI/IsUserValid?userId={userId}&applicationId={ApplicationId}&companyId={companyId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<bool>(apiResponse);
			return data;
		}
		
		public static async Task<UserValidDto> UserCanProceed(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var companyId = await GetCompanyId(null);
			var userId = httpContextAccessor.GetUserId();
			var link = $"{admin}/api/AdminAPI/UserCanProceed?userId={userId}&applicationId={ApplicationId}&companyId={companyId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<UserValidDto>(apiResponse);
			return data ?? new UserValidDto();
		}
		
		public static async Task<UserValidDto> IsUserOk(this IHttpContextAccessor httpContextAccessor)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var userId = httpContextAccessor.GetUserId();
			var link = $"{admin}/api/AdminAPI/IsUserOk?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<UserValidDto>(apiResponse);
			return data ?? new UserValidDto();
		}

		public static async Task<bool> IsSystemAdminUser(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var link = $"{admin}/api/AdminAPI/IsSystemAdminUser?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<bool>(apiResponse);
			return data;
		}

		public static async Task<UserRoleDto?> GetUserRole(string userId)
		{
			var admin = _configuration?.GetSection("Application:AdminAPI").Value;
			var link = $"{admin}/api/AdminAPI/GetUserRole?userId={userId}";
			var apiResponse = await ApiHelper.GetAPI(link);
			var data = JsonConvert.DeserializeObject<UserRoleDto>(apiResponse);
			return data;
		}
		
		public static async Task<ResponseDto> ImportAllTasks(int companyId)
		{
			var sharedApi = _configuration?.GetSection("Application:SharedAPI").Value;
			var link = $"{sharedApi}/api/SystemTasks/ImportAll?companyId={companyId}";
			var apiResponse = await ApiHelper.PostAPI(link, null);
			var data = JsonConvert.DeserializeObject<ResponseDto>(apiResponse) ?? new ResponseDto();
			return data;
		}

		public static string? GetToken(this IHttpContextAccessor httpContextAccessor)
		{
			return httpContextAccessor?.HttpContext?.Request.Headers.ToList().Where(x => x.Key.Contains("Authorization")).Select(s => s.Value).FirstOrDefault().Select(s => s?.Split(" ", 7)).Take(1).FirstOrDefault()?[1];
		}

		public static string? GetMachineName()
		{
			return Environment.MachineName;
		}
		public static string? GetIpAddress(this IHttpContextAccessor httpContextAccessor)
		{
			return httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
		}


	}
}
