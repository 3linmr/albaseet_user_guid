namespace Shared.CoreOne.Models.Dtos.ViewModels;

public interface IHasQuantities
{
	public int ItemId { get; set; }
	public string? ItemName { get; set; }
	public int ItemPackageId { get; set; }
	public DateTime? ExpireDate { get; set; }
	public string? BatchNumber { get; set; }
	public decimal Quantity { get; set; }
	public decimal BonusQuantity { get; set; }
}

public interface IHasQuantitiesOnly
{
	public int ItemId { get; set; }
	public decimal Quantity { get; set; }
}