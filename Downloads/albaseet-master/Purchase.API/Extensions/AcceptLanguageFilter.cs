using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Purchases.API.Extensions
{
    public class AcceptLanguageFilter: IOperationFilter
    {
        public AcceptLanguageFilter()
        {

        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                Description = "Accept-Language",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema{ Type = "string" },
                Required = false
            });
        }
    }
}
