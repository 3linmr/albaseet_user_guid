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
	public static class PurchaseInvoiceMenuCodeHelper
	{
		public static int GetMenuCode(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				return MenuCodeData.PurchaseInvoiceInterim;
			}
			else
			{
				if (isOnTheWay)
				{
					return isCreditPayment ? MenuCodeData.PurchaseInvoiceOnTheWayCredit : MenuCodeData.PurchaseInvoiceOnTheWayCash;
				}
				else
				{
					return isCreditPayment ? MenuCodeData.CreditPurchaseInvoice : MenuCodeData.CashPurchaseInvoice;
				}
			}
		}

		public static int GetMenuCode(PurchaseInvoiceHeaderDto purchaseInvoiceHeader)
		{
			return GetMenuCode(purchaseInvoiceHeader.IsOnTheWay, purchaseInvoiceHeader.IsDirectInvoice, purchaseInvoiceHeader.CreditPayment);
		}

		public static string GetDocumentReference(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				return DocumentReferenceData.PurchaseInvoiceInterim;
			}
			else
			{
				if (isOnTheWay)
				{
					return isCreditPayment ? DocumentReferenceData.PurchaseInvoiceOnTheWayCredit : DocumentReferenceData.PurchaseInvoiceOnTheWayCash;
				}
				else
				{
					return isCreditPayment ? DocumentReferenceData.CreditPurchaseInvoice : DocumentReferenceData.CashPurchaseInvoice;
				}
			}
		}

		public static string GetDocumentReference(PurchaseInvoiceHeaderDto purchaseInvoiceHeader)
		{
			return GetDocumentReference(purchaseInvoiceHeader.IsOnTheWay, purchaseInvoiceHeader.IsDirectInvoice, purchaseInvoiceHeader.CreditPayment);
		}
	}
}
