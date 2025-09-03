using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
    public class HomeSettingDto
    {
        public List<CompanyDropDownDto> Companies { get; set; } = new List<CompanyDropDownDto>();
        public List<BranchDropDownDto> Branches { get; set; } = new List<BranchDropDownDto>();
        public List<StoreDropDownDto> Stores { get; set; } = new List<StoreDropDownDto>();
        public bool HasCompanies { get; set; }
    }
}
