using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;

namespace Shared.CoreOne.Contracts.Admin
{
    public interface IHomeService
    {
        Task<HomeSettingDto> GetHomeSetting();
    }
}
