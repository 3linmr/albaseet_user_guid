using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Domain.Approval;

namespace Shared.Service.Services.Approval
{
	public class ApproveRequestTypeService : BaseService<ApproveRequestType>, IApproveRequestTypeService
	{
		public ApproveRequestTypeService(IRepository<ApproveRequestType> repository) : base(repository)
		{

		}
	}
}
