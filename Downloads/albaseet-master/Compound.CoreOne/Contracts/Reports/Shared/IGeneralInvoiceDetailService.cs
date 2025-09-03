using System.Linq;
using Compound.CoreOne.Models.Dtos.Reports.Shared;

namespace Compound.CoreOne.Contracts.Reports.Shared
{
    public interface IGeneralInvoiceDetailService
    {
        IQueryable<GeneralInvoiceDetailDto> GetGeneralInvoiceDetails();
    }
}
