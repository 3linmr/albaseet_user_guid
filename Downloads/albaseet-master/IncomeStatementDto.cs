public class IncomeStatementDto
{
    public decimal Revenue { get; set; }
    public decimal CostOfGoodsSold { get; set; }
    public decimal GrossProfit => Revenue - CostOfGoodsSold;
    public decimal OperatingExpenses { get; set; }
    public decimal OtherIncome { get; set; }
    public decimal OtherExpenses { get; set; }
    public decimal NetIncome => GrossProfit + OtherIncome - OperatingExpenses - OtherExpenses;
}
