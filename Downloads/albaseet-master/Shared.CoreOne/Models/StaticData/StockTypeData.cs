using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public class StockTypeData
	{
		public const byte ReceiptStatement = 1;
		public const byte ReceiptFromPurchaseInvoiceOnTheWay = 2;
		public const byte ReceiptStatementReturn = 3;
		public const byte ReceiptFromPurchaseInvoiceOnTheWayReturn = 4;
		public const byte ReturnFromPurchaseInvoice = 5;
        public const byte StockOutFromProformaInvoice = 6;
        public const byte StockOutFromSalesInvoice = 7;
        public const byte StockOutReturnFromStockOut = 8;
        public const byte StockOutReturnFromInvoicedStockOut = 9;
        public const byte StockOutReturnFromSalesInvoice = 10;

        public static short ToMenuCode(byte stockTypeId)
		{
            return stockTypeId switch
            {
                ReceiptStatement => MenuCodeData.ReceiptStatement,
                ReceiptFromPurchaseInvoiceOnTheWay => MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay,
                ReceiptStatementReturn => MenuCodeData.ReceiptStatementReturn,
                ReceiptFromPurchaseInvoiceOnTheWayReturn => MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWayReturn,
                ReturnFromPurchaseInvoice => MenuCodeData.ReturnFromPurchaseInvoice,

                StockOutFromProformaInvoice => MenuCodeData.StockOutFromProformaInvoice,
                StockOutFromSalesInvoice => MenuCodeData.StockOutFromReservation,
                StockOutReturnFromStockOut => MenuCodeData.StockOutReturnFromStockOut,
                StockOutReturnFromInvoicedStockOut => MenuCodeData.StockOutReturnFromReservation,
                StockOutReturnFromSalesInvoice => MenuCodeData.StockOutReturnFromInvoice,
                _ => 0
            };
		}

        public static string? ToDocumentReference(byte stockTypeId)
        {
            return stockTypeId switch
            {
                ReceiptStatement => DocumentReferenceData.ReceiptStatement,
                ReceiptFromPurchaseInvoiceOnTheWay => DocumentReferenceData.ReceiptFromPurchaseInvoiceOnTheWay,
                ReceiptStatementReturn => DocumentReferenceData.ReceiptStatementReturn,
                ReceiptFromPurchaseInvoiceOnTheWayReturn => DocumentReferenceData.ReceiptFromPurchaseInvoiceOnTheWayReturn,
                ReturnFromPurchaseInvoice => DocumentReferenceData.ReturnFromPurchaseInvoice,

                StockOutFromProformaInvoice => DocumentReferenceData.StockOutFromProformaInvoice,
                StockOutFromSalesInvoice => DocumentReferenceData.StockOutFromReservation,
                StockOutReturnFromStockOut => DocumentReferenceData.StockOutReturnFromStockOut,
                StockOutReturnFromInvoicedStockOut => DocumentReferenceData.StockOutReturnFromReservation,
                StockOutReturnFromSalesInvoice => DocumentReferenceData.StockOutReturnFromSalesInvoice,
                _ => null
            };
        }
    }
}