using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public static class JournalTypeData
	{
		public const byte OpeningBalance = 1; //رصيد افتتاحي
		public const byte CashSalesInvoice = 2;  //فاتورة بيع نقداً
		public const byte CreditSalesInvoice = 3; //فاتورة بيع آجل
		public const byte CashReturnInvoice = 4; //فاتورة مرتجع بيع نقداً
		public const byte CreditSalesReturnInvoice = 5;  //فاتورة مرتجع بيع آجل
		public const byte CashPurchaseInvoice = 6;  //فاتورة شراء نقداً
		public const byte CreditPurchaseInvoice = 7; //فاتورة شراء آجل
		public const byte CashPurchaseReturnInvoice = 8;  //فاتورة مرتجع شراء نقداً
		public const byte CreditPurchaseReturnInvoice = 9;  //فاتورة مرتجع شراء آجل
		public const byte CheckCashingVoucher = 10;  //سند صرف شيكات
		public const byte JournalEntry = 11;  //قيد يومية
		public const byte ReceiptVoucher = 12;  //سند قبض
		public const byte RentReceiptVoucher = 13; //سند قبض إيجار
		public const byte PaymentVoucher = 14; //سند صرف
		public const byte CashingInvoice = 15;  //فاتورة صرف
		public const byte ReceiptInvoice = 16;  //فاتورة استلام
		public const byte RentEntry = 17; //قيد ايجار
		public const byte SupplierDebitNotice = 18; //إشعار مدين المورد
        public const byte SupplierCreditNotice = 19; //إشعار دائن المورد
        public const byte ClientDebitNotice = 20; //إشعار مدين للعميل
        public const byte ClientCreditNotice = 21; //إشعار دائن للعميل
		public const byte FixedAssetVoucher = 22;//قيد الاصول الثابتة
		public const byte PurchaseInvoiceOnTheWayCash = 23;//فاتورة شراء بضاعة بالطريق نقدي
		public const byte PurchaseInvoiceOnTheWayCredit = 24;//فاتورة شراء بضاعة بالطريق آجل
		public const byte PurchaseInvoiceInterim = 25;//فاتورة شراء مرحلية
		public const byte SalesInvoiceReservationCash = 26;//فاتورة بيع حجز نقدي
		public const byte SalesInvoiceReservationCredit = 27;//فاتورة بيع حجز آجل
		public const byte SalesInvoiceInterim = 28;//فاتورة بيع مرحلية
		public const byte PurchaseInvoiceReturnOnTheWay = 29;//فاتورة مرتجع مشتريات بضاعة في الطريق
		public const byte PurchaseInvoiceReturn = 30;//فاتورة مرتجع ما بعد الشراء
		public const byte SalesInvoiceReturnReservationCloseOut = 31;//فاتورة مرتجع بيع - تصفية حجز
		public const byte SalesInvoiceReturn = 32;//فاتورة مرتجع ما بعد البيع


	}
}
