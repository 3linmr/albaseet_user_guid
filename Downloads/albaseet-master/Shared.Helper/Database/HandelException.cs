
using Microsoft.Extensions.Localization;
using Shared.Helper.Models.Dtos;
using System.Xml.Linq;


namespace Shared.Helper.Database
{
    public  class HandelException
    {
        private readonly IStringLocalizer<HandelException> _localizer;

        public HandelException(IStringLocalizer<HandelException> localizer)
        {
            _localizer = localizer;
        }
        public ResponseDto Handle(Exception exception)
        {
            var exceptionName = exception.GetType().Name;
            if (exceptionName == "DbUpdateException")
            {
                if ((bool)(exception.InnerException?.Message.Contains("Cannot delete or update a parent row: a foreign key constraint fails")))
                {
                    return new ResponseDto(){Success = false,Message = _localizer["DeleteExceptionFKViolation"] };
                }
            }
            return new ResponseDto(){Success = false,Message = exception.Message + "\n " + exception.InnerException?.Message + "\n " + exception.InnerException?.InnerException?.Message };
        }
    }
}
