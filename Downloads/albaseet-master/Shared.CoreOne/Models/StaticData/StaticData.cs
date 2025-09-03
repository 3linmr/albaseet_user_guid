using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public static class StaticData
	{
		public class RequestMode
		{
			public const string Create = "Create";
			public const string Update = "Update";
			public const string Delete = "Delete";
		}

		public static class SellerCommissionTypeData
		{
			public const byte CashIncome = 1;
			public const byte AgeOfDebt = 2;
		}

		public static class AccountCategoryData
		{
			public const byte Assets = 1;
			public const byte Liabilities = 2;
			public const byte Expenses = 3;
			public const byte Revenues = 4;
		}

		public static class ItemSearchFromWhere
		{
			public static byte BarCode = 1;
			public static byte ItemId = 2;
			public static byte ItemCode = 3;
			public static byte ItemName = 4;
			public static byte PackageChange = 5;
		}

		public static class CurrencyData
		{
			public static short SaudiRiyal = 130;
		}
	}

}
