using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Models.StaticData
{
	public static class PurchaseInvoiceReturnMenuCodeHelper
	{
		public static int GetMenuCode(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				if (isOnTheWay)
				{
					return MenuCodeData.PurchaseInvoiceReturnOnTheWay;
				}
				else
				{
					return MenuCodeData.PurchaseInvoiceReturn;
				}
			}
			else
			{
				return isCreditPayment ? MenuCodeData.CreditPurchaseInvoiceReturn : MenuCodeData.CashPurchaseInvoiceReturn;
			}
		}

		public static int GetMenuCode(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader)
		{
			return GetMenuCode(purchaseInvoiceReturnHeader.IsOnTheWay, purchaseInvoiceReturnHeader.IsDirectInvoice, purchaseInvoiceReturnHeader.CreditPayment);
		}

		public static string GetDocumentReference(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				if (isOnTheWay)
				{
					return DocumentReferenceData.PurchaseInvoiceReturnOnTheWay;
				}
				else
				{
					return DocumentReferenceData.PurchaseInvoiceReturn;
				}
			}
			else
			{
				return isCreditPayment ? DocumentReferenceData.CreditPurchaseInvoiceReturn : DocumentReferenceData.CashPurchaseInvoiceReturn;
			}
		}

		public static string GetDocumentReference(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader)
		{
			return GetDocumentReference(purchaseInvoiceReturnHeader.IsOnTheWay, purchaseInvoiceReturnHeader.IsDirectInvoice, purchaseInvoiceReturnHeader.CreditPayment);
		}
	}
}
