using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
    public class SystemTaskDto
    {
        public int TaskId { get; set; }

        public string? TaskNameAr { get; set; }

        public string? TaskNameEn { get; set; }

        public bool IsCompleted { get; set; }
        public bool Loading { get; set; }
    }
}
