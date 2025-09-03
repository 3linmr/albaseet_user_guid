using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public static class PaymentTypeData
	{
		public const byte Cash = 1;
		public const byte BankAccount = 2;
		public const byte BankCard = 3;
		public const byte Installment = 4;
		public const byte CreditTransfer = 5;
	}
}
