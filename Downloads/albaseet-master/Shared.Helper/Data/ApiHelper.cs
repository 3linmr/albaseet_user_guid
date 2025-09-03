using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Helper.Identity;

namespace Shared.Helper.Data
{
    public static class ApiHelper
    {
        private static readonly IHttpContextAccessor HttpContextAccessor = new HttpContextAccessor();
        private static IConfiguration? _configuration;

        public static void HelperAPIConfigure(IConfiguration configuration)
        {
	        _configuration = configuration;
        }

		public static async Task<string> GetAPI(string link)
		{
			var httpClient = new HttpClient();
			var language = HttpContextAccessor.GetProgramCurrentLanguage();
			var token = HttpContextAccessor.GetToken();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
			httpClient.Timeout = TimeSpan.FromMinutes(10);
			var response = await httpClient.GetAsync(link);
			return await response.Content.ReadAsStringAsync();
		}
		public static async Task<string> PostAPI(string link, object? model)
		{
			var httpClient = new HttpClient();
			var token = HttpContextAccessor.GetToken();
			var language = HttpContextAccessor.GetProgramCurrentLanguage();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(language));
			httpClient.Timeout = TimeSpan.FromMinutes(20);
			var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(link, stringContent);
			return await response.Content.ReadAsStringAsync();
		}
	}
}
