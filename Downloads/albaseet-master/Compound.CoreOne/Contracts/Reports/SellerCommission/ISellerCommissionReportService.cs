using Compound.CoreOne.Models.Dtos.Reports.SellerCommissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.SellerCommission
{
	public interface ISellerCommissionReportService
	{
		IQueryable<SellerCommissionReportDto> GetSellerCommissionReport(int companyId, DateTime? fromDate, DateTime? toDate);
	}
}
