using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;

namespace Shared.CoreOne.Models.Dtos.ViewModels.CostCenters
{
	public class CostCenterDto
	{
		public int CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterNameAr { get; set; }
		public string? CostCenterNameEn { get; set; }
		public string? Description { get; set; }
		public int CompanyId { get; set; }
		public bool IsMainCostCenter { get; set; }
		public int? MainCostCenterId { get; set; }
		public string? MainCostCenterCode { get; set; }
		public string? MainCostCenterName { get; set; }
		public byte CostCenterLevel { get; set; }
		public int Order { get; set; }
		public bool IsLastLevel { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsActive { get; set; }
		public string? InActiveReasons { get; set; }
		public bool HasRemarks { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public bool IsNonEditable { get; set; }
		public string? NotesAr { get; set; }
		public string? NotesEn { get; set; }

		public bool HasChildren { get; set; }

		public List<MenuNoteDto>? MenuNotes { get; set; }
	}

	public class CostCenterDropDownDto
	{
		public int CostCenterId { get; set; }
		public short CurrencyId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterName { get; set; }
	}
	public class CostCenterTreeDto
	{
		public int CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterNameAr { get; set; }
		public string? CostCenterNameEn { get; set; }
		public bool IsMainCostCenter { get; set; }
		public int MainCostCenterId { get; set; }
		public string? MainCostCenterCode { get; set; }
		public byte CostCenterLevel { get; set; }
		public int Order { get; set; }
		public bool IsLastLevel { get; set; }
	}

	public class CostCenterSimpleTreeDto
	{
		public int CostCenterId { get; set; }
		public int? MainCostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public byte CostCenterLevel { get; set; }
		public string? CostCenterName { get; set; }
		public List<CostCenterSimpleTreeDto> List { get; set; } = new List<CostCenterSimpleTreeDto>();
	}

	public class CostCenterAutoCompleteDto
	{
		public int CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterName { get; set; }
		public byte CostCenterLevel { get; set; }
	}

	public class CostCenterTreeVm
	{
		public int CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterNameAr { get; set; }
		public string? CostCenterNameEn { get; set; }
		public bool IsMainCostCenter { get; set; }
		public int? MainCostCenterId { get; set; }
		public byte CostCenterLevel { get; set; }
		public bool IsLastLevel { get; set; }

		public List<CostCenterTreeVm>? Children { get; set; }
	}

	public class CostCenterOriginalTreeDto
	{
		public CostCenter? CostCenter { get; set; }
		public List<CostCenter>? Children { get; set; }
	}
}
