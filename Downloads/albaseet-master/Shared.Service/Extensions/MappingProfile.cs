using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;

namespace Shared.Service.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApprovalDto, ApproveRequestDto>();
            CreateMap<ApproveRequestDto, ApprovalDto>();

            CreateMap<PrintSettingDto, PrintSettingVm>();
            CreateMap<PrintSettingVm, PrintSettingDto>();
		}
    }
}
