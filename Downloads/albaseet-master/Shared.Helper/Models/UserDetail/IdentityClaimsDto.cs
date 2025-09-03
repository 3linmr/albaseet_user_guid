using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Models.UserDetail
{
    public class IdentityClaimsDto
    {
        public string? ValueType { get; set; }
        public string? Value { get; set; }
        public string? Issuer { get; set; }
    }
}
