using Microsoft.Extensions.Hosting;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
	public static class ApplicationSettingData
	{

	}

	public static class ApplicationFlagTabData
	{
		public const int PrintSetting = 12;
		public const int PrintInvoiceSetting = 13;
	}

	public static class ApplicationSettingHeaderData
	{
		public const int ItemCategories = 1;
		public const int AccountsEncoding = 2;
		public const int CostCentersEncoding = 3;
	}
	public static class ApplicationSettingDetailData
	{
		public const int ItemCategoryAr = 1;
		public const int ItemCategoryEn = 2;
		public const int ItemSubCategoryAr = 3;
		public const int ItemSubCategoryEn = 4;
		public const int ItemSectionAr = 5;
		public const int ItemSectionEn = 6;
		public const int ItemSubSectionAr = 7;
		public const int ItemSubSectionEn = 8;
		public const int MainItemAr = 9;
		public const int MainItemEn = 10;
		public const int MainAccount = 11;
		public const int IndividualAccount = 12;
		public const int MainCostCenter = 13;
		public const int IndividualCostCenter = 14;
		public const int ItemCostPriceCalculationMethod = 15;
		public const int ItemsVatInclusive = 16;
		public const int SeparatingCashFromCredit = 17;
		public const int DaysToReturnPurchaseInvoice = 18;
		public const int SeparatingCashFromCreditInReturn = 19;
		public const int DaysToQuotationIsValid = 20;
        public const int SeparatingCashFromCreditSales = 21;
        public const int DaysToReturnSalesInvoice = 22;
        public const int SeparatingCashFromCreditInReturnSales = 23;
		public const int SeparatingTheSequenceOfInvoicesForEachSeller = 24;
		public const int ShowItemsGrouped = 25;
		public const int DocumentsSequence = 26;
		public const int InstitutionNameAr = 27;
		public const int InstitutionNameEn = 28;
		public const int InstitutionOtherNameAr = 29;
		public const int InstitutionOtherNameEn = 30;
		public const int Address1Ar = 31;
		public const int Address1En = 32;
		public const int Address2Ar = 33;
		public const int Address2En = 34;
		public const int Address3Ar = 35;
		public const int Address3En = 36;
		public const int TopNotesFirstPageOnlyAr = 37;
		public const int TopNotesFirstPageOnlyEn = 38;
		public const int BottomNotesLastPageOnlyAr = 39;
		public const int BottomNotesLastPageOnlyEn = 40;
		public const int TopNotesAllPagesAr = 41;
		public const int TopNotesAllPagesEn = 42;
		public const int BottomNotesAllPagesAr = 43;
		public const int BottomNotesAllPagesEn = 44;
		public const int PrintBusinessName = 45;
		public const int PrintUserName = 46;
		public const int PrintDateTime = 47;
		public const int InstitutionNameFont = 48;
		public const int InstitutionOtherNameFont = 49;
		public const int BusinessFont = 50;
		public const int Address1Font = 51;
		public const int Address2Font = 52;
		public const int Address3Font = 53;
		public const int ReportNameFont = 54;
		public const int GridFont = 55;
		public const int TopNotesFirstPageOnlyFont = 56;
		public const int TopNotesAllPagesFont = 57;
		public const int BottomNotesFirstPageOnlyFont = 58;
		public const int BottomNotesAllPagesFont = 59;
		public const int DetailRounding = 60;
		public const int SumRounding = 61;
		public const int PrintFormId = 62;
		public const int Logo = 63;
		public const int ReportPeriodFont = 64;
		public const int TopMargin = 65;
		public const int RightMargin = 66;
		public const int BottomMargin = 67;
		public const int LeftMargin = 68;
		public const int PrintInstitutionLogo = 69;
		public const int InvoiceInstitutionNameAr = 70;
		public const int InvoiceInstitutionNameEn = 71;
		public const int InvoiceInstitutionAddressAr = 72;
		public const int InvoiceInstitutionAddressEn = 73;
		public const int InvoiceInstitutionContactInfoAr = 74;
		public const int InvoiceInstitutionContactInfoEn = 75;
		public const int StoreNameAr = 76;
		public const int StoreNameEn = 77;
		public const int StoreAddressAr = 78;
		public const int StoreAddressEn = 79;
		public const int VatNo = 80;
		public const int ShowCollectionMethod = 81;
		public const int ShowDueDate = 82;
		public const int ShowPrintTime = 83;
		public const int NumbersInGrid = 84;
		public const int NumbersInSummation = 85;
		public const int InstitutionLogo = 86;
		public const int BasicNote1 = 87;
		public const int BasicNote2 = 88;
		public const int BasicNote3 = 89;
		public const int SideNote1 = 90;
		public const int SideNote2 = 91;
		public const int SideNote3 = 92;
		public const int SideNote4 = 93;
		public const int SideNote5 = 94;
		public const int SideNote6 = 95;
		public const int SideNote7 = 96;
		public const int SideNote8 = 97;
		public const int SideNote9 = 98;
		public const int SideNote10 = 99;
		public const int ClosingLine1 = 100;
		public const int ClosingLine2 = 101;
	}

	public static class ItemCostCalculationMethodData
	{
		public const int ActualAverage = 1;
		public const int LastPurchasingPrice = 2;
		public const int LastCostPrice = 3;
	}

	public static class ApplicationSettingDefaultData
	{
		public const string FontSize = "12";
		public const string TopMargin = "40";
		public const string RightMargin = "10";
		public const string BottomMargin = "40";
		public const string LeftMargin = "10";
	}
}
