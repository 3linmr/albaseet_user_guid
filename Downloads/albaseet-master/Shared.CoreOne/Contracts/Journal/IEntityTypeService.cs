using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;

namespace Shared.CoreOne.Contracts.Journal
{
	public interface IEntityTypeService : IBaseService<EntityType>
	{
		IQueryable<EntityTypeDto> GetEntityTypes();
	}
}