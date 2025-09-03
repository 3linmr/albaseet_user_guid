using Accounting.CoreOne.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.FixedAssets;

namespace Shared.Repository.Seed
{
	public static class AccountingModels
	{
		public static void Seed(ModelBuilder builder)
		{
			//AccountStoreType
			builder.Entity<AccountStoreType>().HasData(
				new AccountStoreType { AccountStoreTypeId = 1, AccountStoreTypeNameAr = "مخزون أول مدة", AccountStoreTypeNameEn = "Beginning Inventory" },
				new AccountStoreType { AccountStoreTypeId = 2, AccountStoreTypeNameAr = "مخزون آخر مدة", AccountStoreTypeNameEn = "Ending Inventory" },
				new AccountStoreType { AccountStoreTypeId = 3, AccountStoreTypeNameAr = "المخزون", AccountStoreTypeNameEn = "Inventory" },
				new AccountStoreType { AccountStoreTypeId = 4, AccountStoreTypeNameAr = "تكلفة الإيرادات", AccountStoreTypeNameEn = "Cost of Revenue" },
				new AccountStoreType { AccountStoreTypeId = 5, AccountStoreTypeNameAr = "أرباح وخسائر", AccountStoreTypeNameEn = "Profit and Loss" }
			);

			//TransactionType
			builder.Entity<TransactionType>().HasData(
				new TransactionType { TransactionTypeId = 1, TransactionTypeNameAr = "مدين", TransactionTypeNameEn = "Debit" },
				new TransactionType { TransactionTypeId = 2, TransactionTypeNameAr = "دائن", TransactionTypeNameEn = "Credit" }
			);

			//AccountLedger
			builder.Entity<AccountLedger>().HasData(
				new AccountLedger { AccountLedgerId = 1, AccountLedgerNameAr = "مركز مالي", AccountLedgerNameEn = "Balance Sheet" },
				new AccountLedger { AccountLedgerId = 2, AccountLedgerNameAr = "قائمة دخل", AccountLedgerNameEn = "Income Statement" }
			);

			//AccountCategory
			builder.Entity<AccountCategory>().HasData(
				new AccountCategory { AccountCategoryId = 1, AccountLedgerId = 1, AccountCategoryNameAr = "الأصول", AccountCategoryNameEn = "Assets" },
				new AccountCategory { AccountCategoryId = 2, AccountLedgerId = 1, AccountCategoryNameAr = "الخصوم", AccountCategoryNameEn = "Liabilities" },
				new AccountCategory { AccountCategoryId = 3, AccountLedgerId = 2, AccountCategoryNameAr = "المصروفات", AccountCategoryNameEn = "Expenses" },
				new AccountCategory { AccountCategoryId = 4, AccountLedgerId = 2, AccountCategoryNameAr = "الايرادات", AccountCategoryNameEn = "Revenues" }
			);

			//AccountType
			builder.Entity<AccountType>().HasData(
				new AccountType { AccountTypeId = 1, AccountTypeNameAr = "النقدية", AccountTypeNameEn = "Cash", IsInternalSystem = false },
				new AccountType { AccountTypeId = 2, AccountTypeNameAr = "العملاء", AccountTypeNameEn = "Clients", IsInternalSystem = false },
				new AccountType { AccountTypeId = 3, AccountTypeNameAr = "الموردين", AccountTypeNameEn = "Suppliers", IsInternalSystem = false },
				new AccountType { AccountTypeId = 4, AccountTypeNameAr = "البنوك", AccountTypeNameEn = "Banks", IsInternalSystem = false },
				new AccountType { AccountTypeId = 5, AccountTypeNameAr = "فرق تقريب كسور", AccountTypeNameEn = "Fractional Approximation Difference", IsInternalSystem = true },
				new AccountType { AccountTypeId = 6, AccountTypeNameAr = "أصول ثابتة", AccountTypeNameEn = "Fixed Assets", IsInternalSystem = false },
				new AccountType { AccountTypeId = 7, AccountTypeNameAr = "مجمع الاهلاك", AccountTypeNameEn = "Accumulated Depreciation", IsInternalSystem = false },
				new AccountType { AccountTypeId = 8, AccountTypeNameAr = "الاهلاكات", AccountTypeNameEn = "Depreciation", IsInternalSystem = false },
				new AccountType { AccountTypeId = 9, AccountTypeNameAr = "حقوق الملكية", AccountTypeNameEn = "Ownership Equity", IsInternalSystem = false },
				new AccountType { AccountTypeId = 10, AccountTypeNameAr = "المشتريات", AccountTypeNameEn = "Purchases", IsInternalSystem = false },
				new AccountType { AccountTypeId = 11, AccountTypeNameAr = "تكلفة الايرادات", AccountTypeNameEn = "Revenues Cost", IsInternalSystem = false },
				new AccountType { AccountTypeId = 12, AccountTypeNameAr = "المبيعات", AccountTypeNameEn = "Sales", IsInternalSystem = false },
				new AccountType { AccountTypeId = 13, AccountTypeNameAr = "مصروفات عمومية وادارية", AccountTypeNameEn = "Miscellaneous Expenses", IsInternalSystem = false },
				new AccountType { AccountTypeId = 14, AccountTypeNameAr = "ايرادات متنوعة", AccountTypeNameEn = "Miscellaneous Income", IsInternalSystem = false },
				new AccountType { AccountTypeId = 15, AccountTypeNameAr = "خصم مسموح به", AccountTypeNameEn = "Allowed Discount", IsInternalSystem = true },
				new AccountType { AccountTypeId = 16, AccountTypeNameAr = "المخزون", AccountTypeNameEn = "Inventory", IsInternalSystem = false },
				new AccountType { AccountTypeId = 17, AccountTypeNameAr = "حساب المخزون", AccountTypeNameEn = "Inventory Account", IsInternalSystem = true },
				new AccountType { AccountTypeId = 18, AccountTypeNameAr = "حساب تكلفة المبيعات", AccountTypeNameEn = "Revenues Cost Account", IsInternalSystem = true },
				new AccountType { AccountTypeId = 50, AccountTypeNameAr = "أخري", AccountTypeNameEn = "Other", IsInternalSystem = false }
			);

			//JournalType
			builder.Entity<JournalType>().HasData(
				new JournalType { JournalTypeId = 1, JournalTypeNameAr = "رصيد افتتاحي", JournalTypeNameEn = "Opening Balance" },
				new JournalType { JournalTypeId = 2, JournalTypeNameAr = "فاتورة بيع نقداً", JournalTypeNameEn = "Cash Sales Invoice" },
				new JournalType { JournalTypeId = 3, JournalTypeNameAr = "فاتورة بيع آجل", JournalTypeNameEn = "Credit Sales Invoice" },
				new JournalType { JournalTypeId = 4, JournalTypeNameAr = "فاتورة مرتجع بيع نقداً", JournalTypeNameEn = "Cash Invoice Return" },
				new JournalType { JournalTypeId = 5, JournalTypeNameAr = "فاتورة مرتجع بيع آجل", JournalTypeNameEn = "Credit Sales Invoice Return" },
				new JournalType { JournalTypeId = 6, JournalTypeNameAr = "فاتورة شراء نقداً", JournalTypeNameEn = "Cash Purchase Invoice" },
				new JournalType { JournalTypeId = 7, JournalTypeNameAr = "فاتورة شراء آجل", JournalTypeNameEn = "Credit Purchase Invoice" },
				new JournalType { JournalTypeId = 8, JournalTypeNameAr = "فاتورة مرتجع شراء نقداً", JournalTypeNameEn = "Cash Purchase Invoice Return" },
				new JournalType { JournalTypeId = 9, JournalTypeNameAr = "فاتورة مرتجع شراء آجل", JournalTypeNameEn = "Credit Purchase Invoice Return" },
				new JournalType { JournalTypeId = 10, JournalTypeNameAr = "سند صرف شيكات", JournalTypeNameEn = "Check Cashing Voucher" },
				new JournalType { JournalTypeId = 11, JournalTypeNameAr = "قيد يومية", JournalTypeNameEn = "Journal Entry" },
				new JournalType { JournalTypeId = 12, JournalTypeNameAr = "سند قبض", JournalTypeNameEn = "Receipt Voucher" },
				new JournalType { JournalTypeId = 13, JournalTypeNameAr = "سند قبض إيجار", JournalTypeNameEn = "Rent Receipt Voucher" },
				new JournalType { JournalTypeId = 14, JournalTypeNameAr = "سند صرف", JournalTypeNameEn = "Cashing Voucher" },
				new JournalType { JournalTypeId = 15, JournalTypeNameAr = "فاتورة صرف", JournalTypeNameEn = "Cashing Invoice" },
				new JournalType { JournalTypeId = 16, JournalTypeNameAr = "فاتورة استلام", JournalTypeNameEn = "Receipt Invoice" },
				new JournalType { JournalTypeId = 17, JournalTypeNameAr = "قيد ايجار", JournalTypeNameEn = "Rent Entry" },
				new JournalType { JournalTypeId = 18, JournalTypeNameAr = "إشعار مدين المورد", JournalTypeNameEn = "Supplier Debit Notice" },
				new JournalType { JournalTypeId = 19, JournalTypeNameAr = "إشعار دائن المورد", JournalTypeNameEn = "Supplier Credit Notice" },
				new JournalType { JournalTypeId = 20, JournalTypeNameAr = "إشعار مدين للعميل", JournalTypeNameEn = "Client Debit Notice" },
				new JournalType { JournalTypeId = 21, JournalTypeNameAr = "إشعار دائن للعميل", JournalTypeNameEn = "Client Credit Notice" },
				new JournalType { JournalTypeId = 22, JournalTypeNameAr = "أصل ثابت", JournalTypeNameEn = "Fixed Asset" },
				new JournalType { JournalTypeId = 23, JournalTypeNameAr = "فاتورة مشتريات بضاعة بالطريق نقداً", JournalTypeNameEn = "Purchase Invoice On The Way Cash" },
				new JournalType { JournalTypeId = 24, JournalTypeNameAr = "فاتورة مشتريات بضاعة بالطريق آجل", JournalTypeNameEn = "Purchase Invoice On The Way Credit" },
				new JournalType { JournalTypeId = 25, JournalTypeNameAr = "فاتورة مشتريات مرحلية", JournalTypeNameEn = "Purchase Invoice Interim" },
				new JournalType { JournalTypeId = 26, JournalTypeNameAr = "فاتورة بيع حجز نقدي", JournalTypeNameEn = "Cash Reservation Sales Invoice" },
				new JournalType { JournalTypeId = 27, JournalTypeNameAr = "فاتورة بيع حجز آجل", JournalTypeNameEn = "Credit Reservation Sales Invoice" },
				new JournalType { JournalTypeId = 28, JournalTypeNameAr = "فاتورة بيع مرحلية", JournalTypeNameEn = "Sales Invoice Interim" },
				new JournalType { JournalTypeId = 29, JournalTypeNameAr = "فاتورة مرتجع شراء بضاعة بالطريق", JournalTypeNameEn = "Purchase Invoice Return On The Way" },
				new JournalType { JournalTypeId = 30, JournalTypeNameAr = "فاتورة مرتجع ما بعد الشراء", JournalTypeNameEn = "Purchase Invoice Return" },
				new JournalType { JournalTypeId = 31, JournalTypeNameAr = "فاتورة مرتجع بيع لتصفية الحجز", JournalTypeNameEn = "Reservation Sales Invoice Close Out" },
				new JournalType { JournalTypeId = 32, JournalTypeNameAr = "فاتورة مرتجع ما بعد البيع", JournalTypeNameEn = "Sales Invoice Return" },
				new JournalType { JournalTypeId = 33, JournalTypeNameAr = "اهلاك اصل ثابت", JournalTypeNameEn = "Fixed Asset Depreciation" },
				new JournalType { JournalTypeId = 34, JournalTypeNameAr = "استبعاد اصل ثابت", JournalTypeNameEn = "Fixed Asset Exclusion" }

                    );


			//FixedAssetVoucherType
			builder.Entity<FixedAssetVoucherType>().HasData(
				new FixedAssetVoucherType { FixedAssetVoucherTypeId = 1, FixedAssetVoucherTypeNameAr = "إضافة", FixedAssetVoucherTypeNameEn = "Addition" },
				new FixedAssetVoucherType { FixedAssetVoucherTypeId = 2, FixedAssetVoucherTypeNameAr = "إستبعاد", FixedAssetVoucherTypeNameEn = "Exclusion" },
				new FixedAssetVoucherType { FixedAssetVoucherTypeId = 3, FixedAssetVoucherTypeNameAr = "إهلاك", FixedAssetVoucherTypeNameEn = "Depreciation" }
			);
		}
	}
}
