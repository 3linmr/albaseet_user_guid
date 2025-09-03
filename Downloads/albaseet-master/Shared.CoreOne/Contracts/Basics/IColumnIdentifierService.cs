using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;

namespace Shared.CoreOne.Contracts.Basics
{
	public interface IColumnIdentifierService : IBaseService<ColumnIdentifier>
	{
		IQueryable<ColumnIdentifierDto> GetColumnIdentifiers();
		IQueryable<ColumnIdentifierDropDownDto> GetColumnIdentifiersDropDown();
	}
}
