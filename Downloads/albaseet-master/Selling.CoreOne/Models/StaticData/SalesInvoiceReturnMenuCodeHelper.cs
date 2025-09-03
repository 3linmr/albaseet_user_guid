using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Models.StaticData
{
	public interface SalesInvoiceReturnMenuCodeHelper
	{
		public static int GetMenuCode(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				if (isOnTheWay)
				{
					return MenuCodeData.ReservationInvoiceCloseOut;
				}
				else
				{
					return MenuCodeData.SalesInvoiceReturn;
				}
			}
			else
			{
				return isCreditPayment ? MenuCodeData.CreditSalesInvoiceReturn : MenuCodeData.CashSalesInvoiceReturn;
			}
		}

		public static int GetMenuCode(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader)
		{
			return GetMenuCode(salesInvoiceReturnHeader.IsOnTheWay, salesInvoiceReturnHeader.IsDirectInvoice, salesInvoiceReturnHeader.CreditPayment);
		}

		public static string GetDocumentReference(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				if (isOnTheWay)
				{
					return DocumentReferenceData.ReservationInvoiceCloseOut;
				}
				else
				{
					return DocumentReferenceData.SalesInvoiceReturn;
				}
			}
			else
			{
				return isCreditPayment ? DocumentReferenceData.CreditSalesInvoiceReturn : DocumentReferenceData.CashSalesInvoiceReturn;
			}
		}

		public static string GetDocumentReference(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader)
		{
			return GetDocumentReference(salesInvoiceReturnHeader.IsOnTheWay, salesInvoiceReturnHeader.IsDirectInvoice, salesInvoiceReturnHeader.CreditPayment);
		}
	}
}
