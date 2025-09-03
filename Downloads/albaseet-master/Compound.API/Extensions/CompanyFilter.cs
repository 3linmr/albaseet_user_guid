using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Compound.API.Extensions
{
    public class CompanyFilter : IOperationFilter
    {
        public CompanyFilter()
        {

        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "companyId",
                Description = "Company Id",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema{ Type = "number" },
                Required = true
            });

            operation.Parameters.Add(new OpenApiParameter
            {
	            Name = "branchId",
	            Description = "Branch Id",
	            In = ParameterLocation.Header,
	            Schema = new OpenApiSchema { Type = "number" },
	            Required = true
            });

            operation.Parameters.Add(new OpenApiParameter
            {
	            Name = "storeId",
	            Description = "Store Id",
	            In = ParameterLocation.Header,
	            Schema = new OpenApiSchema { Type = "number" },
	            Required = true
            });
		}
    }
}
