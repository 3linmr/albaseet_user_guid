using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public class AccountTypeData
	{
		public const byte Cash = 1;
		public const byte Clients = 2;
		public const byte Suppliers = 3;
		public const byte Banks = 4;
		public const byte FractionalApproximationDifference = 5;
		public const byte FixedAssets = 6;
		public const byte AccumulatedDepreciation = 7;
		public const byte Depreciation = 8;
		public const byte OwnershipEquity = 9;
		public const byte Purchases = 10;
		public const byte RevenuesCost = 11;
		public const byte Sales = 12;
		public const byte MiscellaneousExpenses =13;
		public const byte MiscellaneousIncome = 14;
		public const byte AllowedDiscount = 15;
		public const byte Inventory = 16;
		public const byte InventoryAccount = 17;
		public const byte RevenuesCostAccount = 18;
	}
}
