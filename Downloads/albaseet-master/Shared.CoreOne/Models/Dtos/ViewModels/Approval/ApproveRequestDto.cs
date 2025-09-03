using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Approval
{
	public class ApproveRequestDto
	{
		public int RequestId { get; set; }
		public int RequestCode { get; set; }

		public short MenuCode { get; set; }
		public string? MenuName { get; set; }
		public DateTime RequestDate { get; set; }
		public string? MenuUrl { get; set; }

		public byte ApproveRequestTypeId { get; set; }

		public string? ApproveRequestTypeName { get; set; }

		public int ReferenceId { get; set; }//BaseIdFromScreen

		public string? ReferenceCode { get; set; }//CodeBasedOnIdFromScreen

		public string? FromUserName { get; set; }

		public bool? IsApproved { get; set; }

		public int ApproveId { get; set; }
		public string? ApproveName { get; set; }

		public int CurrentStepId { get; set; }

		public string? CurrentStepName { get; set; }

		public int CurrentStatusId { get; set; }
		public string? CurrentStatusName { get; set; }

		public int LastStepId { get; set; }
		public string? LastStepName { get; set; }

		public int LastStatusId { get; set; }
		public string? LastStatusName{ get; set; }

		public byte CurrentStepCount { get; set; }

		public string? UsersApprove { get; set; }
		public bool UserCanEdit { get; set; }
		public bool UserCanDelete { get; set; }

        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int StoreId { get; set; }
    }

	public class ApproveRequestDetailDto
	{
		public int ApproveRequestDetailId { get; set; }
		public int ApproveRequestId { get; set; }
		public string? OldValue { get; set; }
		public string? NewValue { get; set; }
		public string? Changes { get; set; }
	}

	public class HandleApproveRequestDto
	{
		public bool Approved { get; set; }
		public int RequestId { get; set; }
		public string? Remarks { get; set; }
	}
	public class HandleApproveRequestsDto
	{
		public bool Approved { get; set; }
		public string? Remarks { get; set; }
		public List<int>? Requests { get; set; }
	}
	public class ApproveRequestUserDto
	{
		public int ApproveRequestUserId { get; set; }
		public int RequestId { get; set; }
		public int StepId { get; set; }
		public int StatusId { get; set; }
		public string? StepName { get; set; }
		public string? StatusName { get; set; }
		public string? UserName { get; set; }
		public string? Remarks { get; set; }
		public string EntryDate { get; set; }
	}

	public class ApproveRequestTypeDto
	{
		public byte ApproveRequestTypeId { get; set; }
		public string? ApproveRequestTypeNameAr { get; set; }
		public string? ApproveRequestTypeNameEn { get; set; }
	}

	public class NewApproveRequestDto
	{
		public int RequestId { get; set; }
		public short MenuCode { get; set; }
		public byte ApproveRequestTypeId { get; set; }
		public int ReferenceId { get; set; }//BaseIdFromScreen
		public string? ReferenceCode { get; set; }//CodeBasedOnIdFromScreen
		public object? OldValue { get; set; }
		public object? NewValue { get; set; }
		public object? Changes { get; set; }
	}

	public class RequestChangesDto
	{
		public string? TableName { get; set; }
		public string? ColumnName { get; set; }
		public int Key { get; set; }
		public string? OldValue { get; set; }
		public string? NewValue { get; set; }
	}
	public class RequestHistoryDto
	{
		public string? ApproveStatus { get; set; }
		public string? Remarks { get; set; }
	}
}
