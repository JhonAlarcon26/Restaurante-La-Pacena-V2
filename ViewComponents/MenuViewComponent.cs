using la_paceña_v2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace la_paceña_v2.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IServicioMenu _servicioMenu;

        public MenuViewComponent(IServicioMenu servicioMenu)
        {
            _servicioMenu = servicioMenu;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menu = await _servicioMenu.ObtenerMenuPermitidoAsync((ClaimsPrincipal)User);
            return View(menu);
        }
    }
}
