using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Archive;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Settings
{

	public class SaveApplicationSettingDto
	{
		public int? CompanyId { get; set; }
		public int ApplicationFlagDetailId { get; set; }
		public byte ApplicationFlagTypeId { get; set; }
		public string? FlagName { get; set; }
		public string? FlagValue { get; set; }
		public IFormFile? File { get; set; }
	}

	public class ApplicationFlagTabDto
	{
		public int ApplicationFlagTabId { get; set; }
		public string? ApplicationFlagTabName { get; set; }
	}

	public class SettingTabCardDto
	{
		public int ApplicationFlagHeaderId { get; set; }
		public string? ApplicationFlagHeaderName { get; set; }
		public short Order { get; set; }
		public List<SettingTabCardDetailDto>? CardList { get; set; }

	}
	public class SettingTabCardDetailDto
	{
		public int ApplicationFlagHeaderId { get; set; }
		public int ApplicationFlagDetailId { get; set; }
		public byte ApplicationFlagTypeId { get; set; }
		public string? FlagName { get; set; }
		public string? FlagValue { get; set; }
		public short Order { get; set; }

		public List<SettingTabCardDetailSelectDto> SelectDetail { get; set; } = new List<SettingTabCardDetailSelectDto>();
	}
	public class SettingTabCardDetailSelectDto
	{
		public int ApplicationFlagDetailSelectId { get; set; }
		public int ApplicationFlagHeaderId { get; set; }
		public int ApplicationFlagDetailId { get; set; }
		public short SelectId { get; set; }
		public string? SelectName { get; set; }
		public short Order { get; set; }
	}

	public class ApplicationSettingVm
	{
		public int ApplicationFlagDetailId { get; set; }
		public int ApplicationFlagHeaderId { get; set; }
		public string? ApplicationFlagHeaderName { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public short HeaderOrder { get; set; }
		public byte ApplicationFlagTypeId { get; set; }
		public string? ApplicationFlagTypeName { get; set; }
		public string? FlagName { get; set; }
		public string? FlagValue { get; set; }
		public short DetailOrder { get; set; }

	}

	public class ApplicationFlagDetailImageDto
	{
		public int ApplicationFlagDetailImageId { get; set; }

		public int ApplicationFlagDetailCompanyId { get; set; }

		public string? FileName { get; set; }

		public string? FileType { get; set; }

		public IFormFile? Image { get; set; }
		public byte[]? ImageBinary { get; set; }
		public DateTime? CreatedAt { get; set; }

		public string? UserNameCreated { get; set; }

	}

	public class ImageBase64Dto
	{
		public string? ImageData { get; set; }
		public string? ContentType { get; set; }
	}

	public class PrintSettingDto
	{
		public string? InstitutionNameAr { get; set; }
		public string? InstitutionNameEn { get; set; }
		public string? InstitutionOtherNameAr { get; set; }
		public string? InstitutionOtherNameEn { get; set; }
		public string? Address1Ar { get; set; }
		public string? Address1En { get; set; }
		public string? Address2Ar { get; set; }
		public string? Address2En { get; set; }
		public string? Address3Ar { get; set; }
		public string? Address3En { get; set; }
		public string? TopNotesFirstPageOnlyAr { get; set; }
		public string? TopNotesFirstPageOnlyEn { get; set; }
		public string? BottomNotesLastPageOnlyAr { get; set; }
		public string? BottomNotesLastPageOnlyEn { get; set; }
		public string? TopNotesAllPagesAr { get; set; }
		public string? TopNotesAllPagesEn { get; set; }
		public string? BottomNotesAllPagesAr { get; set; }
		public string? BottomNotesAllPagesEn { get; set; }
		public bool PrintBusinessName { get; set; }
		public bool PrintDateTime { get; set; }
		public bool PrintUserName { get; set; }
		public bool PrintInstitutionLogo { get; set; }
		public int InstitutionNameFont { get; set; }
		public int InstitutionOtherNameFont { get; set; }
		public int Address1Font { get; set; }
		public int Address2Font { get; set; }
		public int Address3Font { get; set; }
		public int TopNotesFirstPageOnlyFont { get; set; }
		public int BottomNotesLastPageOnlyFont { get; set; }
		public int TopNotesAllPagesFont { get; set; }
		public int BottomNotesAllPagesFont { get; set; }
		public int ReportPeriodFont { get; set; }
		public int ReportNameFont { get; set; }
		public int GridFont { get; set; }
		public int BusinessFont { get; set; }
		public int DetailRounding { get; set; }
		public int SumRounding { get; set; }
		public int PrintFormId { get; set; }
		public string? Logo { get; set; }
		public int TopMargin { get; set; }
		public int RightMargin { get; set; }
		public int BottomMargin { get; set; }
		public int LeftMargin { get; set; }
	}

	public class ReportPrintSettingDto : PrintSettingDto
	{
		public int ReportPrintSettingId { get; set; } = 0;

		public int CompanyId { get; set; } = 0;
		public string? CompanyName { get; set; } = null;

		public short MenuCode { get; set; } = 0;
		public string? MenuCodeName { get; set; } = null;
	}

	public class PrintSettingVm : ReportPrintSettingDto
	{
		public string? BusinessName { get; set; }
		public string? InstitutionName { get; set; }
		public string? InstitutionOtherName { get; set; }
		public string? Address1 { get; set; }
		public string? Address2 { get; set; }
		public string? Address3 { get; set; }
		public string? TopNotesFirstPageOnly { get; set; }
		public string? BottomNotesLastPageOnly { get; set; }
		public string? TopNotesAllPages { get; set; }
		public string? BottomNotesAllPages { get; set; }
	}

	public class ApplicationFLagValueDto()
	{
		public int ApplicationFlagDetailId { get; set; }
		public string? ApplicationFlagValue { get; set; }
	}

	public class ReportPrintFormDropDownDto
	{
		public int ReportPrintFormId { get; set; }

		public string? ReportPrintFormName { get; set; }
	}

	public class InvoiceSettingDto
	{
		public string? InvoiceName { get; set; }
		public string? StoreName { get; set; }
		public string? StoreAddress { get; set; }
		public string? VatNo { get; set; }

		public bool ShowPaymentType { get; set; }
		public bool ShowPrintTime { get; set; }
		public bool ShowDueDate { get; set; }

		public int RoundNumberInTable { get; set; } = 2;
		public int RoundNumberInFooter { get; set; } = 2;

		// Add these for the new footer requirements
		public string? FooterBoldLine1 { get; set; } // First bold centered line
		public string? FooterBoldLine2 { get; set; } // Second bold centered line
		public string? FooterBoldLine3 { get; set; } // Third bold centered line

		public string? FooterLine1 { get; set; }     // First normal footer line
		public string? FooterLine2 { get; set; }     // Second normal footer line
		public string? FooterLine3 { get; set; }     // Third normal footer line
		public string? FooterLine4 { get; set; }     // Fourth normal footer line
		public string? FooterLine5 { get; set; }     // Fifth normal footer line
		public string? FooterLine6 { get; set; }     // Sixth normal footer line
		public string? FooterLine7 { get; set; }     // Seventh normal footer line
		public string? FooterLine8 { get; set; }     // Eighth normal footer line
		public string? FooterLine9 { get; set; }     // Ninth normal footer line
		public string? FooterLine10 { get; set; }    // Tenth normal footer line

		//header/footer for print only
		public string? InstitutionName { get; set; }
		public string? InstitutionAddress { get; set; }
		public string? InstitutionContact { get; set; }
		public string? InstitutionLogo { get; set; }
		public string? ClosureFooterLine1 { get; set; }
		public string? ClosureFooterLine2 { get; set; }
	}


	//public class ApplicationFlagHeaderDto
	//{
	//	public int ApplicationFlagHeaderId { get; set; }
	//	public string? ApplicationFlagHeaderName { get; set; }
	//	public short? MenuCode { get; set; }
	//	public string? MenuName { get; set; }
	//	public short Order { get; set; }
	//}

	//public class ApplicationFlagDetailDto
	//{
	//	public int ApplicationFlagDetailId { get; set; }
	//	public int ApplicationFlagHeaderId { get; set; }
	//	public byte ApplicationFlagTypeId { get; set; }
	//	public string? ApplicationFlagTypeName { get; set; }
	//	public string? FlagName { get; set; }
	//	public string? FlagValue { get; set; }
	//	public short Order { get; set; }
	//	public List<ApplicationDetailSelectDto>? DetailSelectList { get; set; }
	//}

	//public class ApplicationDetailSelectDto
	//{
	//	public int SelectId { get; set; }
	//	public int ApplicationFlagDetailId { get; set; }
	//	public string? SelectName { get; set; }
	//	public short Order { get; set; }
	//}

}
