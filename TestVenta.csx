using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;
using la_paceña_v2.Models;

var optionsBuilder = new DbContextOptionsBuilder<ContextoRestaurante>();
optionsBuilder.UseSqlServer("Server=localhost;Database=BD_la_pacena;User Id=sa;Password=75629487;TrustServerCertificate=True;");
var _contexto = new ContextoRestaurante(optionsBuilder.Options);

var locro = _contexto.Platos.FirstOrDefault(p => p.Nombre.Contains("Locro"));
if(locro == null) {
    Console.WriteLine("Locro no encontrado");
    return;
}

Console.WriteLine("Stocks ANTES:");
var insumosReceta = _contexto.Receta.Where(r => r.IdPlato == locro.IdPlato).Select(r => r.IdInsumo).ToList();
var stocks = _contexto.Stocks.Where(s => insumosReceta.Contains(s.IdInsumo.Value)).Include(s => s.IdInsumoNavigation).ToList();
foreach(var s in stocks) {
    Console.WriteLine($"{s.IdInsumoNavigation.Nombre}: {s.CantidadActual}");
}

// Simulamos la lógica de VentaController
using var transaction = _contexto.Database.BeginTransaction();
var listaReceta = _contexto.Receta.Where(r => r.IdPlato == locro.IdPlato).ToList();
foreach (var ingrediente in listaReceta)
{
    var stockInsumo = _contexto.Stocks.FirstOrDefault(s => s.IdInsumo == ingrediente.IdInsumo);
    if (stockInsumo != null)
    {
        decimal cantidadADescontar = ingrediente.Cantidad * 1;
        stockInsumo.CantidadActual -= cantidadADescontar;
        stockInsumo.UltimaActualizacion = DateTime.Now;
        _contexto.Stocks.Update(stockInsumo);
        Console.WriteLine($"Descontando {cantidadADescontar} a {stockInsumo.IdInsumoNavigation.Nombre}");
    }
}
_contexto.SaveChanges();
transaction.Commit();

Console.WriteLine("Stocks DESPUÉS:");
stocks = _contexto.Stocks.Where(s => insumosReceta.Contains(s.IdInsumo.Value)).Include(s => s.IdInsumoNavigation).ToList();
foreach(var s in stocks) {
    Console.WriteLine($"{s.IdInsumoNavigation.Nombre}: {s.CantidadActual}");
}
