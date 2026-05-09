using la_paceña_v2.Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using la_paceña_v2.Services;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new System.Globalization.CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddControllersWithViews();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddDbContext<ContextoRestaurante>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Autenticacion/Login";
        options.AccessDeniedPath = "/Autenticacion/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

builder.Services.AddAuthorization(options =>
{
    var permisos = new[] {
        "VerUsuarios", "CrearUsuarios", "EditarUsuarios", "EliminarUsuarios",
        "VerRoles", "CrearRoles", "EditarRoles", "EliminarRoles",
        "VerPermisos", "CrearPermisos", "EditarPermisos", "EliminarPermisos", "AsignarPermisos",
        "VerCategorias", "CrearCategorias", "EditarCategorias", "EliminarCategorias",
        "VerProductos", "CrearProductos", "EditarProductos", "EliminarProductos",
        "VerInsumos", "CrearInsumos", "EditarInsumos", "EliminarInsumos",
        "VerAbastecimientos", "CrearAbastecimientos", "EliminarAbastecimientos",
        "VerProveedores", "CrearProveedores", "EditarProveedores", "EliminarProveedores",
        "VerCompras", "CrearCompras", "EliminarCompras",
        "VerClientes", "CrearClientes", "EditarClientes", "EliminarClientes",
        "VerVentas", "CrearVentas", "EliminarVentas", "EditarVentas",
        "VerReporteVentas", "VerReporteStock", "VerKardex",
        "ImprimirVenta", "ImprimirCompra", "ImprimirAbastecimiento", "ExportarExcel", "ExportarPDF"
    };

    foreach (var permiso in permisos)
    {
        options.AddPolicy(permiso, policy => policy.RequireClaim("Permiso", permiso));
    }
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<la_paceña_v2.Services.IReporteService, la_paceña_v2.Services.ReporteService>();
builder.Services.AddScoped<IServicioMenu, ServicioMenu>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ContextoRestaurante>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Autenticacion}/{action=Login}/{id?}");

app.Run();
