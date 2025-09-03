using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Settings
{
	public interface IZeroStockSettingService
	{
		Task<bool> GetZeroStockSettingByMenuCode(int menuCode, int storeId);
	}
}
