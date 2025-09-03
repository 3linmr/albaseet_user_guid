using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Models.StaticData;

namespace Shared.Helper.Identity
{
    public static class LanguageHelper
    {
        public static string? GetSelectedLanguage(this IHttpContextAccessor httpContextAccessor)
        {
            var currentLanguage= httpContextAccessor?.HttpContext?.Request?.Headers?.AcceptLanguage.FirstOrDefault()?.Split(",")[0] ?? "ar";
            return currentLanguage;
        }
        public static int GetSelectedLanguageId(this IHttpContextAccessor httpContextAccessor)
        {
            var currentLanguage = GetSelectedLanguage(httpContextAccessor);
            return currentLanguage == "ar" ? (int)LanguageData.Languages.Arabic : currentLanguage == "en" ? (int)LanguageData.Languages.English : (int)LanguageData.Languages.Arabic;
        }
    }
}
