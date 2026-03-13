using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;
using la_paceña_v2.Models;

namespace la_paceña_v2.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public ClienteController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.Clientes.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("RazonSocial,NitCarnet,Telefono,Email")] Cliente cliente)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(cliente.RazonSocial))
            {
                TempData["Error"] = "La razón social es obligatoria";
                return View(cliente);
            }

            if (!string.IsNullOrWhiteSpace(cliente.NitCarnet))
            {
                var clienteExistente = await _contexto.Clientes
                    .FirstOrDefaultAsync(c => c.NitCarnet == cliente.NitCarnet);
                if (clienteExistente != null)
                {
                    TempData["Error"] = $"Ya existe un cliente con el NIT/Carnet '{cliente.NitCarnet}'";
                    return View(cliente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cliente.Email))
            {
                if (!cliente.Email.Contains("@"))
                {
                    TempData["Error"] = "El formato del email no es válido";
                    return View(cliente);
                }
            }
            
            _contexto.Add(cliente);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Cliente creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var cliente = await _contexto.Clientes.FindAsync(id);
            return cliente == null ? NotFound() : View(cliente);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdCliente,RazonSocial,NitCarnet,Telefono,Email")] Cliente cliente)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != cliente.IdCliente) return NotFound();

            if (string.IsNullOrWhiteSpace(cliente.RazonSocial))
            {
                TempData["Error"] = "La razón social es obligatoria";
                return View(cliente);
            }

            if (!string.IsNullOrWhiteSpace(cliente.NitCarnet))
            {
                var clienteExistente = await _contexto.Clientes
                    .FirstOrDefaultAsync(c => c.NitCarnet == cliente.NitCarnet && c.IdCliente != cliente.IdCliente);
                if (clienteExistente != null)
                {
                    TempData["Error"] = $"Ya existe otro cliente con el NIT/Carnet '{cliente.NitCarnet}'";
                    return View(cliente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cliente.Email) && !cliente.Email.Contains("@"))
            {
                TempData["Error"] = "El formato del email no es válido";
                return View(cliente);
            }
            
            _contexto.Update(cliente);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Cliente actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var cliente = await _contexto.Clientes.FirstOrDefaultAsync(m => m.IdCliente == id);
            return cliente == null ? NotFound() : View(cliente);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var cliente = await _contexto.Clientes.FindAsync(id);
            if (cliente != null)
            {

                var tieneVentas = await _contexto.Ventas.AnyAsync(v => v.IdCliente == id);

                if (tieneVentas)
                {
                    TempData["Error"] = "No se puede eliminar el cliente porque tiene ventas registradas";
                    return RedirectToAction(nameof(Listar));
                }
                
                try
                {
                    _contexto.Clientes.Remove(cliente);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Cliente eliminado";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este cliente porque tiene ventas u otros registros asociados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
