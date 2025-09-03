public interface IIncomeStatementService
{
    IQueryable<IncomeStatementDto> GetIncomeStatement(int storeId, DateTime? fromDate, DateTime? toDate);
}
