using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Shared
{
    public class ApprovalDto
    {
	    public int ApproveId { get; set; }
        public bool? IsApproved { get; set; }

        public int CurrentStepId { get; set; }
        public string? CurrentStepName { get; set; }

        public int CurrentStatusId { get; set; }

        public int LastStepId { get; set; }

        public int LastStatusId { get; set; }

        public byte CurrentStepCount { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }

        public int CurrentUserStatusId { get; set; }
    }

    public class NextStepStatusDto
    {
        public int Step { get; set; }
        public int Status { get; set; }
        public int LastStep { get; set; }
        public int LastStatus { get; set; }
        public int StepCount { get; set; }


        public string? Message { get; set; }
        public bool Success { get; set; }
    }

    public class LastStepStatus
    {
        public int StepId { get; set; }
        public int StatusId { get; set; }
    }
    public class FirstStepStatus
    {
        public bool? IsApproved { get; set; }

        public int? CurrentStepId { get; set; }

        public int? CurrentStatusId { get; set; }

        public int? LastStepId { get; set; }

        public int? LastStatusId { get; set; }

        public byte? CurrentStepCounter { get; set; }

        public string? UsersApprove { get; set; }
    }

    public class StepStatusDto
    {
        public int StepId { get; set; }
        public int StatusId { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLastStep { get; set; }
    }

    public class UserStepDto
    {
        public int StepId { get; set; }
        public int StatusId { get; set; }
        public string? UserName { get; set; }
    }

    public class UserStepVm
    {
	    public int StepId { get; set; }
	    public string? StepName { get; set; }
	    public int StatusId { get; set; }
	    public string? StatusName { get; set; }
	    public string? UserName { get; set; }
    }
}
