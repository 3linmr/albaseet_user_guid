using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Settings
{
	public class ReportPrintSetting : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int ReportPrintSettingId { get; set; }

		[Column(Order = 2)]
		public int CompanyId { get; set; }

		[Column(Order = 3)]
		public short MenuCode { get; set; }

		[Column(Order = 4)]
		public string? InstitutionNameAr { get; set; }

		[Column(Order = 5)]
		public string? InstitutionNameEn { get; set; }

		[Column(Order = 6)]
		public string? InstitutionOtherNameAr { get; set; }

		[Column(Order = 7)]
		public string? InstitutionOtherNameEn { get; set; }

		[Column(Order = 8)]
		public string? Address1Ar { get; set; }

		[Column(Order = 9)]
		public string? Address1En { get; set; }

		[Column(Order = 10)]
		public string? Address2Ar { get; set; }

		[Column(Order = 11)]
		public string? Address2En { get; set; }

		[Column(Order = 12)]
		public string? Address3Ar { get; set; }

		[Column(Order = 13)]
		public string? Address3En { get; set; }

		[Column(Order = 14)]
		public string? TopNotesFirstPageOnlyAr { get; set; }

		[Column(Order = 15)]
		public string? TopNotesFirstPageOnlyEn { get; set; }

		[Column(Order = 16)]
		public string? BottomNotesLastPageOnlyAr { get; set; }

		[Column(Order = 17)]
		public string? BottomNotesLastPageOnlyEn { get; set; }

		[Column(Order = 18)]
		public string? TopNotesAllPagesAr { get; set; }

		[Column(Order = 19)]
		public string? TopNotesAllPagesEn { get; set; }

		[Column(Order = 20)]
		public string? BottomNotesAllPagesAr { get; set; }

		[Column(Order = 21)]
		public string? BottomNotesAllPagesEn { get; set; }

		[Column(Order = 22)]
		public bool PrintBusinessName { get; set; }

		[Column(Order = 23)]
		public bool PrintDateTime { get; set; }

		[Column(Order = 24)]
		public bool PrintUserName { get; set; }		
		
		[Column(Order = 25)]
		public bool PrintInstitutionLogo { get; set; }

		[Column(Order = 26)]
		public int InstitutionNameFont { get; set; }

		[Column(Order = 27)]
		public int InstitutionOtherNameFont { get; set; }

		[Column(Order = 28)]
		public int Address1Font { get; set; }

		[Column(Order = 29)]
		public int Address2Font { get; set; }

		[Column(Order = 30)]
		public int Address3Font { get; set; }

		[Column(Order = 31)]
		public int TopNotesFirstPageOnlyFont { get; set; }

		[Column(Order = 32)]
		public int BottomNotesLastPageOnlyFont { get; set; }

		[Column(Order = 33)]
		public int TopNotesAllPagesFont { get; set; }

		[Column(Order = 34)]
		public int BottomNotesAllPagesFont { get; set; }

		[Column(Order = 35)]
		public int ReportNameFont { get; set; }

		[Column(Order = 36)]
		public int ReportPeriodFont { get; set; }

		[Column(Order = 37)]
		public int GridFont { get; set; }

		[Column(Order = 38)]
		public int BusinessFont { get; set; }

		[Column(Order = 39)]
		public int DetailRounding { get; set; }

		[Column(Order = 40)]
		public int SumRounding { get; set; }

		[Column(Order = 41)]
		public int PrintFormId { get; set; }

		[Column(Order = 42)]
		public int TopMargin { get; set; }

		[Column(Order = 43)]
		public int RightMargin { get; set; }

		[Column(Order = 44)]
		public int BottomMargin { get; set; }

		[Column(Order = 45)]
		public int LeftMargin { get; set; }


		[ForeignKey(nameof(CompanyId))]
		public Company? Company { get; set; }

		[ForeignKey(nameof(MenuCode))]
		public Menu? Menu { get; set; }
		
		[ForeignKey(nameof(PrintFormId))]
		public ReportPrintForm? PrintForm { get; set; }
	}
}
