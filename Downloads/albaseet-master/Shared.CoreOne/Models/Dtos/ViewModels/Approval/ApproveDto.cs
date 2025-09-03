using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Approval
{
	public class ApproveDto
	{
		public int ApproveId { get; set; }

		public int ApproveCode { get; set; }

		public int ApplicationId { get; set; }

		public short MenuCode { get; set; }

		public int CompanyId { get; set; }

		public string? MenuName { get; set; }

		public string? ApproveName { get; set; }
		public string? ApproveNameAr { get; set; }

		public string? ApproveNameEn { get; set; }

		public bool OnAdd { get; set; }

		public bool OnEdit { get; set; }

		public bool OnDelete { get; set; }

		public bool IsStopped { get; set; }

		public string? IsStoppedName { get; set; }
		public string? OnAddName { get; set; }

		public string? OnEditName { get; set; }

		public string? OnDeleteName { get; set; }
	}

	public class ApproveStepDto
	{
		public int ApproveStepId { get; set; }
		public int ApproveId { get; set; }

		public string? ApproveName { get; set; }

		public string? StepName { get; set; }
		public string? StepNameAr { get; set; }

		public string? StepNameEn { get; set; }

		public byte ApproveOrder { get; set; }

		public byte ApproveCount { get; set; }

		public bool AllCountShouldApprove { get; set; }

		public bool IsDeleted { get; set; }

		public bool IsLastStep { get; set; }
	}

	public class ApproveDefinitionDto
	{
		public ApproveDto? Approve { get; set; }
		public List<ApproveStepDto>? ApproveSteps { get; set; }
	}

	public class MenuApproveTypeDto
	{
		public short MenuCode { get; set; }
		public bool HasApprove { get; set; }
		public bool OnAdd { get; set; }
		public bool OnEdit { get; set; }
		public bool OnDelete { get; set; }
	}

	public class ApproveStatusDto
	{
		public int StepId { get; set; }
		public string? StepName { get; set; }
		public int StatusId { get; set; }
		public string? StatusName { get; set; }
	}

	public class ApproveTreeDto
	{
		public int Id { get; set; }
		public string? Text { get; set; }
		public int? MainId { get; set; }
		public bool? IsMain { get; set; }
		public int Order { get; set; }

		public bool Selected { get; set; }
		public bool IsStep { get; set; } // Other Than Is Approve Def
	}
}
