using Compound.CoreOne.Models.Dtos.Reports.Accounting;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    public interface IincomeStatmentservice
    {
        IQueryable<IncomeStatementDto> GetList(DateTime fromDate, DateTime toDate, int storeId);
    }
}
