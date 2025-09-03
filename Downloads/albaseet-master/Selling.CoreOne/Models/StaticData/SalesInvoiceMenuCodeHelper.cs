using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Models.StaticData
{
	public static class SalesInvoiceMenuCodeHelper
	{
		public static int GetMenuCode(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				return MenuCodeData.SalesInvoiceInterim;
			}
			else
			{
				if (isOnTheWay)
				{
					return isCreditPayment ? MenuCodeData.CreditReservationInvoice : MenuCodeData.CashReservationInvoice;
				}
				else
				{
					return isCreditPayment ? MenuCodeData.CreditSalesInvoice : MenuCodeData.CashSalesInvoice;
				}
			}
		}

		public static int GetMenuCode(SalesInvoiceHeaderDto salesInvoiceHeader)
		{
			return GetMenuCode(salesInvoiceHeader.IsOnTheWay, salesInvoiceHeader.IsDirectInvoice, salesInvoiceHeader.CreditPayment);
		}

		public static string GetDocumentReference(bool isOnTheWay, bool isDirectInvoice, bool isCreditPayment)
		{
			if (!isDirectInvoice)
			{
				return DocumentReferenceData.SalesInvoiceInterim;
			}
			else
			{
				if (isOnTheWay)
				{
					return isCreditPayment ? DocumentReferenceData.CreditReservationInvoice : DocumentReferenceData.CashReservationInvoice;
				}
				else
				{
					return isCreditPayment ? DocumentReferenceData.CreditSalesInvoice : DocumentReferenceData.CashSalesInvoice;
				}
			}
		}

		public static string GetDocumentReference(SalesInvoiceHeaderDto salesInvoiceHeader)
		{
			return GetDocumentReference(salesInvoiceHeader.IsOnTheWay, salesInvoiceHeader.IsDirectInvoice, salesInvoiceHeader.CreditPayment);
		}
	}
}
