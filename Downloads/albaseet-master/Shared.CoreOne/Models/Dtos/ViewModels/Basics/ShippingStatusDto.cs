using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
    public class ShippingStatusDto
    {
        public int ShippingStatusId { get; set; }
        public int ShippingStatusCode { get; set; }
        public string? ShippingStatusNameAr { get; set; }
        public string? ShippingStatusNameEn { get; set; }
        public int CompanyId { get; set; }
        public short MenuCode { get; set; }
        public string? MenuName { get; set; }
        public int StatusOrder { get; set; }
        public bool IsActive { get; set; }
        public string? InActiveReasons { get; set; }
    }

    public class ShippingStatusDropDownDto
    {
        public int ShippingStatusId { get; set; }
        public string? ShippingStatusName { get; set; }
    }
}
