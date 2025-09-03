using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
    public static class BasicData
    {
        public enum SystemTask
        {
            ImportCountries = 1,
            ImportStates = 2,
            ImportCities = 3,
            ImportDistricts = 4,
            ImportCurrencies = 5
        }
    }
}
