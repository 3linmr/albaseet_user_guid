using Microsoft.Extensions.Configuration;


namespace Purchases.Service.Extensions
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
