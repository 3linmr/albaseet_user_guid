using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public static class TaxTypeData
	{
		public const byte Taxable = 1;
		public const byte Exempted = 2;
		public const byte Zero = 3;
		public const byte PrivateContracts = 4;
		public const byte Exports = 5;
		public const byte Imports = 6;
		public const byte ReverseCalculation = 7;
	}
}
