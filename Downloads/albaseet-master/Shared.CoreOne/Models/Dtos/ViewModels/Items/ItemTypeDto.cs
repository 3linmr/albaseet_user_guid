using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
    public class ItemTypeDto
    {
        public byte ItemTypeId { get; set; }
        public string? ItemTypeNameAr { get; set; }
        public string? ItemTypeNameEn { get; set; }
    }

    public class ItemTypeDropDownDto
    {
        public byte ItemTypeId { get; set; }
        public string? ItemTypeName { get; set; }
    }
}
