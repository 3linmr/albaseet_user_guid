using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Logic.Tree
{
	public static class TreeLogic
	{
		public static string GenerateNextCode(List<string?> codes, string mainCode, bool isMain, string mainLength, string individualLength, int nextCode)
		{
			while (true)
			{
				var code = "";
				if (isMain)
				{
					code = mainCode + nextCode.ToString($"D{mainLength}");
				}
				else
				{
					code = mainCode + nextCode.ToString($"D{individualLength}");
				}

				var isExist = IsNextCodeExist(codes, code);
				if (isExist)
				{
					nextCode++;
				}
				else
				{
					return code;
				}
			}
		}

		public static bool IsNextCodeExist(List<string?> codes, string code)
		{
			return codes.Any(x => x == code);
		}
	}
}
