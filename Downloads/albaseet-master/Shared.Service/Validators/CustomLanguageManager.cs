using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Validators
{
    public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
    {
        public CustomLanguageManager()
        {
            AddTranslation("en", "NotNullValidator", "'{PropertyName}' is required.");
            AddTranslation("ar", "NotNullValidator", "'{PropertyName}' مطلوب إدخال");
        }
    }
}
