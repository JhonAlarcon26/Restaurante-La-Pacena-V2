using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class UnidadDeMedidaController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public UnidadDeMedidaController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAutenticacion()
        {
            return HttpContext.Session.GetInt32("IdUsuario") != null;
        }

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            return View(await _contexto.UnidadDeMedida.ToListAsync());
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            if (id == null)
                return NotFound();

            var unidad = await _contexto.UnidadDeMedida
                .FirstOrDefaultAsync(m => m.IdUnidad == id);

            if (unidad == null)
                return NotFound();

            return View(unidad);
        }

        public IActionResult Crear()
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdUnidad,Nombre,Abreviatura")] UnidadDeMedidum unidad)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(unidad.Nombre))
            {
                TempData["Error"] = "El nombre de la unidad es obligatorio";
                return View(unidad);
            }

            if (string.IsNullOrWhiteSpace(unidad.Abreviatura))
            {
                TempData["Error"] = "La abreviatura es obligatoria";
                return View(unidad);
            }

            var nombreExistente = await _contexto.UnidadDeMedida
                .FirstOrDefaultAsync(u => u.Nombre.ToLower() == unidad.Nombre.ToLower());
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe una unidad con el nombre '{unidad.Nombre}'";
                return View(unidad);
            }

            var abreviaturaExistente = await _contexto.UnidadDeMedida
                .FirstOrDefaultAsync(u => u.Abreviatura.ToLower() == unidad.Abreviatura.ToLower());
            if (abreviaturaExistente != null)
            {
                TempData["Error"] = $"Ya existe una unidad con la abreviatura '{unidad.Abreviatura}'";
                return View(unidad);
            }

            _contexto.Add(unidad);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Unidad de medida creada exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            if (id == null)
                return NotFound();

            var unidad = await _contexto.UnidadDeMedida.FindAsync(id);
            if (unidad == null)
                return NotFound();

            return View(unidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdUnidad,Nombre,Abreviatura")] UnidadDeMedidum unidad)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            if (id != unidad.IdUnidad)
                return NotFound();

            if (string.IsNullOrWhiteSpace(unidad.Nombre))
            {
                TempData["Error"] = "El nombre de la unidad es obligatorio";
                return View(unidad);
            }

            if (string.IsNullOrWhiteSpace(unidad.Abreviatura))
            {
                TempData["Error"] = "La abreviatura es obligatoria";
                return View(unidad);
            }

            var nombreExistente = await _contexto.UnidadDeMedida
                .FirstOrDefaultAsync(u => u.Nombre.ToLower() == unidad.Nombre.ToLower() && u.IdUnidad != unidad.IdUnidad);
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe otra unidad con el nombre '{unidad.Nombre}'";
                return View(unidad);
            }

            var abreviaturaExistente = await _contexto.UnidadDeMedida
                .FirstOrDefaultAsync(u => u.Abreviatura.ToLower() == unidad.Abreviatura.ToLower() && u.IdUnidad != unidad.IdUnidad);
            if (abreviaturaExistente != null)
            {
                TempData["Error"] = $"Ya existe otra unidad con la abreviatura '{unidad.Abreviatura}'";
                return View(unidad);
            }

            _contexto.Update(unidad);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Unidad de medida actualizada exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            if (id == null)
                return NotFound();

            var unidad = await _contexto.UnidadDeMedida
                .FirstOrDefaultAsync(m => m.IdUnidad == id);

            if (unidad == null)
                return NotFound();

            return View(unidad);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAutenticacion())
                return RedirectToAction("Login", "Autenticacion");

            var unidad = await _contexto.UnidadDeMedida.FindAsync(id);
            if (unidad != null)
            {

                var tieneInsumos = await _contexto.Insumos.AnyAsync(i => i.IdUnidad == id);
                if (tieneInsumos)
                {
                    TempData["Error"] = "No se puede eliminar la unidad de medida porque tiene insumos asignados";
                    return RedirectToAction(nameof(Listar));
                }

                try
                {
                    _contexto.UnidadDeMedida.Remove(unidad);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Unidad de medida eliminada exitosamente";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar esta unidad porque hay insumos que la utilizan.";
                }
            }

            return RedirectToAction(nameof(Listar));
        }

        private bool UnidadExiste(int id)
        {
            return _contexto.UnidadDeMedida.Any(e => e.IdUnidad == id);
        }
    }
}
