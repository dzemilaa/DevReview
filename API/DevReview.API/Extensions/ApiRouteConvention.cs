using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DevReview.API.Extensions
{
    /// <summary>
    /// Prefixes all controller routes with api/v1 (replaces api/ when present).
    /// </summary>
    public sealed class ApiRouteConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var selector in controller.Selectors)
                {
                    if (string.IsNullOrEmpty(selector.AttributeRouteModel?.Template))
                    {
                        continue;
                    }

                    var template = selector.AttributeRouteModel.Template;
                    if (template.StartsWith("api/", StringComparison.OrdinalIgnoreCase) &&
                        !template.StartsWith("api/v", StringComparison.OrdinalIgnoreCase))
                    {
                        selector.AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = "api/v1/" + template["api/".Length..]
                        };
                    }
                }
            }
        }
    }
}
