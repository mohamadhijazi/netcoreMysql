using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.core.Samples.Web
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    public class SchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var key in context.SchemaRepository.Schemas.Keys)
            {
                if (key.Contains("Microsoft") || key.Contains("System"))
                    context.SchemaRepository.Schemas.Remove(key);
            }
        }
    }
}
