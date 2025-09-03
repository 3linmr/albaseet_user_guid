namespace Compound.CoreOne.Contracts.Reports.Inventory;

public interface IItemReportDataService
{
    Task<Dictionary<int, string>> GetItemMenuNotes(List<int> companyIds);
    Task<Dictionary<int, string>> GetItemMenuNotesByStoreIds(List<int> storeIds);
    Task<string> GetItemMenuNotesByItemId(int itemId);
    Task<Dictionary<int, string>> GetItemAttributes(List<int> companyIds);
    Task<Dictionary<int, string>> GetItemAttributesByStoreIds(List<int> storeIds);
    Task<string> GetItemAttributesByItemId(int itemId);
    Task<Dictionary<int, string>> GetItemOtherTaxes(List<int> companyIds);
    Task<Dictionary<int, string>> GetItemOtherTaxesByStoreId(List<int> storeIds);
    Task<string> GetItemOtherTaxesByItemId(int itemId);
}
