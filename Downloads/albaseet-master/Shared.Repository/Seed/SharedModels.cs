using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Inventory;

namespace Shared.Repository.Seed
{
	public static class SharedModels
	{
		public static void Seed(ModelBuilder builder)
		{
			//Task
			builder.Entity<SystemTask>().HasData(
				new SystemTask() { TaskId = 1, TaskNameAr = "استيراد الدول", TaskNameEn = "Import Countries" },
				new SystemTask() { TaskId = 2, TaskNameAr = "استيراد المناطق الإدارية", TaskNameEn = "Import States" },
				new SystemTask() { TaskId = 3, TaskNameAr = "استيراد المدن", TaskNameEn = "Import Cities" },
				new SystemTask() { TaskId = 4, TaskNameAr = "استيراد الأحياء", TaskNameEn = "Import Districts" },
				new SystemTask() { TaskId = 5, TaskNameAr = "استيراد العملات", TaskNameEn = "Import Currencies" }
			);

			//DocumentStatus
			builder.Entity<DocumentStatus>().HasData(
				new DocumentStatus { DocumentStatusId = 1, DocumentStatusNameAr = "تم إنشاء إتفاقية شراء", DocumentStatusNameEn = "Purchase order has been created" },
				new DocumentStatus { DocumentStatusId = 2, DocumentStatusNameAr = "تم إستلام الكمية جزئياً", DocumentStatusNameEn = "The quantity has been partially received" },
				new DocumentStatus { DocumentStatusId = 3, DocumentStatusNameAr = "تم إستلام الكمية بالكامل", DocumentStatusNameEn = "The entire quantity has been received" },
				new DocumentStatus { DocumentStatusId = 4, DocumentStatusNameAr = "تم عمل فاتورة المشتريات", DocumentStatusNameEn = "Purchase invoice has been created" },
				new DocumentStatus { DocumentStatusId = 5, DocumentStatusNameAr = "تم عمل فاتورة المشتريات لبعض البضائع", DocumentStatusNameEn = "Purchase invoice has been created for some of the items" },
				new DocumentStatus { DocumentStatusId = 6, DocumentStatusNameAr = "تم عمل فاتورة المشتريات لكل البضائع", DocumentStatusNameEn = "Purchase invoice has been created for all of the items" },
				new DocumentStatus { DocumentStatusId = 7, DocumentStatusNameAr = "تم عمل فاتورة المشتريات وفي انتظار الاستلام", DocumentStatusNameEn = "purchase invoice has been created and is awaiting receipt" },
				new DocumentStatus { DocumentStatusId = 8, DocumentStatusNameAr = "تم عمل مرتجع للكمية جزئياً وفي انتظار فاتورة مرتجع المشتريات", DocumentStatusNameEn = "The quantity has been partially returned and the purchase return invoice is awaited" },
				new DocumentStatus { DocumentStatusId = 9, DocumentStatusNameAr = "تم عمل مرتجع بالكمية بالكامل وفي انتظار فاتورة مرتجع المشتريات", DocumentStatusNameEn = "The entire quantity has been returned and the purchase return invoice is awaited" },
				new DocumentStatus { DocumentStatusId = 10, DocumentStatusNameAr = "تم عمل فاتورة مرتجع المشتريات", DocumentStatusNameEn = "Purchase return invoice has been created" },
				new DocumentStatus { DocumentStatusId = 11, DocumentStatusNameAr = "تم عمل فاتورة مرتجع المشتريات لبعض البضائع", DocumentStatusNameEn = "Purchase return invoice has been created for some of the items" },
				new DocumentStatus { DocumentStatusId = 12, DocumentStatusNameAr = "تم عمل فاتورة مرتجع المشتريات لكل البضائع", DocumentStatusNameEn = "Purchase return invoice has been created for all of the items" },

				new DocumentStatus { DocumentStatusId = 13, DocumentStatusNameAr = "تم إنشاء الفاتورة الأولية", DocumentStatusNameEn = "Proforma invoice has been created" },
				new DocumentStatus { DocumentStatusId = 14, DocumentStatusNameAr = "تم صرف الكمية جزئياً", DocumentStatusNameEn = "The quantity has been partially disbursed" },
				new DocumentStatus { DocumentStatusId = 15, DocumentStatusNameAr = "تم صرف الكمية بالكامل", DocumentStatusNameEn = "The entire quantity has been disbursed" },
				new DocumentStatus { DocumentStatusId = 16, DocumentStatusNameAr = "تم عمل فاتورة المبيعات", DocumentStatusNameEn = "Sales invoice has been created" },
				new DocumentStatus { DocumentStatusId = 17, DocumentStatusNameAr = "تم عمل فاتورة المبيعات لبعض البضائع", DocumentStatusNameEn = "Sales invoice has been created for some of the items" },
				new DocumentStatus { DocumentStatusId = 18, DocumentStatusNameAr = "تم عمل فاتورة المبيعات لكل البضائع", DocumentStatusNameEn = "Sales invoice has been created for all of the items" },
				new DocumentStatus { DocumentStatusId = 19, DocumentStatusNameAr = "تم عمل مرتجع للكمية المصروفة جزئياً وفي انتظار فاتورة مرتجع المبيعات", DocumentStatusNameEn = "The partially disbursed quantity has been returned and the sales return invoice is awaited" },
				new DocumentStatus { DocumentStatusId = 20, DocumentStatusNameAr = "تم عمل مرتجع للكمية المصروفة بالكامل وفي انتظار فاتورة مرتجع المبيعات", DocumentStatusNameEn = "The entire quantity disbursed has been returned and the sales return invoice is awaited" },
				new DocumentStatus { DocumentStatusId = 21, DocumentStatusNameAr = "تم عمل فاتورة مرتجع المبيعات", DocumentStatusNameEn = "Sales return invoice has been created" },
				new DocumentStatus { DocumentStatusId = 22, DocumentStatusNameAr = "تم عمل فاتورة مرتجع المبيعات لبعض البضائع", DocumentStatusNameEn = "Sales return invoice has been created for some of the items" },
				new DocumentStatus { DocumentStatusId = 23, DocumentStatusNameAr = "تم عمل فاتورة مرتجع المبيعات لكل البضائع", DocumentStatusNameEn = "Sales return invoice has been created for all of the items" },

				new DocumentStatus { DocumentStatusId = 24, DocumentStatusNameAr = "تم عمل إشعار مدين المورد", DocumentStatusNameEn = "Supplier debit memo has been created" },
				new DocumentStatus { DocumentStatusId = 25, DocumentStatusNameAr = "تم عمل إشعار دائن المورد", DocumentStatusNameEn = "Supplier credit memo has been created" },
				new DocumentStatus { DocumentStatusId = 26, DocumentStatusNameAr = "تم عمل إشعار مدين للعميل", DocumentStatusNameEn = "Client debit memo has been created" },
				new DocumentStatus { DocumentStatusId = 27, DocumentStatusNameAr = "تم عمل إشعار دائن للعميل", DocumentStatusNameEn = "Client credit memo has been created" }
			);

			//StoreClassification
			builder.Entity<StoreClassification>().HasData(
				new StoreClassification { StoreClassificationId = 1, ClassificationNameAr = "تجاري", ClassificationNameEn = "Commercial" },
				new StoreClassification { StoreClassificationId = 2, ClassificationNameAr = "صناعي", ClassificationNameEn = "Industrial" },
				new StoreClassification { StoreClassificationId = 3, ClassificationNameAr = "خدمي", ClassificationNameEn = "Services" },
				new StoreClassification { StoreClassificationId = 4, ClassificationNameAr = "إداري", ClassificationNameEn = "Managerial" }
			);

			//ColumnIdentifiers
			builder.Entity<ColumnIdentifier>().HasData(
				new ColumnIdentifier { ColumnIdentifierId = 1, ColumnIdentifierNameAr = "نص", ColumnIdentifierNameEn = "Text" },
				new ColumnIdentifier { ColumnIdentifierId = 2, ColumnIdentifierNameAr = "رقم", ColumnIdentifierNameEn = "Number" },
				new ColumnIdentifier { ColumnIdentifierId = 3, ColumnIdentifierNameAr = "تاريخ", ColumnIdentifierNameEn = "Date" },
				new ColumnIdentifier { ColumnIdentifierId = 4, ColumnIdentifierNameAr = "نعم/لا", ColumnIdentifierNameEn = "true/false" }
			);

			//EntityType
			builder.Entity<EntityType>().HasData(
				new EntityType { EntityTypeId = 1, EntityTypeNameAr = "مورد", EntityTypeNameEn = "Supplier",Order = 1},
				new EntityType { EntityTypeId = 2, EntityTypeNameAr = "عميل", EntityTypeNameEn = "Client" ,Order = 2},
				new EntityType { EntityTypeId = 3, EntityTypeNameAr = "مندوب مبيعات", EntityTypeNameEn = "Seller", Order = 3 },
				new EntityType { EntityTypeId = 4, EntityTypeNameAr = "بنك", EntityTypeNameEn = "Bank", Order = 4 }
			);

			//ItemType
			builder.Entity<ItemType>().HasData(
				new ItemType { ItemTypeId = 1, ItemTypeNameAr = "بضاعة", ItemTypeNameEn = "Goods" },
				new ItemType { ItemTypeId = 2, ItemTypeNameAr = "مجمع", ItemTypeNameEn = "Combined" },
				new ItemType { ItemTypeId = 3, ItemTypeNameAr = "مصنع", ItemTypeNameEn = "Manufactured" },
				new ItemType { ItemTypeId = 4, ItemTypeNameAr = "مركب", ItemTypeNameEn = "Assembled" },
				new ItemType { ItemTypeId = 5, ItemTypeNameAr = "خدمة", ItemTypeNameEn = "Service" },
				new ItemType { ItemTypeId = 6, ItemTypeNameAr = "ملاحظة", ItemTypeNameEn = "Note" }
			);

			//ShipmentType
			builder.Entity<ShipmentType>().HasData(
				new ShipmentType { ShipmentTypeId = 1, ShipmentTypeCode = 1, ShipmentTypeNameAr = "سيارة", ShipmentTypeNameEn = "Car" },
				new ShipmentType { ShipmentTypeId = 2, ShipmentTypeCode = 2, ShipmentTypeNameAr = "دراجة نارية", ShipmentTypeNameEn = "Motorcycle" },
				new ShipmentType { ShipmentTypeId = 3, ShipmentTypeCode = 3, ShipmentTypeNameAr = "شاحنة", ShipmentTypeNameEn = "Truck" },
				new ShipmentType { ShipmentTypeId = 4, ShipmentTypeCode = 4, ShipmentTypeNameAr = "الطائرة", ShipmentTypeNameEn = "Plane" },
				new ShipmentType { ShipmentTypeId = 5, ShipmentTypeCode = 5, ShipmentTypeNameAr = "الباخرة", ShipmentTypeNameEn = "Ship" }
			);

			//PaymentType
			builder.Entity<PaymentType>().HasData(
				new PaymentType { PaymentTypeId = 1, PaymentTypeNameAr = "نقدي", PaymentTypeNameEn = "Cash", PaymentTypeCode = "10" },
				new PaymentType { PaymentTypeId = 2, PaymentTypeNameAr = "حساب بنكي", PaymentTypeNameEn = "Bank Account", PaymentTypeCode = "42" },
				new PaymentType { PaymentTypeId = 3, PaymentTypeNameAr = "بطاقة مصرفية", PaymentTypeNameEn = "Bank Card", PaymentTypeCode = "48" },
				new PaymentType { PaymentTypeId = 4, PaymentTypeNameAr = "تقسيط", PaymentTypeNameEn = "Installment", PaymentTypeCode = "" },
				new PaymentType { PaymentTypeId = 5, PaymentTypeNameAr = "تحويل رصيد", PaymentTypeNameEn = "Credit Transfer", PaymentTypeCode = "30" }
			);


			//TaxType
			builder.Entity<TaxType>().HasData(
				new TaxType { TaxTypeId = 0, TaxTypeNameAr = "غير محدد", TaxTypeNameEn = "unknown" },
				new TaxType { TaxTypeId = 1, TaxTypeNameAr = "ضريبي", TaxTypeNameEn = "Taxable" },
				new TaxType { TaxTypeId = 2, TaxTypeNameAr = "معفي", TaxTypeNameEn = "Exempted" },
				new TaxType { TaxTypeId = 3, TaxTypeNameAr = "صفري", TaxTypeNameEn = "Zero" },
				new TaxType { TaxTypeId = 4, TaxTypeNameAr = "اتفاقيات خاصة", TaxTypeNameEn = "Private Contracts" },
				new TaxType { TaxTypeId = 5, TaxTypeNameAr = "صادرات", TaxTypeNameEn = "Exports" },
				new TaxType { TaxTypeId = 6, TaxTypeNameAr = "استيراد من الخارج", TaxTypeNameEn = "Imports" },
				new TaxType { TaxTypeId = 7, TaxTypeNameAr = "احتساب عكسي", TaxTypeNameEn = "Reverse Calculation" }
			);

			//InvoiceType
			builder.Entity<InvoiceType>().HasData(
				new InvoiceType { InvoiceTypeId = 0, InvoiceTypeNameAr = "غير معرف", InvoiceTypeNameEn = "unknown" },
				new InvoiceType { InvoiceTypeId = 1, InvoiceTypeNameAr = "فاتورة ضريبية ", InvoiceTypeNameEn = "Tax Invoice" },
				new InvoiceType { InvoiceTypeId = 2, InvoiceTypeNameAr = "فاتورة ضريبية مبسطة", InvoiceTypeNameEn = "Simplified Tax Invoice" }
			);

			//DocumentType
			builder.Entity<DocumentType>().HasData(
				new DocumentType { DocumentTypeId = 1, DocumentTypeNameAr = "الجرد والأرصدة الافتتاحية", DocumentTypeNameEn = "StockTaking And Open Balances" },
				new DocumentType { DocumentTypeId = 2, DocumentTypeNameAr = "اعتماد الجرد كرصيد إفتتاحي", DocumentTypeNameEn = "Approval of StockTaking as Open Balance" },
				new DocumentType { DocumentTypeId = 3, DocumentTypeNameAr = "الجرد والأرصدة الحالية", DocumentTypeNameEn = "StockTaking And Current Balances" },
				new DocumentType { DocumentTypeId = 4, DocumentTypeNameAr = "اعتماد الجرد كرصيد حالي", DocumentTypeNameEn = "Approval of StockTaking as Current Balance" },
				new DocumentType { DocumentTypeId = 5, DocumentTypeNameAr = "سند استلام", DocumentTypeNameEn = "Inventory In" },
				new DocumentType { DocumentTypeId = 6, DocumentTypeNameAr = "سند تسليم", DocumentTypeNameEn = "Inventory Out" },
				new DocumentType { DocumentTypeId = 7, DocumentTypeNameAr = "نقل بضاعة داخلي", DocumentTypeNameEn = "Internal Transfer Items" },
				new DocumentType { DocumentTypeId = 8, DocumentTypeNameAr = "استلام بضاعة داخلي", DocumentTypeNameEn = "Internal Receive Items" },
				new DocumentType { DocumentTypeId = 9, DocumentTypeNameAr = "قيد يومية", DocumentTypeNameEn = "Journal Entry" },
				new DocumentType { DocumentTypeId = 10, DocumentTypeNameAr = "سند قبض", DocumentTypeNameEn = "Receipt Voucher" },
				new DocumentType { DocumentTypeId = 11, DocumentTypeNameAr = "سند صرف", DocumentTypeNameEn = "Payment Voucher" },
				new DocumentType { DocumentTypeId = 12, DocumentTypeNameAr = "طلب بضاعة", DocumentTypeNameEn = "Product Request" },
				new DocumentType { DocumentTypeId = 13, DocumentTypeNameAr = "طلب تسعير بضاعة", DocumentTypeNameEn = "Product Request Price" },
				new DocumentType { DocumentTypeId = 14, DocumentTypeNameAr = "عرض أسعار المورد", DocumentTypeNameEn = "Supplier Quotation" },
				new DocumentType { DocumentTypeId = 15, DocumentTypeNameAr = "فاتورة شراء نقداً", DocumentTypeNameEn = "Cash Purchase Invoice" },
				new DocumentType { DocumentTypeId = 16, DocumentTypeNameAr = "فاتورة شراء آجل", DocumentTypeNameEn = "Credit Purchase Invoice" },
				new DocumentType { DocumentTypeId = 17, DocumentTypeNameAr = "فاتورة مرتجع شراء نقداً", DocumentTypeNameEn = "Cash Purchase Invoice Return" },
				new DocumentType { DocumentTypeId = 18, DocumentTypeNameAr = "فاتورة مرتجع شراء آجل", DocumentTypeNameEn = "Credit Purchase Invoice Return" },
				new DocumentType { DocumentTypeId = 19, DocumentTypeNameAr = "اتفاقية شراء", DocumentTypeNameEn = "Purchase Order" },
				new DocumentType { DocumentTypeId = 20, DocumentTypeNameAr = "بيان استلام", DocumentTypeNameEn = "Receipt Statement" },
				new DocumentType { DocumentTypeId = 21, DocumentTypeNameAr = "مرتجع من بيان استلام", DocumentTypeNameEn = "Receipt Statement Return" },
				new DocumentType { DocumentTypeId = 22, DocumentTypeNameAr = "فاتورة مشتريات مرحلية", DocumentTypeNameEn = "Purchase Invoice Interim " },
				new DocumentType { DocumentTypeId = 23, DocumentTypeNameAr = "فاتورة مشتريات بضاعة بالطريق نقداً", DocumentTypeNameEn = "Purchase Invoice On The Way Cash" },
				new DocumentType { DocumentTypeId = 24, DocumentTypeNameAr = "فاتورة مشتريات بضاعة بالطريق آجل", DocumentTypeNameEn = "Purchase Invoice On The Way Credit" },
				new DocumentType { DocumentTypeId = 25, DocumentTypeNameAr = "استلام من فاتورة بضاعة بالطريق", DocumentTypeNameEn = "Receipt From Purchase Invoice On The Way " },
				new DocumentType { DocumentTypeId = 26, DocumentTypeNameAr = "فاتورة مرتجع بضاعة بالطريق", DocumentTypeNameEn = "Purchase Invoice Return On The Way" },
				new DocumentType { DocumentTypeId = 27, DocumentTypeNameAr = "مرتجع من فاتورة المشتريات", DocumentTypeNameEn = "Return From Purchase Invoice" },
				new DocumentType { DocumentTypeId = 28, DocumentTypeNameAr = "فاتورة مرتجع المشتريات", DocumentTypeNameEn = "Purchase Invoice Return" },
				new DocumentType { DocumentTypeId = 29, DocumentTypeNameAr = "إشعار مدين", DocumentTypeNameEn = "Supplier Debit Memo" },
				new DocumentType { DocumentTypeId = 30, DocumentTypeNameAr = "إشعار دائن", DocumentTypeNameEn = "Supplier Credit Memo" },
				new DocumentType { DocumentTypeId = 31, DocumentTypeNameAr = "طلب تسعير", DocumentTypeNameEn = "Client Price Request" },
				new DocumentType { DocumentTypeId = 32, DocumentTypeNameAr = "عرض أسعار", DocumentTypeNameEn = "Client Quotation" },
				new DocumentType { DocumentTypeId = 33, DocumentTypeNameAr = "اعتماد عرض أسعار", DocumentTypeNameEn = "Client Quotation Approval" },
				new DocumentType { DocumentTypeId = 34, DocumentTypeNameAr = "فاتورة بيع نقداً", DocumentTypeNameEn = "Cash Sales Invoice" },
				new DocumentType { DocumentTypeId = 35, DocumentTypeNameAr = "فاتورة بيع آجل", DocumentTypeNameEn = "Credit Sales Invoice" },
				new DocumentType { DocumentTypeId = 36, DocumentTypeNameAr = "فاتورة مرتجع بيع نقداً", DocumentTypeNameEn = "Cash Sales Invoice Return" },
				new DocumentType { DocumentTypeId = 37, DocumentTypeNameAr = "فاتورة مرتجع بيع آجل", DocumentTypeNameEn = "Credit Sales Invoice Return" },
				new DocumentType { DocumentTypeId = 38, DocumentTypeNameAr = "اتفاقية بيع", DocumentTypeNameEn = "Proforma Invoice" },
				new DocumentType { DocumentTypeId = 39, DocumentTypeNameAr = "بيان تسليم", DocumentTypeNameEn = "Stock Out From Proforma Invoice" },
				new DocumentType { DocumentTypeId = 40, DocumentTypeNameAr = "مرتجع من بيان تسليم", DocumentTypeNameEn = "Stock Out Return From Stock Out" },
				new DocumentType { DocumentTypeId = 41, DocumentTypeNameAr = "فاتورة مرحلية", DocumentTypeNameEn = "Sales Invoice Interim" },
				new DocumentType { DocumentTypeId = 42, DocumentTypeNameAr = "فاتورة حجز نقدي", DocumentTypeNameEn = "Cash Reservation Invoice" },
				new DocumentType { DocumentTypeId = 43, DocumentTypeNameAr = "فاتورة حجز آجل", DocumentTypeNameEn = "Credit Reservation Invoice" },
				new DocumentType { DocumentTypeId = 44, DocumentTypeNameAr = "تسليم محجوز", DocumentTypeNameEn = "Stock Out From Reservation" },
				new DocumentType { DocumentTypeId = 45, DocumentTypeNameAr = "مرتجع من تسليم محجوز", DocumentTypeNameEn = "Stock Out Return From Reservation" },
				new DocumentType { DocumentTypeId = 46, DocumentTypeNameAr = "فاتورة تصفية حجز", DocumentTypeNameEn = "Reservation Invoice Close Out" },
				new DocumentType { DocumentTypeId = 47, DocumentTypeNameAr = "مرتجع من فاتورة", DocumentTypeNameEn = "Stock Out Return From Invoice" },
				new DocumentType { DocumentTypeId = 48, DocumentTypeNameAr = "فاتورة مرتجع", DocumentTypeNameEn = "Sales Invoice Return" },
				new DocumentType { DocumentTypeId = 49, DocumentTypeNameAr = "إشعار مدين للعميل", DocumentTypeNameEn = "Debit Memo" },
				new DocumentType { DocumentTypeId = 50, DocumentTypeNameAr = "إشعار دائن للعميل", DocumentTypeNameEn = "Credit Memo" }
			// TODO: Fixed Asset documents
			);

			//ItemCostCalculationType
			builder.Entity<ItemCostCalculationType>().HasData(
				new ItemCostCalculationType { ItemCostCalculationTypeId = 1, ItemCostCalculationTypeNameAr = "المتوسط الفعلي", ItemCostCalculationTypeNameEn = "Actual Average" },
				new ItemCostCalculationType { ItemCostCalculationTypeId = 2, ItemCostCalculationTypeNameAr = "السعر الإفتتاحي", ItemCostCalculationTypeNameEn = "Opening Price" },
				new ItemCostCalculationType { ItemCostCalculationTypeId = 3, ItemCostCalculationTypeNameAr = "آخر سعر شراء", ItemCostCalculationTypeNameEn = "last Purchase Price" },
				new ItemCostCalculationType { ItemCostCalculationTypeId = 4, ItemCostCalculationTypeNameAr = "آخر تكلفة شراء", ItemCostCalculationTypeNameEn = "Last Cost Price" }
			);


			////FixedAssetType
			//builder.Entity<FixedAssetType>().HasData(
			//    new FixedAssetType { FixedAssetTypeId = 1, FixedAssetTypeNameAr = "سيارات", FixedAssetTypeNameEn = "Cars" },
			//    new FixedAssetType { FixedAssetTypeId = 2, FixedAssetTypeNameAr = "أراضي", FixedAssetTypeNameEn = "Lands" },
			//    new FixedAssetType { FixedAssetTypeId = 3, FixedAssetTypeNameAr = "عقارات", FixedAssetTypeNameEn = "Real Estate" },
			//    new FixedAssetType { FixedAssetTypeId = 4, FixedAssetTypeNameAr = "آلات", FixedAssetTypeNameEn = "Machines" },
			//    new FixedAssetType { FixedAssetTypeId = 5, FixedAssetTypeNameAr = "معدات", FixedAssetTypeNameEn = "Equipment" },
			//    new FixedAssetType { FixedAssetTypeId = 6, FixedAssetTypeNameAr = "ديكور", FixedAssetTypeNameEn = "Decor" },
			//    new FixedAssetType { FixedAssetTypeId = 7, FixedAssetTypeNameAr = "أثاث", FixedAssetTypeNameEn = "Furniture" },
			//    new FixedAssetType { FixedAssetTypeId = 8, FixedAssetTypeNameAr = "أجهزة كهربائية", FixedAssetTypeNameEn = "Electrical Devices" },
			//    new FixedAssetType { FixedAssetTypeId = 9, FixedAssetTypeNameAr = "أخري", FixedAssetTypeNameEn = "Other" }
			//);


			//CompanyFlagType
			builder.Entity<ApplicationFlagType>().HasData(
				new ApplicationFlagType { ApplicationFlagTypeId = 1, ApplicationFlagTypeNameAr = "نص", ApplicationFlagTypeNameEn = "Text" },
				new ApplicationFlagType { ApplicationFlagTypeId = 2, ApplicationFlagTypeNameAr = "رقم", ApplicationFlagTypeNameEn = "Number" },
				new ApplicationFlagType { ApplicationFlagTypeId = 3, ApplicationFlagTypeNameAr = "تاريخ", ApplicationFlagTypeNameEn = "Date" },
				new ApplicationFlagType { ApplicationFlagTypeId = 4, ApplicationFlagTypeNameAr = "وقت", ApplicationFlagTypeNameEn = "Time" },
				new ApplicationFlagType { ApplicationFlagTypeId = 5, ApplicationFlagTypeNameAr = "تاريخ محدد بوقت", ApplicationFlagTypeNameEn = "DateTime" },
				new ApplicationFlagType { ApplicationFlagTypeId = 6, ApplicationFlagTypeNameAr = "نعم/لا", ApplicationFlagTypeNameEn = "Boolean" },
				new ApplicationFlagType { ApplicationFlagTypeId = 7, ApplicationFlagTypeNameAr = "اختيار", ApplicationFlagTypeNameEn = "Select" },
				new ApplicationFlagType { ApplicationFlagTypeId = 8, ApplicationFlagTypeNameAr = "رفع صورة", ApplicationFlagTypeNameEn = "Upload Image" }
			);

			//SellerType
			builder.Entity<SellerType>().HasData(
				new SellerType { SellerTypeId = 1, SellerTypeNameAr = "موظف", SellerTypeNameEn = "Employee" },
				new SellerType { SellerTypeId = 2, SellerTypeNameAr = "توظيف خارجي", SellerTypeNameEn = "OutSourcing" },
				new SellerType { SellerTypeId = 3, SellerTypeNameAr = "بالعمولة", SellerTypeNameEn = "With a Commission" },
				new SellerType { SellerTypeId = 4, SellerTypeNameAr = "أخري", SellerTypeNameEn = "Other" }
			);

			//SellerCommissionType
			builder.Entity<SellerCommissionType>().HasData(
				new SellerCommissionType { SellerCommissionTypeId = 1, SellerCommissionTypeNameAr = "حسب الدخل النقدي", SellerCommissionTypeNameEn = "According To Cash Income" },
				new SellerCommissionType { SellerCommissionTypeId = 2, SellerCommissionTypeNameAr = "حسب عمر الدين المحصل", SellerCommissionTypeNameEn = "According To Age Of Debt Collected" }
			);

			//StockType
			builder.Entity<StockType>().HasData(
				new StockType { StockTypeId = 1, StockTypeNameAr = "بيان استلام", StockTypeNameEn = "Receipt Statement" },
				new StockType { StockTypeId = 2, StockTypeNameAr = "استلام من فاتورة بضاعة بالطريق", StockTypeNameEn = "Receipt From Purchase Invoice On The Way" },
				new StockType { StockTypeId = 3, StockTypeNameAr = "مرتجع من بيان استلام", StockTypeNameEn = "Receipt Statement Return" },
				new StockType { StockTypeId = 4, StockTypeNameAr = "مرتجع من استلام فاتورة بضاعة بالطريق", StockTypeNameEn = "Receipt From Purchase Invoice On The Way Return" },
				new StockType { StockTypeId = 5, StockTypeNameAr = "مرتجع من فاتورة المشتريات", StockTypeNameEn = "StockIn Return From Purchase Invoice" },

				new StockType { StockTypeId = 6, StockTypeNameAr = "بيان تسليم", StockTypeNameEn = "StockOut From Proforma Invoice" },
				new StockType { StockTypeId = 7, StockTypeNameAr = "تسليم محجوز", StockTypeNameEn = "Stock Out From Reservation" },
				new StockType { StockTypeId = 8, StockTypeNameAr = "مرتجع من بيان تسليم", StockTypeNameEn = "Stock Out Return From Stock Out" },
				new StockType { StockTypeId = 9, StockTypeNameAr = "مرتجع من تسليم محجوز", StockTypeNameEn = "Stock Out Return From Reservation" },
				new StockType { StockTypeId = 10, StockTypeNameAr = "مرتجع من فاتورة المبيعات", StockTypeNameEn = "StockOut Return From Sales Invoice" }
			);

			//Menu
			builder.Entity<Menu>().HasData(
				new Menu { MenuCode = 1, MenuNameAr = "الطلبات", MenuNameEn = "Requests", MenuUrl = "/request", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 2, MenuNameAr = "تعريف الموافقات ومراحلها", MenuNameEn = "Approvals Steps", MenuUrl = "/approve", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 3, MenuNameAr = "الموافقة علي المستندات", MenuNameEn = "Documents Approvals", MenuUrl = "/approverequest", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 4, MenuNameAr = "استيراد البيانات الأساسية", MenuNameEn = "Import Basic Data", MenuUrl = "/task/importfiles", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 5, MenuNameAr = "ترميز أكواد المستندات", MenuNameEn = "Menu Encoding", MenuUrl = "/menuencoding", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 6, MenuNameAr = "إعدادات البرنامج", MenuNameEn = "Application Settings", MenuUrl = "/setting", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 7, MenuNameAr = "الدول", MenuNameEn = "Countries", MenuUrl = "/country", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 8, MenuNameAr = "المناطق الإدارية", MenuNameEn = "States", MenuUrl = "/state", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 9, MenuNameAr = "المدن", MenuNameEn = "Cities", MenuUrl = "/city", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 10, MenuNameAr = "الأحياء", MenuNameEn = "Districts", MenuUrl = "/district", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 11, MenuNameAr = "الأنشطة", MenuNameEn = "Business", MenuUrl = "/company", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 12, MenuNameAr = "الفروع", MenuNameEn = "Branches", MenuUrl = "/branch", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 13, MenuNameAr = "المواقع", MenuNameEn = "Stores", MenuUrl = "/store", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 14, MenuNameAr = "الموردين", MenuNameEn = "Suppliers", MenuUrl = "/supplier", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 15, MenuNameAr = "العملاء", MenuNameEn = "Clients", MenuUrl = "/client", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 16, MenuNameAr = "مندوبي المبيعات", MenuNameEn = "Sellers", MenuUrl = "/seller", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 17, MenuNameAr = "البنوك", MenuNameEn = "Banks", MenuUrl = "/bank", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 18, MenuNameAr = "معرفات الملاحظات", MenuNameEn = "Notes Identifiers", MenuUrl = "/noteidentifier", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 19, MenuNameAr = "اسماء شرائح عمولة المناديب", MenuNameEn = "Seller Commission Names", MenuUrl = "/sellercommissionmethod", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 20, MenuNameAr = "أنواع مصاريف الفاتورة", MenuNameEn = "Invoice Expense Types", MenuUrl = "/invoiceexpensetype", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 21, MenuNameAr = "طرق الشحن", MenuNameEn = "Shipment Types", MenuUrl = "/shipmenttype", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 22, MenuNameAr = "أنواع حالات الشحن", MenuNameEn = "Shipping Status Types", MenuUrl = "/shippingstatus", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 23, MenuNameAr = "شرائح عمولات مندوبي المبيعات", MenuNameEn = "Sellers Commision Methods", MenuUrl = "/sellercommission", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 24, MenuNameAr = "الضرائب", MenuNameEn = "Taxes", MenuUrl = "/tax", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 25, MenuNameAr = "طرق القبض/الدفع", MenuNameEn = "Payment/Receiving Methods", MenuUrl = "/paymentmethod", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 26, MenuNameAr = "العملات", MenuNameEn = "Currencies", MenuUrl = "/currency", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 27, MenuNameAr = "أسعار العملات", MenuNameEn = "Currencies Rates", MenuUrl = "/currencyrate", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 28, MenuNameAr = "الأصناف", MenuNameEn = "Items", MenuUrl = "/item", HasApprove = false, HasNotes = true, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 29, MenuNameAr = "وحدات الأصناف", MenuNameEn = "Items Packages", MenuUrl = "/itempackage", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 30, MenuNameAr = "تقسيمات الأصناف", MenuNameEn = "Items Categories", MenuUrl = "/itemdivision", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 31, MenuNameAr = "الشركات المصنعة", MenuNameEn = "Vendors", MenuUrl = "/vendor", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 32, MenuNameAr = "سمات الأصناف", MenuNameEn = "Item Attributes", MenuUrl = "/itemattribute", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 33, MenuNameAr = "الرصيد الافتتاحي", MenuNameEn = "Open Balance", MenuUrl = "/openbalance", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 34, MenuNameAr = "رصيد المخزن", MenuNameEn = "Current Balance", MenuUrl = "/currentbalance", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 35, MenuNameAr = "أسعار التكلفة", MenuNameEn = "Item Cost", MenuUrl = "/itemcost", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 36, MenuNameAr = "الجرد والأرصدة الافتتاحية", MenuNameEn = "StockTaking And Open Balances", MenuUrl = "/stocktakingopenbalance", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 37, MenuNameAr = "اعتماد الجرد كرصيد إفتتاحي", MenuNameEn = "Approval of StockTaking as Open Balance", MenuUrl = "/stocktakingcarryoveropenbalance", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 38, MenuNameAr = "الجرد والأرصدة الحالية", MenuNameEn = "StockTaking And Current Balances", MenuUrl = "/stocktakingcurrentbalance", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 39, MenuNameAr = "اعتماد الجرد كرصيد حالي", MenuNameEn = "Approval of StockTaking as Current Balance", MenuUrl = "/stocktakingcarryovercurrentbalance", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 40, MenuNameAr = "سند استلام", MenuNameEn = "Inventory In", MenuUrl = "/inventoryin", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 41, MenuNameAr = "سند تسليم", MenuNameEn = "Inventory Out", MenuUrl = "/inventoryout", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 42, MenuNameAr = "نقل بضاعة داخلي", MenuNameEn = "Internal Transfer Items", MenuUrl = "/internaltransfer", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 43, MenuNameAr = "استلام بضاعة داخلي", MenuNameEn = "Internal Receive Items", MenuUrl = "/internaltransferreceive", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 44, MenuNameAr = "الدليل المحاسبي", MenuNameEn = "Charts of Account", MenuUrl = "/account", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 45, MenuNameAr = "قيد يومية", MenuNameEn = "Journal Entry", MenuUrl = "/journalentry", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 46, MenuNameAr = "سند قبض", MenuNameEn = "Receipt Voucher", MenuUrl = "/receiptvoucher", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 47, MenuNameAr = "سند صرف", MenuNameEn = "Payment Voucher", MenuUrl = "/paymentvoucher", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 48, MenuNameAr = "مراكز التكلفة", MenuNameEn = "Cost Center", MenuUrl = "/costcenter", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 49, MenuNameAr = "طلب بضاعة", MenuNameEn = "Product Request", MenuUrl = "/productrequest", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 50, MenuNameAr = "طلب تسعير بضاعة", MenuNameEn = "Product Request Price", MenuUrl = "/productrequestprice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 51, MenuNameAr = "عرض أسعار المورد", MenuNameEn = "Supplier Quotation", MenuUrl = "/supplierquotation", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 52, MenuNameAr = "فاتورة شراء نقداً", MenuNameEn = "Cash Purchase Invoice", MenuUrl = "/purchaseinvoicecash", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 53, MenuNameAr = "فاتورة شراء آجل", MenuNameEn = "Credit Purchase Invoice", MenuUrl = "/purchaseinvoicecredit", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 54, MenuNameAr = "فاتورة مرتجع شراء نقداً", MenuNameEn = "Cash Purchase Invoice Return", MenuUrl = "/purchaseinvoicereturncash", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 55, MenuNameAr = "فاتورة مرتجع شراء آجل", MenuNameEn = "Credit Purchase Invoice Return", MenuUrl = "/purchaseinvoicereturncredit", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 56, MenuNameAr = "اتفاقية شراء", MenuNameEn = "Purchase Order", MenuUrl = "/purchaseorder", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 57, MenuNameAr = "بيان استلام", MenuNameEn = "Receipt Statement", MenuUrl = "/stockinfrompurchaseorder", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 58, MenuNameAr = "مرتجع من بيان استلام", MenuNameEn = "Receipt Statement Return", MenuUrl = "/stockinreturnfromstockin", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 59, MenuNameAr = "فاتورة مشتريات مرحلية", MenuNameEn = "Purchase Invoice Interim ", MenuUrl = "/purchaseinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 60, MenuNameAr = "فاتورة مشتريات بضاعة بالطريق نقداً", MenuNameEn = "Purchase Invoice On The Way Cash", MenuUrl = "/purchaseinvoicecashonway", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 61, MenuNameAr = "فاتورة مشتريات بضاعة بالطريق آجل", MenuNameEn = "Purchase Invoice On The Way Credit", MenuUrl = "/purchaseinvoicecreditonway", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 62, MenuNameAr = "استلام من فاتورة بضاعة بالطريق", MenuNameEn = "Receipt From Purchase Invoice On The Way ", MenuUrl = "/stockinfrompurchaseinvoiceonway", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 63, MenuNameAr = "مرتجع من استلام فاتورة بضاعة بالطريق", MenuNameEn = "Receipt From Purchase Invoice On The Way Return", MenuUrl = "/stockinreturnfromstockinonway", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 64, MenuNameAr = "فاتورة مرتجع بضاعة بالطريق", MenuNameEn = "Purchase Invoice Return On The Way", MenuUrl = "/purchaseinvoicereturnonway", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 65, MenuNameAr = "مرتجع من فاتورة المشتريات", MenuNameEn = "Return From Purchase Invoice", MenuUrl = "/stockinreturnfrompurchaseinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 66, MenuNameAr = "فاتورة مرتجع المشتريات", MenuNameEn = "Purchase Invoice Return", MenuUrl = "/purchaseinvoicereturn", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 67, MenuNameAr = "إشعار مدين المورد", MenuNameEn = "Supplier Debit Memo", MenuUrl = "/supplierdebitmemo", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 68, MenuNameAr = "إشعار دائن المورد", MenuNameEn = "Supplier Credit Memo", MenuUrl = "/suppliercreditmemo", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 69, MenuNameAr = "طلب تسعير", MenuNameEn = "Client Price Request", MenuUrl = "/clientpricerequest", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 70, MenuNameAr = "عرض أسعار", MenuNameEn = "Client Quotation", MenuUrl = "/clientquotation", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 71, MenuNameAr = "اعتماد عرض أسعار", MenuNameEn = "Client Quotation Approval", MenuUrl = "/clientquotationapproval", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 72, MenuNameAr = "فاتورة بيع نقداً", MenuNameEn = "Cash Sales Invoice", MenuUrl = "/cashsalesinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 73, MenuNameAr = "فاتورة بيع آجل", MenuNameEn = "Credit Sales Invoice", MenuUrl = "/creditsalesinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 74, MenuNameAr = "فاتورة مرتجع بيع نقداً", MenuNameEn = "Cash Sales Invoice Return", MenuUrl = "/cashsalesinvoicereturn", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 75, MenuNameAr = "فاتورة مرتجع بيع آجل", MenuNameEn = "Credit Sales Invoice Return", MenuUrl = "/creditsalesinvoicereturn", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 76, MenuNameAr = "اتفاقية بيع", MenuNameEn = "Proforma Invoice", MenuUrl = "/proformainvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 77, MenuNameAr = "بيان تسليم", MenuNameEn = "Stock Out From Proforma Invoice", MenuUrl = "/stockoutfromproformainvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 78, MenuNameAr = "مرتجع من بيان تسليم", MenuNameEn = "Stock Out Return From Stock Out", MenuUrl = "/stockoutreturnfromstockout", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 79, MenuNameAr = "فاتورة مرحلية", MenuNameEn = "Sales Invoice Interim", MenuUrl = "/salesinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 80, MenuNameAr = "فاتورة حجز نقدي", MenuNameEn = "Cash Reservation Invoice", MenuUrl = "/cashreservationinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 81, MenuNameAr = "فاتورة حجز آجل", MenuNameEn = "Credit Reservation Invoice", MenuUrl = "/creditreservationinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 82, MenuNameAr = "تسليم محجوز", MenuNameEn = "Stock Out From Reservation", MenuUrl = "/stockoutfromreservationinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 83, MenuNameAr = "مرتجع من تسليم محجوز", MenuNameEn = "Stock Out Return From Reservation", MenuUrl = "/stockoutreturnfromreservationinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 84, MenuNameAr = "فاتورة تصفية حجز", MenuNameEn = "Reservation Invoice Close Out", MenuUrl = "/reservationinvoicecloseout", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 85, MenuNameAr = "مرتجع من فاتورة المبيعات", MenuNameEn = "Stock Out Return From Invoice", MenuUrl = "/stockoutreturnfromsalesinvoice", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 86, MenuNameAr = "فاتورة مرتجع المبيعات", MenuNameEn = "Sales Invoice Return", MenuUrl = "/salesinvoicereturn", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 87, MenuNameAr = "إشعار مدين للعميل", MenuNameEn = "Client Debit Memo", MenuUrl = "/clientdebitmemo", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 88, MenuNameAr = "إشعار دائن للعميل", MenuNameEn = "Client Credit Memo", MenuUrl = "/clientcreditmemo", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false },
				new Menu { MenuCode = 89, MenuNameAr = "فك وحدات الأصناف", MenuNameEn = "Disassemble Item Packages", MenuUrl = "/disassembleitempackages", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 90, MenuNameAr = "بيان فك وحدات الأصناف", MenuNameEn = "Disassemble Item Packages Statement", MenuUrl = "/disassemblestatement", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false },
				new Menu { MenuCode = 91, MenuNameAr = "ميزان مراجعة أساسي", MenuNameEn = "Basic Trial Balance", MenuUrl = "/reports/accounting/basictrialbalance", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 92, MenuNameAr = "ميزان مراجعة فرعي", MenuNameEn = "Sub-Trial Balance", MenuUrl = "/reports/accounting/subtrialbalance", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 93, MenuNameAr = "أرصدة الحسابات  ", MenuNameEn = "Account Balances", MenuUrl = "/reports/accounting/accountbalances", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 94, MenuNameAr = "قائمة الدخل", MenuNameEn = "Income Statement", MenuUrl = "/reports/accounting/incomestatement", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 95, MenuNameAr = "ميزانية عمومية", MenuNameEn = "Balance Sheet", MenuUrl = "/reports/accounting/balancesheet", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 96, MenuNameAr = "بيانات الحسابات الجزئية", MenuNameEn = "Individual Account Statement", MenuUrl = "/reports/accounting/individualaccountstatement", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 97, MenuNameAr = "كشف حساب مفصل", MenuNameEn = "Detailed Account Statement", MenuUrl = "/reports/accounting/accountstatementdetailed", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 98, MenuNameAr = "كشف حساب لمجموعة حسابات", MenuNameEn = "Account Statement for Group of Accounts", MenuUrl = "/reports/accounting/accountstatementgrouped", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 99, MenuNameAr = "دفتر قيود اليومية", MenuNameEn = "General Journal", MenuUrl = "/reports/accounting/generaljournal", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 100, MenuNameAr = "أرقام السندات المفقودة", MenuNameEn = "Missing Documents Numbers", MenuUrl = "/reports/accounting/missingdocumentsnumbers", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 101, MenuNameAr = "تقرير اجمالي الدخل للموقع", MenuNameEn = "Gross Income Report On Store", MenuUrl = "/reports/accounting/grossincome", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 102, MenuNameAr = "تقرير طرق التحصيلات النقدية", MenuNameEn = "Cash Collections Methods Report", MenuUrl = "/reports/accounting/cashmethodsreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 103, MenuNameAr = "التقرير الضريبي النموذجي", MenuNameEn = "Typical Tax Report", MenuUrl = "/reports/tax/typicaltaxreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 104, MenuNameAr = "تقرير تفصيلي الضريبة", MenuNameEn = "Detailed Tax Report", MenuUrl = "/reports/tax/detailedtaxreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 105, MenuNameAr = "تقرير الضرائب الأخرى", MenuNameEn = "Other Tax Report", MenuUrl = "/reports/tax/othertaxreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 106, MenuNameAr = "تفصيلي الضرائب الأخرى", MenuNameEn = "Detailed Other Tax Report", MenuUrl = "/reports/tax/detailedothertaxreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 107, MenuNameAr = "مجمع مراكز التكلفة الأساسي", MenuNameEn = "Main Cost Center Consolidated", MenuUrl = "/reports/costcenter/maincenterconsolidated", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 108, MenuNameAr = "مجمع مراكز التكلفة الجزئي", MenuNameEn = "Individual Cost Center Consolidated", MenuUrl = "/reports/costcenter/individualcenterconsolidated", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 109, MenuNameAr = "دفتر قيود يومية لمركز تكلفة", MenuNameEn = "Cost Center Journal", MenuUrl = "/reports/costcenter/costcenterjournal", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 110, MenuNameAr = "حسابات مراكز التكلفة", MenuNameEn = "Cost Center Accounts", MenuUrl = "/reports/costcenter/costcenteraccounts", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 111, MenuNameAr = "استهلاك الأصول الثابتة", MenuNameEn = "Fixed Assets Depreciation", MenuUrl = "/reports/fixedasset/fixedassetsdepreciation", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 112, MenuNameAr = "الحركة الأصلية في مركز التكلفة (المشروع)", MenuNameEn = "Original Movement in Cost Center (Project)", MenuUrl = "/reports/fixedasset/originalmovementincostcenter", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 113, MenuNameAr = "جرد البضاعة", MenuNameEn = "Inventory Of Goods", MenuUrl = "/reports/inventory/inventoryofgoods", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 114, MenuNameAr = "قيمة البضاعة", MenuNameEn = "Value Of Goods", MenuUrl = "/reports/inventory/valueofgoods", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 115, MenuNameAr = "حركة تفصيلية لصنف", MenuNameEn = "Detailed Transaction Of An Item", MenuUrl = "/reports/inventory/transactionofitem", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 116, MenuNameAr = "حركة الأصناف", MenuNameEn = "Transaction Of Items", MenuUrl = "/reports/inventory/transactionofitems", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 117, MenuNameAr = "الحركة التجارية للمشتريات", MenuNameEn = "Commercial Movement Of Purchases", MenuUrl = "/reports/purchases/commercialmovementpurchases", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 118, MenuNameAr = "الأصناف منتهية الصلاحية", MenuNameEn = "Expired Items", MenuUrl = "/reports/inventory/expireditems", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 119, MenuNameAr = "الأصناف التي ستنتهي صلاحيتها خلال مدة", MenuNameEn = "Items That Will Expire Within A Period", MenuUrl = "/reports/inventory/itemswillexpire", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 120, MenuNameAr = "الأصناف الراكدة لم ينفذ لها بيع منذ مدة", MenuNameEn = "Stagnant Items That Haven't Been Sold For A Period", MenuUrl = "/reports/inventory/stagnantitems", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 121, MenuNameAr = "الأصناف الأكثر ربيحة", MenuNameEn = "Most Profitable Items", MenuUrl = "/reports/inventory/mostprofitableitems", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 122, MenuNameAr = "الأصناف الأكثر دوران", MenuNameEn = "Most Circulating Items", MenuUrl = "/reports/inventory/mostcirculatingitems", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 123, MenuNameAr = "الأصناف التي وصلت لحد الطلب", MenuNameEn = "Items That Have Reached The Demand Limit", MenuUrl = "/reports/inventory/itemsdemandlimit", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 124, MenuNameAr = "فواتير البيع المعمرة", MenuNameEn = "Outstanding Sales Invoices", MenuUrl = "/reports/client/outstandingsalesinvoices", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 125, MenuNameAr = "فواتير البيع الغيرة مسددة", MenuNameEn = "Unpaid Sales Invoices", MenuUrl = "/reports/client/unpaidsalesinvoices", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 126, MenuNameAr = "عملاء تجاوز رصيدهم حد الائتمان", MenuNameEn = "Clients with Credit Limit Exceeded", MenuUrl = "/reports/client/clientslimitexceeded", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 127, MenuNameAr = "فواتير بيع سيحل موعد سدادها خلال فترة", MenuNameEn = "Sales Invoices Due in a Period", MenuUrl = "/reports/client/salesinaperiod", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 128, MenuNameAr = "فواتير مبيعات نسبة الربح فيها أقل من", MenuNameEn = "Sales Invoices with a Profit Rate Less than", MenuUrl = "/reports/client/saleslessthan", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 129, MenuNameAr = "فواتير مبيعات نسبة الخصم فيها أكبر من", MenuNameEn = "Sales Invoices with a Discount Rate Greater than", MenuUrl = "/reports/client/salesgreaterthan", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 130, MenuNameAr = "تقرير النشاط التجاري", MenuNameEn = "Business Activity Report", MenuUrl = "/reports/client/businessactivityreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 131, MenuNameAr = "العملاء الأكثر مبيعاً (مبالغ)", MenuNameEn = "Top Selling Clients (Amounts)", MenuUrl = "/reports/client/topsellingclients", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 132, MenuNameAr = "العملاء الأكثر دورية (عدد فواتير)", MenuNameEn = "Top Frequent Clients (No of Invoices)", MenuUrl = "/reports/client/topfrequentclients", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 133, MenuNameAr = "العملاء الأكثر ربحية", MenuNameEn = "Top Profitable Clients", MenuUrl = "/reports/client/topprofitableclients", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 134, MenuNameAr = "فواتير الشراء المعمرة", MenuNameEn = "Outstanding Purchase Invoices", MenuUrl = "/reports/supplier/outstandingpurchaseinvoices", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 135, MenuNameAr = "فواتير الشراء الغير مسددة", MenuNameEn = "Unpaid Purchase Invoices", MenuUrl = "/reports/supplier/unpaidpurchaseinvoices", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 136, MenuNameAr = "موردين تجاوز رصيدهم حد الائتمان", MenuNameEn = "Suppliers Over Credit Limit", MenuUrl = "/reports/supplier/supplierscreditlimit", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 137, MenuNameAr = "فواتير شراء سيحل موعد سدادها خلال فترة", MenuNameEn = "Purchase Invoices Due Within a Period", MenuUrl = "/reports/supplier/purchaseinaperiod", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 138, MenuNameAr = "تقرير النشاط التجاري لفواتير الشراء", MenuNameEn = "Business Activity Report for Purchase Invoices", MenuUrl = "/reports/supplier/businesspurchaseinvoices", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 139, MenuNameAr = "تقرير عمولات المناديب", MenuNameEn = "Sellers Commission Report", MenuUrl = "/reports/seller/sellerscommissionreport", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 140, MenuNameAr = "متابعة طلبات البضاعة", MenuNameEn = "Products Request Follow-up", MenuUrl = "/reports/purchases/productrequestfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 141, MenuNameAr = "متابعة طلبات التسعير للموردين", MenuNameEn = "Products Requests Price Follow-up", MenuUrl = "/reports/purchases/productrequestpricefollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 142, MenuNameAr = "متابعة عروض أسعار الموردين", MenuNameEn = "Suppliers Quotations Follow-up", MenuUrl = "/reports/purchases/supplierquotationfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 143, MenuNameAr = "متابعة اتفاقيات شراء", MenuNameEn = "Purchase Order Follow-up", MenuUrl = "/reports/purchases/purchaseorderfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 144, MenuNameAr = "متابعة بيانات استلام", MenuNameEn = "Receipt Statements Follow-up", MenuUrl = "/reports/purchases/receiptstatementfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 145, MenuNameAr = "متابعة مرتجع من بيانات استلام", MenuNameEn = "Receipt Statements Return Follow-up", MenuUrl = "/reports/purchases/receiptstatementreturnfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 146, MenuNameAr = "متابعة فاتورة مشتريات مرحلية", MenuNameEn = "Purchase Invoices Interim Follow-up", MenuUrl = "/reports/purchases/purchaseinvoicefollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 147, MenuNameAr = "متابعة مرتجع كميات من فاتورة مشتريات مرحلية", MenuNameEn = "Purchase Invoices Interim Return Follow-up", MenuUrl = "/reports/purchases/purchaseinvoiceinterimfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 148, MenuNameAr = "متابعة بضاعة بالطريق", MenuNameEn = "Products On The Way Follow-up", MenuUrl = "/reports/purchases/productonthewayfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 149, MenuNameAr = "متابعة طلبات التسعير للعملاء", MenuNameEn = "Client Price Requests Follow-up", MenuUrl = "/reports/sales/clientpricerequestfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 150, MenuNameAr = "متابعة عروض الأسعار للعملاء", MenuNameEn = "Client Quotations Follow-up", MenuUrl = "/reports/sales/clientquotationfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 151, MenuNameAr = "متابعة اعتمادات عروض الأسعار للعملاء", MenuNameEn = "Client Quotations Approval Follow-up", MenuUrl = "/reports/sales/clientquotationapprovalfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 152, MenuNameAr = "متابعة اتفاقيات بيع", MenuNameEn = "Proforma Invoices Follow-up", MenuUrl = "/reports/sales/proformainvoicefollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 153, MenuNameAr = "متابعة بيان تسليم", MenuNameEn = "Stock Out Follow-up", MenuUrl = "/reports/sales/stockoutfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 154, MenuNameAr = "متابعة مرتجع من بيان تسليم", MenuNameEn = "Stock Out Return Follow-up", MenuUrl = "/reports/sales/stockoutreturnfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 155, MenuNameAr = "متابعة فاتورة  بيع مرحلية", MenuNameEn = "Sales Invoice Interim Follow-up", MenuUrl = "/reports/sales/salesinvoiceinterimfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 156, MenuNameAr = "متابعة مرتجع كميات من فاتورة مرحلية", MenuNameEn = "Sales Invoice Interim Return Follow-up", MenuUrl = "/reports/sales/salesinvoiceinterimreturnfollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 157, MenuNameAr = "متابعة بضاعة محجوزة", MenuNameEn = "Reservation Invoice Follow-up", MenuUrl = "/reports/sales/reservationinvoicefollowup", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true },
				new Menu { MenuCode = 158, MenuNameAr = "الأصول الثابتة", MenuNameEn = "Fixed Assets", MenuUrl = "/fixedasset", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false, IsReport = false },
				new Menu { MenuCode = 159, MenuNameAr = "نقل أصل ثابت", MenuNameEn = "Fixed Asset Movement", MenuUrl = "/fixedassetmovement", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false, IsReport = false },
				new Menu { MenuCode = 160, MenuNameAr = "إضافة أصل ثابت", MenuNameEn = "Fixed Asset Addition", MenuUrl = "/fixedassetaddition", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false, IsReport = false },
				new Menu { MenuCode = 161, MenuNameAr = "استبعاد أصل ثابت", MenuNameEn = "Fixed Asset Exclusion", MenuUrl = "/fixedassetexclusion", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false, IsReport = false },
				new Menu { MenuCode = 162, MenuNameAr = "إهلاك أصل ثابت", MenuNameEn = "Fixed Asset Depreciation", MenuUrl = "/fixedassetdepreciation", HasApprove = true, HasNotes = true, HasEncoding = true, IsFavorite = false, IsReport = false },
				new Menu { MenuCode = 163, MenuNameAr = "الحركة التجارية للمبيعات", MenuNameEn = "Commercial Movement Of Sales", MenuUrl = "/reports/purchases/commercialmovementsales", HasApprove = false, HasNotes = false, HasEncoding = false, IsFavorite = false, IsReport = true }
			);


			//ApplicationFlagTab
			builder.Entity<ApplicationFlagTab>().HasData(
				new ApplicationFlagTab { ApplicationFlagTabId = 1, ApplicationFlagTabNameAr = "تقسيمات الأصناف", ApplicationFlagTabNameEn = "Items Categories", Order = 5 },
				new ApplicationFlagTab { ApplicationFlagTabId = 2, ApplicationFlagTabNameAr = "ترميز الدليل المحاسبي", ApplicationFlagTabNameEn = "Charts of Account Encoding", Order = 6 },
				new ApplicationFlagTab { ApplicationFlagTabId = 3, ApplicationFlagTabNameAr = "ترميز مراكز التكلفة", ApplicationFlagTabNameEn = "Cost Centers Encoding", Order = 7 },
				new ApplicationFlagTab { ApplicationFlagTabId = 4, ApplicationFlagTabNameAr = "أسعار التكلفة", ApplicationFlagTabNameEn = "Item Cost", Order = 8 },
				new ApplicationFlagTab { ApplicationFlagTabId = 5, ApplicationFlagTabNameAr = "الأصناف", ApplicationFlagTabNameEn = "Items", Order = 4 },
				new ApplicationFlagTab { ApplicationFlagTabId = 6, ApplicationFlagTabNameAr = "فاتورة المشتريات", ApplicationFlagTabNameEn = "Purchase Invoice", Order = 9 },
				new ApplicationFlagTab { ApplicationFlagTabId = 7, ApplicationFlagTabNameAr = "فاتورة مرتجع المشتريات", ApplicationFlagTabNameEn = "Purchase Invoice Return", Order = 10 },
				new ApplicationFlagTab { ApplicationFlagTabId = 8, ApplicationFlagTabNameAr = "عرض أسعار العميل", ApplicationFlagTabNameEn = "Client Quotation", Order = 11 },
				new ApplicationFlagTab { ApplicationFlagTabId = 9, ApplicationFlagTabNameAr = "فاتورة المبيعات", ApplicationFlagTabNameEn = "Sales Invoice", Order = 12 },
				new ApplicationFlagTab { ApplicationFlagTabId = 10, ApplicationFlagTabNameAr = "فاتورة مرتجع المبيعات", ApplicationFlagTabNameEn = "Sales Invoice Return", Order = 13 },
				new ApplicationFlagTab { ApplicationFlagTabId = 11, ApplicationFlagTabNameAr = "إعدادات عامة", ApplicationFlagTabNameEn = "General Settings", Order = 1 },
				new ApplicationFlagTab { ApplicationFlagTabId = 12, ApplicationFlagTabNameAr = "طباعة التقارير", ApplicationFlagTabNameEn = "Print Reports", Order = 2 },
				new ApplicationFlagTab { ApplicationFlagTabId = 13, ApplicationFlagTabNameAr = "طباعة الفواتير", ApplicationFlagTabNameEn = "Print Invoices", Order = 3 }
			);

			//ApplicationFlagHeader
			builder.Entity<ApplicationFlagHeader>().HasData(
				new ApplicationFlagHeader { ApplicationFlagTabId = 1, ApplicationFlagHeaderId = 1, ApplicationFlagHeaderNameAr = "تقسيمات الأصناف", ApplicationFlagHeaderNameEn = "Items Categories", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 2, ApplicationFlagHeaderId = 2, ApplicationFlagHeaderNameAr = "ترميز الدليل المحاسبي", ApplicationFlagHeaderNameEn = "Charts of Account Encoding", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 3, ApplicationFlagHeaderId = 3, ApplicationFlagHeaderNameAr = "ترميز مراكز التكلفة", ApplicationFlagHeaderNameEn = "Cost Centers Encoding", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 4, ApplicationFlagHeaderId = 4, ApplicationFlagHeaderNameAr = "أسعار التكلفة", ApplicationFlagHeaderNameEn = "Item Cost", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 5, ApplicationFlagHeaderId = 5, ApplicationFlagHeaderNameAr = "الأصناف", ApplicationFlagHeaderNameEn = "Items", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 6, ApplicationFlagHeaderId = 6, ApplicationFlagHeaderNameAr = "فاتورة المشتريات", ApplicationFlagHeaderNameEn = "Purchase Invoice", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 7, ApplicationFlagHeaderId = 7, ApplicationFlagHeaderNameAr = "فاتورة مرتجع المشتريات", ApplicationFlagHeaderNameEn = "Purchase Invoice Return", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 8, ApplicationFlagHeaderId = 8, ApplicationFlagHeaderNameAr = "عرض أسعار العميل", ApplicationFlagHeaderNameEn = "Client Quotation", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 9, ApplicationFlagHeaderId = 9, ApplicationFlagHeaderNameAr = "فاتورة المبيعات", ApplicationFlagHeaderNameEn = "Sales Invoice", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 10, ApplicationFlagHeaderId = 10, ApplicationFlagHeaderNameAr = "فاتورة مرتجع المبيعات", ApplicationFlagHeaderNameEn = "Sales Invoice Return", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 11, ApplicationFlagHeaderId = 11, ApplicationFlagHeaderNameAr = "إعدادات عامة", ApplicationFlagHeaderNameEn = "General Settings", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 12, ApplicationFlagHeaderNameAr = "إعدادات أساسية", ApplicationFlagHeaderNameEn = "Basic Settings", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 13, ApplicationFlagHeaderNameAr = "ملاحظات إضافية", ApplicationFlagHeaderNameEn = "Additional Notes", Order = 2 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 14, ApplicationFlagHeaderNameAr = "إضافات أخري", ApplicationFlagHeaderNameEn = "Other Additions", Order = 3 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 15, ApplicationFlagHeaderNameAr = "حجم الخط", ApplicationFlagHeaderNameEn = "Font Size", Order = 4 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 16, ApplicationFlagHeaderNameAr = "تقريب الكسر العشري", ApplicationFlagHeaderNameEn = "Rounding a decimal", Order = 5 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 17, ApplicationFlagHeaderNameAr = "هيئة الطباعة", ApplicationFlagHeaderNameEn = "Printing Form", Order = 6 }, //report1 report2 print 
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 18, ApplicationFlagHeaderNameAr = "صور مرفقة", ApplicationFlagHeaderNameEn = "Images", Order = 7 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 12, ApplicationFlagHeaderId = 19, ApplicationFlagHeaderNameAr = "حجم الهوامش", ApplicationFlagHeaderNameEn = "margin Size", Order = 8 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 13, ApplicationFlagHeaderId = 20, ApplicationFlagHeaderNameAr = "إعدادات أساسية", ApplicationFlagHeaderNameEn = "Basic Settings", Order = 1 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 13, ApplicationFlagHeaderId = 21, ApplicationFlagHeaderNameAr = "إضافات أخري", ApplicationFlagHeaderNameEn = "Other Additions", Order = 2 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 13, ApplicationFlagHeaderId = 22, ApplicationFlagHeaderNameAr = "تقريب الكسر العشري", ApplicationFlagHeaderNameEn = "Rounding a decimal", Order = 3 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 13, ApplicationFlagHeaderId = 23, ApplicationFlagHeaderNameAr = "صور مرفقة", ApplicationFlagHeaderNameEn = "Images", Order = 4 },
				new ApplicationFlagHeader { ApplicationFlagTabId = 13, ApplicationFlagHeaderId = 24, ApplicationFlagHeaderNameAr = "إضافة ملاحظات", ApplicationFlagHeaderNameEn = "Notes Addition", Order = 5 }

			);

			//ApplicationFlagDetail
			builder.Entity<ApplicationFlagDetail>().HasData(
				new ApplicationFlagDetail { ApplicationFlagDetailId = 1, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الأول بالعربية", FlagNameEn = "First Level Ar", FlagValue = "المستوى الأول", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 2, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الأول بالانجليزية", FlagNameEn = "First Level En", FlagValue = "First Level", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 3, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الثاني بالعربية", FlagNameEn = "Second Level Ar", FlagValue = "المستوى الثاني", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 4, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الثاني بالانجليزية", FlagNameEn = "Second Level En", FlagValue = "Second Level", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 5, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الثالث بالعربية", FlagNameEn = "Third Level Ar", FlagValue = "المستوى الثالث", Order = 5, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 6, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الثالث بالانجليزية", FlagNameEn = "Third Level En", FlagValue = "Third Level", Order = 6, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 7, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الرابع بالعربية", FlagNameEn = "Forth Level Ar", FlagValue = "المستوى الرابع", Order = 7, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 8, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الرابع بالانجليزية", FlagNameEn = "Forth Level En", FlagValue = "Forth Level", Order = 8, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 9, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الخامس بالعربية", FlagNameEn = "Fifth Level Ar", FlagValue = "المستوى الخامس", Order = 9, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 10, ApplicationFlagHeaderId = 1, FlagNameAr = "المستوى الخامس بالانجليزية", FlagNameEn = "Fifth Level En", FlagValue = "Fifth Level", Order = 10, ApplicationFlagTypeId = ApplicationFlagTypeData.String },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 11, ApplicationFlagHeaderId = 2, FlagNameAr = "عدد خانات التسلسل للحسابات العامة", FlagNameEn = "Number of Code Width for Main Accounts", FlagValue = "2", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 12, ApplicationFlagHeaderId = 2, FlagNameAr = "عدد خانات التسلسل للحسابات الجزئية", FlagNameEn = "Number of Code Width for Individual Accounts", FlagValue = "4", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 13, ApplicationFlagHeaderId = 3, FlagNameAr = "عدد خانات التسلسل للمراكز العامة", FlagNameEn = "Number of Code Width for Main Centers", FlagValue = "2", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 14, ApplicationFlagHeaderId = 3, FlagNameAr = "عدد خانات التسلسل للمراكز الجزئية", FlagNameEn = "Number of Code Width for Individual Centers", FlagValue = "4", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 15, ApplicationFlagHeaderId = 4, FlagNameAr = "طريقة احتساب سعر التكلفة للصنف", FlagNameEn = "Item Cost Price Calculation Method", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Select },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 16, ApplicationFlagHeaderId = 5, FlagNameAr = "الأسعار تشمل ضريبة القيمة المضافة", FlagNameEn = "Prices VAT Inclusive", FlagValue = "0", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 17, ApplicationFlagHeaderId = 6, FlagNameAr = "فصل تسلسل النقدي عن الآجل", FlagNameEn = "Separating the sequence of cash from credit", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 18, ApplicationFlagHeaderId = 6, FlagNameAr = "عدد الأيام لإرتجاع الفاتورة", FlagNameEn = "No of days to return the invoice", FlagValue = "0", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 19, ApplicationFlagHeaderId = 7, FlagNameAr = "فصل تسلسل النقدي عن الآجل", FlagNameEn = "Separating the sequence of cash from credit", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 20, ApplicationFlagHeaderId = 8, FlagNameAr = "عدد أيام صلاحية العرض", FlagNameEn = "No of days the quotation is valid", FlagValue = "0", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 21, ApplicationFlagHeaderId = 9, FlagNameAr = "فصل تسلسل النقدي عن الآجل", FlagNameEn = "Separating the sequence of cash from credit", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 22, ApplicationFlagHeaderId = 9, FlagNameAr = "عدد الأيام لإرتجاع الفاتورة", FlagNameEn = "No of days to return the invoice", FlagValue = "0", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 23, ApplicationFlagHeaderId = 10, FlagNameAr = "فصل تسلسل النقدي عن الآجل", FlagNameEn = "Separating the sequence of cash from credit", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 24, ApplicationFlagHeaderId = 9, FlagNameAr = "فصل تسلسل الفواتير لكل مندوب", FlagNameEn = "Separating the sequence of invoices for each seller", FlagValue = "0", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 25, ApplicationFlagHeaderId = 11, FlagNameAr = "عرض الأصناف مجمعة", FlagNameEn = "Show Items Grouped", FlagValue = "0", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },

				new ApplicationFlagDetail { ApplicationFlagDetailId = 26, ApplicationFlagHeaderId = 11, FlagNameAr = "فصل تسلسل أرقام السندات لكل سنة", FlagNameEn = "Separating the sequence of documents every year", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 27, ApplicationFlagHeaderId = 12, FlagNameAr = "اسم المؤسسة بالعربية", FlagNameEn = "Name of the institution Ar", FlagValue = "", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 28, ApplicationFlagHeaderId = 12, FlagNameAr = "اسم المؤسسة بالانجليزية", FlagNameEn = "Name of the institution En", FlagValue = "", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 29, ApplicationFlagHeaderId = 12, FlagNameAr = "اسم آخر أسفل اسم المؤسسة بالعربية", FlagNameEn = "Another name below the name of the institution Ar", FlagValue = "", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 30, ApplicationFlagHeaderId = 12, FlagNameAr = "اسم آخر أسفل اسم المؤسسة بالانجليزية", FlagNameEn = "Another name below the name of the institution En", FlagValue = "", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 31, ApplicationFlagHeaderId = 12, FlagNameAr = "عنوان 1 بالعربية", FlagNameEn = "Address 1 Ar", FlagValue = "", Order = 5, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 32, ApplicationFlagHeaderId = 12, FlagNameAr = "عنوان 1 بالانجليزية", FlagNameEn = "Address 1 En", FlagValue = "", Order = 6, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 33, ApplicationFlagHeaderId = 12, FlagNameAr = "عنوان 2 بالعربية", FlagNameEn = "Address 2 Ar", FlagValue = "", Order = 7, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 34, ApplicationFlagHeaderId = 12, FlagNameAr = "عنوان 2 بالانجليزية", FlagNameEn = "Address 2 En", FlagValue = "", Order = 8, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 35, ApplicationFlagHeaderId = 12, FlagNameAr = "عنوان 3 بالعربية", FlagNameEn = "Address 3 Ar", FlagValue = "", Order = 9, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 36, ApplicationFlagHeaderId = 12, FlagNameAr = "عنوان 3 بالانجليزية", FlagNameEn = "Address 3 En", FlagValue = "", Order = 10, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 37, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات علوية في أول صفحة فقط بالعربية", FlagNameEn = "Top notes on first page only Ar", FlagValue = "", Order = 11, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 38, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات علوية في أول صفحة فقط بالانجليزية", FlagNameEn = "Top notes on first page only En", FlagValue = "", Order = 12, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 39, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات سفلية في آخر صفحة فقط بالعربية", FlagNameEn = "Footnotes on last page only Ar", FlagValue = "", Order = 13, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 40, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات سفلية في آخر صفحة فقط بالانجليزية", FlagNameEn = "Footnotes on last page only En", FlagValue = "", Order = 14, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 41, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات علوية متكررة في كل الصفحات بالعربية", FlagNameEn = "Repeated top notes on all pages Ar", FlagValue = "", Order = 15, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 42, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات علوية متكررة في كل الصفحات بالانجليزية", FlagNameEn = "Repeated top notes on all pages En", FlagValue = "", Order = 16, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 43, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات سفلية متكررة في كل الصفحات بالعربية", FlagNameEn = "Repeated footnotes on all pages Ar", FlagValue = "", Order = 17, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 44, ApplicationFlagHeaderId = 13, FlagNameAr = "ملاحظات سفلية متكررة في كل الصفحات بالانجليزية", FlagNameEn = "Repeated footnotes on all pages En", FlagValue = "", Order = 18, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 45, ApplicationFlagHeaderId = 14, FlagNameAr = "طباعة اسم النشاط والموقع؟", FlagNameEn = "Print Business Name And Store?", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 46, ApplicationFlagHeaderId = 14, FlagNameAr = "طباعة اسم المستخدم؟", FlagNameEn = "Print User Name?", FlagValue = "0", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 47, ApplicationFlagHeaderId = 14, FlagNameAr = "طباعة تاريخ اليوم؟", FlagNameEn = "Print datetime?", FlagValue = "1", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 48, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط لاسم المؤسسة", FlagNameEn = "Font size for the institution name", FlagValue = "16", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 49, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للاسم الآخر للمؤسسة", FlagNameEn = "Font size for other name of the institution", FlagValue = "14", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 50, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للنشاط والموقع", FlagNameEn = "Font size for business and store name", FlagValue = "14", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 51, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للعنوان الأول", FlagNameEn = "Font size for first address", FlagValue = "14", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 52, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للعنوان الثاني", FlagNameEn = "Font size for second address", FlagValue = "14", Order = 5, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 53, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للعنوان الثالث", FlagNameEn = "Font size for third address", FlagValue = "14", Order = 6, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 54, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط لعنوان التقرير", FlagNameEn = "Font size for report title", FlagValue = "16", Order = 7, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 55, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للجدول", FlagNameEn = "Font size for table", FlagValue = "12", Order = 9, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 56, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للملاحظات العلوية الثابتة", FlagNameEn = "Font size for fixed top notes", FlagValue = "12", Order = 10, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 57, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للملاحظات العلوية المتكررة", FlagNameEn = "Font size for repeated top notes", FlagValue = "12", Order = 11, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 58, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للملاحظات السفلية الثابتة", FlagNameEn = "Font size for fixed footnotes", FlagValue = "12", Order = 12, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 59, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط للملاحظات السفلية المتكررة", FlagNameEn = "Font size for repeated footnotes", FlagValue = "12", Order = 13, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 60, ApplicationFlagHeaderId = 16, FlagNameAr = "الأرقام في الجدول", FlagNameEn = "Numbers in grid", FlagValue = "2", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 61, ApplicationFlagHeaderId = 16, FlagNameAr = "الأرقام في المجاميع", FlagNameEn = "Numbers in summation", FlagValue = "2", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 62, ApplicationFlagHeaderId = 17, FlagNameAr = "هيئة الورقة أثناء الطباعة", FlagNameEn = "Paper Form During Printing", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Select },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 63, ApplicationFlagHeaderId = 18, FlagNameAr = "شعار المؤسسة", FlagNameEn = "Institution Logo", FlagValue = "", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.UploadImage },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 64, ApplicationFlagHeaderId = 15, FlagNameAr = "حجم الخط لفترة التقرير", FlagNameEn = "Font size for report period", FlagValue = "14", Order = 8, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 65, ApplicationFlagHeaderId = 19, FlagNameAr = "الهامش العلوي", FlagNameEn = "Top Margin", FlagValue = "40", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 66, ApplicationFlagHeaderId = 19, FlagNameAr = "هامش الجانب الأيمن", FlagNameEn = "Right Margin", FlagValue = "10", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 67, ApplicationFlagHeaderId = 19, FlagNameAr = "الهامش السفلي", FlagNameEn = "Bottom Margin", FlagValue = "40", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 68, ApplicationFlagHeaderId = 19, FlagNameAr = "هامش الجانب الأيسر", FlagNameEn = "Left Margin", FlagValue = "10", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 69, ApplicationFlagHeaderId = 14, FlagNameAr = "طباعة شعار المؤسسة؟", FlagNameEn = "Print Institution Logo?", FlagValue = "1", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 70, ApplicationFlagHeaderId = 20, FlagNameAr = "اسم المؤسسة بالعربية", FlagNameEn = "Institution Name Ar", FlagValue = "", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 71, ApplicationFlagHeaderId = 20, FlagNameAr = "اسم المؤسسة بالانجليزية", FlagNameEn = "Institution Name En", FlagValue = "", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 72, ApplicationFlagHeaderId = 20, FlagNameAr = "عنوان المؤسسة بالعربية", FlagNameEn = "Institution address Ar", FlagValue = "", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 73, ApplicationFlagHeaderId = 20, FlagNameAr = "عنوان المؤسسة بالانجليزية", FlagNameEn = "Institution address En", FlagValue = "", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 74, ApplicationFlagHeaderId = 20, FlagNameAr = "بيانات التواصل للمؤسسة بالعربية", FlagNameEn = "Institution contact info Ar", FlagValue = "", Order = 5, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 75, ApplicationFlagHeaderId = 20, FlagNameAr = "بيانات التواصل للمؤسسة بالانجليزية", FlagNameEn = "Institution contact info En", FlagValue = "", Order = 6, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 76, ApplicationFlagHeaderId = 20, FlagNameAr = "اسم المتجر بالعربية", FlagNameEn = "Store Name Ar", FlagValue = "", Order = 7, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 77, ApplicationFlagHeaderId = 20, FlagNameAr = "اسم المتجر بالانجليزية", FlagNameEn = "Store Name En", FlagValue = "", Order = 8, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 78, ApplicationFlagHeaderId = 20, FlagNameAr = "عنوان المتجر بالعربية", FlagNameEn = "Store Address Ar", FlagValue = "", Order = 9, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 79, ApplicationFlagHeaderId = 20, FlagNameAr = "عنوان المتجر بالانجليزية", FlagNameEn = "Store Address En", FlagValue = "", Order = 10, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 80, ApplicationFlagHeaderId = 20, FlagNameAr = "الرقم الضريبي", FlagNameEn = "VAT no", FlagValue = "", Order = 11, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 81, ApplicationFlagHeaderId = 21, FlagNameAr = "عرض طريقة التحصيل؟", FlagNameEn = "Show collection method?", FlagValue = "1", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 82, ApplicationFlagHeaderId = 21, FlagNameAr = "عرض تاريخ الاستحقاق؟", FlagNameEn = "Show due date?", FlagValue = "1", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 83, ApplicationFlagHeaderId = 21, FlagNameAr = "عرض وقت الطباعة؟", FlagNameEn = "Show print time?", FlagValue = "1", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Boolean },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 84, ApplicationFlagHeaderId = 22, FlagNameAr = "الأرقام في الجدول", FlagNameEn = "Numbers in grid", FlagValue = "2", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 85, ApplicationFlagHeaderId = 22, FlagNameAr = "الأرقام في المجاميع", FlagNameEn = "Numbers in summation", FlagValue = "2", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.Number },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 86, ApplicationFlagHeaderId = 23, FlagNameAr = "شعار المؤسسة", FlagNameEn = "Institution Logo", FlagValue = "", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.UploadImage },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 87, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة أساسية 1", FlagNameEn = "Basic Note 1", FlagValue = "", Order = 1, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 88, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة أساسية 2", FlagNameEn = "Basic Note 2", FlagValue = "", Order = 2, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 89, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة أساسية 3", FlagNameEn = "Basic Note 3", FlagValue = "", Order = 3, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 90, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 1", FlagNameEn = "Side Note 1", FlagValue = "", Order = 4, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 91, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 2", FlagNameEn = "Side Note 2", FlagValue = "", Order = 5, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 92, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 3", FlagNameEn = "Side Note 3", FlagValue = "", Order = 6, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 93, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 4", FlagNameEn = "Side Note 4", FlagValue = "", Order = 7, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 94, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 5", FlagNameEn = "Side Note 5", FlagValue = "", Order = 8, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 95, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 6", FlagNameEn = "Side Note 6", FlagValue = "", Order = 9, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 96, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 7", FlagNameEn = "Side Note 7", FlagValue = "", Order = 10, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 97, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 8", FlagNameEn = "Side Note 8", FlagValue = "", Order = 11, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 98, ApplicationFlagHeaderId = 24, FlagNameAr = "ماحظة جانبية 9", FlagNameEn = "Side Note 9", FlagValue = "", Order = 12, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 99, ApplicationFlagHeaderId = 24, FlagNameAr = "ملاحظة جانبية 10", FlagNameEn = "Side Note 10", FlagValue = "", Order = 13, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 100, ApplicationFlagHeaderId = 24, FlagNameAr = "سطر ختامي 1", FlagNameEn = "Closing line 1", FlagValue = "", Order = 14, ApplicationFlagTypeId = ApplicationFlagTypeData.String },
				new ApplicationFlagDetail { ApplicationFlagDetailId = 101, ApplicationFlagHeaderId = 24, FlagNameAr = "سطر ختامي 2", FlagNameEn = "Closing line 2", FlagValue = "", Order = 15, ApplicationFlagTypeId = ApplicationFlagTypeData.String }
			);

			//ApplicationFlagDetailSelect
			builder.Entity<ApplicationFlagDetailSelect>().HasData(
				new ApplicationFlagDetailSelect { ApplicationFlagDetailSelectId = 1, ApplicationFlagDetailId = 15, SelectId = 1, SelectNameAr = "المتوسط الفعلي", SelectNameEn = "Actual average", Order = 1 },
				new ApplicationFlagDetailSelect { ApplicationFlagDetailSelectId = 2, ApplicationFlagDetailId = 15, SelectId = 2, SelectNameAr = "آخر سعر شراء", SelectNameEn = "last Purchasing Price", Order = 2 },
				new ApplicationFlagDetailSelect { ApplicationFlagDetailSelectId = 3, ApplicationFlagDetailId = 15, SelectId = 3, SelectNameAr = "آخر سعر تكلفة", SelectNameEn = "last Cost Price", Order = 3 },
				new ApplicationFlagDetailSelect { ApplicationFlagDetailSelectId = 4, ApplicationFlagDetailId = 62, SelectId = 1, SelectNameAr = "رسمي", SelectNameEn = "Formal", Order = 1 },
				new ApplicationFlagDetailSelect { ApplicationFlagDetailSelectId = 5, ApplicationFlagDetailId = 62, SelectId = 2, SelectNameAr = "شبه رسمي", SelectNameEn = "Semi-Formal", Order = 2 }
			);

			//ReportPrintForm
			builder.Entity<ReportPrintForm>().HasData(
				new ReportPrintForm { ReportPrintFormId = 1, ReportPrintFormNameAr = "رسمي", ReportPrintFormNameEn = "Formal" },
				new ReportPrintForm { ReportPrintFormId = 2, ReportPrintFormNameAr = "شبه رسمي", ReportPrintFormNameEn = "Semi-Formal" }
			);


			//ApproveRequestType
			builder.Entity<ApproveRequestType>().HasData(
				new ApproveRequestType { ApproveRequestTypeId = 1, ApproveRequestTypeNameAr = "إضافة جديد", ApproveRequestTypeNameEn = "Create New" },
				new ApproveRequestType { ApproveRequestTypeId = 2, ApproveRequestTypeNameAr = "تعديل البيانات", ApproveRequestTypeNameEn = "Edit Data" },
				new ApproveRequestType { ApproveRequestTypeId = 3, ApproveRequestTypeNameAr = "حذف", ApproveRequestTypeNameEn = "Delete" }
			);


			//ItemPackage
			builder.Entity<ItemPackage>().HasData(
				new ItemPackage { ItemPackageId = 1, ItemPackageCode = 1, PackageNameAr = "حبة", PackageNameEn = "each" }
			);

		}
	}
}
