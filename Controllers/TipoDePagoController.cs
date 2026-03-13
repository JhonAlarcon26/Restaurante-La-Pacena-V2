using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;
using la_paceña_v2.Models;

namespace la_paceña_v2.Controllers
{
    public class TipoDePagoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public TipoDePagoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.TipoDePagos.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,EsBancarizado,Estado")] TipoDePago tipo)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(tipo.Nombre))
            {
                TempData["Error"] = "El nombre del tipo de pago es obligatorio";
                return View(tipo);
            }

            var tipoExistente = await _contexto.TipoDePagos
                .FirstOrDefaultAsync(t => t.Nombre.ToLower() == tipo.Nombre.ToLower());
            if (tipoExistente != null)
            {
                TempData["Error"] = $"Ya existe un tipo de pago con el nombre '{tipo.Nombre}'";
                return View(tipo);
            }
            
            _contexto.Add(tipo);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Tipo de pago creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var tipo = await _contexto.TipoDePagos.FindAsync(id);
            return tipo == null ? NotFound() : View(tipo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdTipoPago,Nombre,EsBancarizado,Estado")] TipoDePago tipo)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != tipo.IdTipoPago) return NotFound();

            if (string.IsNullOrWhiteSpace(tipo.Nombre))
            {
                TempData["Error"] = "El nombre del tipo de pago es obligatorio";
                return View(tipo);
            }

            var tipoExistente = await _contexto.TipoDePagos
                .FirstOrDefaultAsync(t => t.Nombre.ToLower() == tipo.Nombre.ToLower() && t.IdTipoPago != tipo.IdTipoPago);
            if (tipoExistente != null)
            {
                TempData["Error"] = $"Ya existe otro tipo de pago con el nombre '{tipo.Nombre}'";
                return View(tipo);
            }
            
            _contexto.Update(tipo);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Tipo de pago actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var tipo = await _contexto.TipoDePagos.FirstOrDefaultAsync(m => m.IdTipoPago == id);
            return tipo == null ? NotFound() : View(tipo);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var tipo = await _contexto.TipoDePagos.FindAsync(id);
            if (tipo != null)
            {

                var tiposProtegidos = new[] { "Efectivo", "Tarjeta", "QR" };
                if (tiposProtegidos.Contains(tipo.Nombre, StringComparer.OrdinalIgnoreCase))
                {
                    TempData["Error"] = $"No se puede eliminar el tipo de pago '{tipo.Nombre}' porque es un tipo del sistema";
                    return RedirectToAction(nameof(Listar));
                }

                var tieneVentas = await _contexto.Ventas.AnyAsync(v => v.IdTipoPago == id);
                if (tieneVentas)
                {
                    TempData["Error"] = "No se puede eliminar el tipo de pago porque tiene ventas registradas. Puede desactivarlo en su lugar.";
                    return RedirectToAction(nameof(Listar));
                }
                
                try
                {
                    _contexto.TipoDePagos.Remove(tipo);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Tipo de pago eliminado";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este tipo de pago porque está en uso.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
