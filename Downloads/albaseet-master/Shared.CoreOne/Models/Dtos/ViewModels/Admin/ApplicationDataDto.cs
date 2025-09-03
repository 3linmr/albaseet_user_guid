using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Admin
{
	public class ApplicationDataDto
	{

	}

	public class ApplicationValidationDataDto
	{
		public List<ApplicationBusinessDto>? ApplicationBusinesses { get; set; }
		public List<ApplicationBranchDto>? ApplicationBranches { get; set; }
		public List<ApplicationStoreDto>? ApplicationStores { get; set; }
		public List<ApplicationApproveStepDto>? ApplicationApproveSteps { get; set; }
	}

	public class ApplicationBusinessDto
	{
		public short ApplicationId { get; set; }
		public int BusinessId { get; set; }
		public string? BusinessName { get; set; }
	}

	public class ApplicationBranchDto
	{
		public short ApplicationId { get; set; }
		public int BranchId { get; set; }
		public string? BranchName { get; set; }
	}

	public class ApplicationStoreDto
	{
		public short ApplicationId { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
	}

	public class ApplicationApproveStepDto
	{
		public short ApplicationId { get; set; }
		public int BusinessId { get; set; }
		public int ApproveStepId { get; set; }
		public string? ApproveStepName { get; set; }
	}
}
