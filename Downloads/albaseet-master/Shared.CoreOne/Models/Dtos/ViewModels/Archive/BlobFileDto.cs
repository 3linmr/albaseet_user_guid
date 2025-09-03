using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Archive
{
    public class BlobFileDto
    {
	    public int Id { get; set; }
	    public string? FileName { get; set; }
	    public byte[]? Content { get; set; }
	    public string? ContentType { get; set; }
	    public DateTime CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
	}
}
