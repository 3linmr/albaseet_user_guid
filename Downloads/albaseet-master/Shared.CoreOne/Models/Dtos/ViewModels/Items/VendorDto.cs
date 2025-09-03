using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
    public class VendorDto
    {
        public int VendorId { get; set; }
        public int VendorCode { get; set; }
        public string? VendorNameAr { get; set; }
        public string? VendorNameEn { get; set; }
        public int CompanyId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string? UserNameCreated { get; set; }

        public string? UserNameModified { get; set; }
    }
    public class VendorDropDownDto
    {
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
    }
}
