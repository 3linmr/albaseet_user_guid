using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Helper.Data;
using Shared.Helper.Models.UserDetail;
using System.Text;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;

namespace Accounting.Service.Extensions
{
    public static class AccountingAPI
    {
        private static IConfiguration? _configuration;

        public static void AccountingAPIConfigure(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //public static async Task<string> AccountingPost(string url,object model)
        //{
        //	var accounting = _configuration?.GetSection("Application:AccountingAPI").Value;
        //	var link = $"{accounting}/api{url}";
        //	return await ApiHelper.PostAPI(link, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
        //}
    }
}
