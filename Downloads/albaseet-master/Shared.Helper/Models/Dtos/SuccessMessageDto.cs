using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Models.Dtos
{
    public class ResponseDto
    {
        public int Id { get; set; }
        public List<int> IdList { get; set; } = new List<int>();
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ResponseType { get; set; } = ResponseTypeData.Message;
    }

    public class DocumentCodeDto
    {
	    public int NextCode { get; set; }
	    public string? Prefix { get; set; }
	    public string? Suffix { get; set; }
    }
    public class ApproveResponseDto
    {
	    public int RequestId { get; set; }
	    public bool? Approved { get; set; }
	    public bool Success { get; set; }
	    public string? Message { get; set; }
	    public short MenuCode { get; set; }
	    public byte ApproveRequestTypeId { get; set; }
	    public object? UserRequest { get; set; }
    }
	public class TaskResultDto
    {
        public int BaseId { get; set; }
        public string? Title { get; set; }
        public bool Success { get; set; }
        public bool Done { get; set; }
        public string? Message { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Url { get; set; }
    }

    public static class ResponseTypeData
    {
	    public const string? Message = "Message";
	    public const string? Alert = "Alert";
	    public const string? Confirm = "Confirm";
    }
}
