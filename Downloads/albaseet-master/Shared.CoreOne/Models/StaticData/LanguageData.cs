using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
    public static class LanguageData
    {
       public enum Languages
        {
            Arabic = 1,
            English = 2
        }

        public static class LanguageCode
        {
            public const string Arabic = "ar";
            public const string English = "en";
        }
    }
}
