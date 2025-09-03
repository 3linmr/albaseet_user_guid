using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Services.Localization
{
    public class SharedResource : ISharedResource
    {
        private readonly IStringLocalizer _localizer;

        public SharedResource(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string index]
        {
            get
            {
                return _localizer[index];
            }
        }
    }
}
