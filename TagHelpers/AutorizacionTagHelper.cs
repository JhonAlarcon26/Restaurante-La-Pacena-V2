using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace la_paceña_v2.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-politica")]
    public class AutorizacionTagHelper : TagHelper
    {
        private readonly IAuthorizationService _servicioAutorizacion;

        public AutorizacionTagHelper(IAuthorizationService servicioAutorizacion)
        {
            _servicioAutorizacion = servicioAutorizacion;
        }

        [HtmlAttributeName("asp-politica")]
        public string Politica { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var resultado = await _servicioAutorizacion.AuthorizeAsync(ViewContext.HttpContext.User, Politica);

            if (!resultado.Succeeded)
            {
                output.SuppressOutput();
            }
        }
    }
}
