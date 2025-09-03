using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToNormalizedString(this decimal num)
        {
            return (num / 1.000000000000000000000000000M).ToString();
        }
    }
}
