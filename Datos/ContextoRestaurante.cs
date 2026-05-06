using la_paceña_v2.Models;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Datos;

public partial class ContextoRestaurante : DbContext
{
    public ContextoRestaurante() { }
    public ContextoRestaurante(DbContextOptions<ContextoRestaurante> options) : base(options) { }

    public virtual DbSet<Abastecimiento> Abastecimientos { get; set; }
    public virtual DbSet<CategoriaPlato> CategoriaPlatos { get; set; }
    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<Compra> Compras { get; set; }
    public virtual DbSet<DetalleAbastecimiento> DetalleAbastecimientos { get; set; }
    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }
    public virtual DbSet<DetalleVenta> DetalleVentas { get; set; }
    public virtual DbSet<Insumo> Insumos { get; set; }
    public virtual DbSet<KardexGeneral> KardexGenerals { get; set; }
    public virtual DbSet<Permiso> Permisos { get; set; }
    public virtual DbSet<Plato> Platos { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Proveedor> Proveedors { get; set; }
    public virtual DbSet<AbastecimientoPlato> AbastecimientoPlatos { get; set; }
    public virtual DbSet<DetalleAbastecimientoPlato> DetalleAbastecimientoPlatos { get; set; }
    public virtual DbSet<StockPlato> StockPlatos { get; set; }
    public virtual DbSet<Rol> Rols { get; set; }
    public virtual DbSet<RolPermiso> RolPermisos { get; set; }
    public virtual DbSet<Stock> Stocks { get; set; }
    public virtual DbSet<Sucursal> Sucursals { get; set; }
    public virtual DbSet<TipoDePago> TipoDePagos { get; set; }
    public virtual DbSet<TipoProducto> TipoProductos { get; set; }
    public virtual DbSet<UnidadDeMedidum> UnidadDeMedida { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Venta> Ventas { get; set; }
    public virtual DbSet<VistaAlertaStock> VistaAlertaStocks { get; set; }
    public virtual DbSet<VistaValoracionInventario> VistaValoracionInventarios { get; set; }
    public virtual DbSet<VistaHistorialCompleto> VistaHistorialCompletos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abastecimiento>(entity =>
        {
            entity.HasKey(e => e.IdAbastecimiento).HasName("PK__avasteci__53D4A869");
            entity.ToTable("compra_producto");
            entity.Property(e => e.IdAbastecimiento).HasColumnName("id_avastecimiento");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.FechaAbastecimiento).HasDefaultValueSql("(getdate())").HasColumnName("fecha_avastecimiento");
            entity.Property(e => e.NroFacturaProveedor).HasMaxLength(50).IsUnicode(false).HasColumnName("nro_referencia");
            entity.Property(e => e.TotalAbastecimiento).HasColumnType("decimal(10, 2)").HasColumnName("total_avastecimiento");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Abastecimientos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Avas_Proveedor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Abastecimientos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Avas_Usuario");
        });

        modelBuilder.Entity<CategoriaPlato>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__categori__CD54A221");
            entity.ToTable("categoria_plato");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.ImpresoraDestino).HasMaxLength(100).IsUnicode(false).HasColumnName("impresora_destino");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__cliente__677F38F5");
            entity.ToTable("cliente");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.NitCarnet).HasMaxLength(50).IsUnicode(false).HasColumnName("nit_carnet");
            entity.Property(e => e.RazonSocial).HasMaxLength(150).IsUnicode(false).HasColumnName("razon_social");
            entity.Property(e => e.Telefono).HasMaxLength(50).IsUnicode(false).HasColumnName("telefono");
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).HasColumnName("email");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__compra__C4BAA604");
            entity.ToTable("compra_insumo");
            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.FechaCompra).HasDefaultValueSql("(getdate())").HasColumnName("fecha_compra");
            entity.Property(e => e.NroFacturaProveedor).HasMaxLength(50).IsUnicode(false).HasColumnName("nro_referencia");
            entity.Property(e => e.TotalCompra).HasColumnType("decimal(10, 2)").HasColumnName("total_compra");
            entity.Property(e => e.Estado).HasMaxLength(20).IsUnicode(false).HasDefaultValue("COMPLETADA").HasColumnName("estado");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Proveedor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Usuario");
        });

        modelBuilder.Entity<DetalleAbastecimiento>(entity =>
        {
            entity.HasKey(e => e.IdDetalleAbastecimiento).HasName("PK__detalle___09384E3A");
            entity.ToTable("detalle_compra_producto");
            entity.Property(e => e.IdDetalleAbastecimiento).HasColumnName("id_detalle_avastecimiento");
            entity.Property(e => e.IdAbastecimiento).HasColumnName("id_avastecimiento");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)").HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)").HasColumnName("subtotal");

            entity.HasOne(d => d.IdAbastecimientoNavigation).WithMany(p => p.DetalleAbastecimientos)
                .HasForeignKey(d => d.IdAbastecimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleA_Avaste");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleAbastecimientos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleA_Producto");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("PK__detalle___768CE6A5");
            entity.ToTable("detalle_compra_insumo");
            entity.Property(e => e.IdDetalleCompra).HasColumnName("id_detalle_compra");
            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 3)").HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)").HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)").HasColumnName("subtotal");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleC_Compra");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdInsumo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleC_Insumo");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.IdDetalleVenta).HasName("PK__detalle___B3000AF5");
            entity.ToTable("detalle_venta");
            entity.Property(e => e.IdDetalleVenta).HasColumnName("id_detalle_venta");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)").HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)").HasColumnName("subtotal");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.IdPlato)
                .HasConstraintName("FK_DetalleV_Plato");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_DetalleV_Producto");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleV_Venta");
        });

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.IdInsumo).HasName("PK__insumo__DF9C369B");
            entity.ToTable("insumo");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad");
            entity.Property(e => e.CostoPromedio).HasColumnType("decimal(10, 2)").HasDefaultValue(0m).HasColumnName("costo_promedio");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.Insumos)
                .HasForeignKey(d => d.IdUnidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Insumo_Unidad");
        });

        modelBuilder.Entity<KardexGeneral>(entity =>
        {
            entity.HasKey(e => e.IdKardex).HasName("PK__kardex_g__463A29B1");
            entity.ToTable("kardex_general");
            entity.Property(e => e.IdKardex).HasColumnName("id_kardex");
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())").HasColumnName("fecha");
            entity.Property(e => e.TipoMovimiento).HasMaxLength(50).IsUnicode(false).HasColumnName("tipo_movimiento");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 3)").HasColumnName("cantidad");
            entity.Property(e => e.CostoUnitario).HasColumnType("decimal(18, 2)").HasColumnName("costo_unitario");
            entity.Property(e => e.IdReferencia).HasColumnName("id_referencia");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.KardexGenerals)
                .HasForeignKey(d => d.IdInsumo)
                .HasConstraintName("FK_Kardex_Insumo");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.KardexGenerals)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Kardex_Producto");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.KardexGenerals)
                .HasForeignKey(d => d.IdPlato)
                .HasConstraintName("FK_Kardex_Plato");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__permisos__2D9638E7");
            entity.ToTable("permisos");
            entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasMaxLength(250).IsUnicode(false).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.IdPlato).HasName("PK__plato__6F37C223");
            entity.ToTable("plato");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.DescripcionCarta).HasMaxLength(255).IsUnicode(false).HasColumnName("descripcion_carta");
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(10, 2)").HasColumnName("precio_venta");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Platos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Plato_Categoria");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__producto__1413F405");
            entity.ToTable("producto");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.CodigoBarras).HasMaxLength(100).IsUnicode(false).HasColumnName("codigo_barras");
            entity.Property(e => e.IdTipoProducto).HasColumnName("id_tipo_producto");
            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad");
            entity.Property(e => e.PrecioCompraActual).HasColumnType("decimal(10, 2)").HasDefaultValue(0m).HasColumnName("precio_compra_actual");
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(10, 2)").HasColumnName("precio_venta");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");

            entity.HasOne(d => d.IdTipoProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdTipoProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_TipoProducto");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdUnidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Unidad");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__proveedo__8D0CE3DF");
            entity.ToTable("proveedor");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.NitCarnet).HasMaxLength(50).IsUnicode(false).HasColumnName("nit_carnet");
            entity.Property(e => e.NombreEmpresa).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre_empresa");
            entity.Property(e => e.ContactoNombre).HasMaxLength(150).IsUnicode(false).HasColumnName("contacto_nombre");
            entity.Property(e => e.Telefono).HasMaxLength(50).IsUnicode(false).HasColumnName("telefono");
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).HasColumnName("email");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<AbastecimientoPlato>(entity =>
        {
            entity.HasKey(e => e.IdAbastecimientoPlato);
            entity.ToTable("avastecimiento_plato");
            entity.Property(e => e.IdAbastecimientoPlato).HasColumnName("id_avastecimiento_plato");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.FechaAbastecimiento).HasDefaultValueSql("(getdate())").HasColumnName("fecha_avastecimiento");
            entity.Property(e => e.NroFacturaProveedor).HasMaxLength(50).IsUnicode(false).HasColumnName("nro_referencia");
            entity.Property(e => e.TotalAbastecimiento).HasColumnType("decimal(10, 2)").HasColumnName("total_avastecimiento");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvasP_Usuario");
        });

        modelBuilder.Entity<DetalleAbastecimientoPlato>(entity =>
        {
            entity.HasKey(e => e.IdDetalleAbastecimientoPlato);
            entity.ToTable("detalle_avastecimiento_plato");
            entity.Property(e => e.IdDetalleAbastecimientoPlato).HasColumnName("id_detalle_avastecimiento_plato");
            entity.Property(e => e.IdAbastecimientoPlato).HasColumnName("id_avastecimiento_plato");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)").HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)").HasColumnName("subtotal");

            entity.HasOne(d => d.IdAbastecimientoPlatoNavigation).WithMany(p => p.DetalleAbastecimientoPlatos)
                .HasForeignKey(d => d.IdAbastecimientoPlato)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DetalleAP_Avaste");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.DetalleAbastecimientoPlatos)
                .HasForeignKey(d => d.IdPlato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleAP_Plato");
        });

        modelBuilder.Entity<StockPlato>(entity =>
        {
            entity.HasKey(e => e.IdStockPlato);
            entity.ToTable("stock_plato");
            entity.Property(e => e.IdStockPlato).HasColumnName("id_stock_plato");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.CantidadActual).HasColumnType("decimal(10, 3)").HasDefaultValue(0m).HasColumnName("cantidad_actual");
            entity.Property(e => e.CantidadMinima).HasColumnType("decimal(10, 3)").HasDefaultValue(5m).HasColumnName("cantidad_minima");
            entity.Property(e => e.UltimaActualizacion).HasDefaultValueSql("(getdate())").HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.StockPlatos)
                .HasForeignKey(d => d.IdPlato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockP_Plato");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__rol__6ABCB5E0");
            entity.ToTable("rol");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasMaxLength(255).IsUnicode(false).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<RolPermiso>(entity =>
        {
            entity.HasKey(e => new { e.IdRol, e.IdPermiso }).HasName("PK__rol_perm__D047F817");
            entity.ToTable("rol_permiso");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
            entity.Property(e => e.IdRolPermiso).HasColumnName("id_rol_permiso");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.RolPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Permiso");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolPermisos)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Rol");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock).HasName("PK__stock__288BA44D");
            entity.ToTable("stock");
            entity.Property(e => e.IdStock).HasColumnName("id_stock");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.CantidadActual).HasColumnType("decimal(10, 3)").HasDefaultValue(0m).HasColumnName("cantidad_actual");
            entity.Property(e => e.CantidadMinima).HasColumnType("decimal(10, 3)").HasDefaultValue(5m).HasColumnName("cantidad_minima");
            entity.Property(e => e.UltimaActualizacion).HasDefaultValueSql("(getdate())").HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdInsumo)
                .HasConstraintName("FK_Stock_Insumo");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Stock_Producto");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PK__sucursal__32A559C3");
            entity.ToTable("sucursal");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Direccion).HasMaxLength(250).IsUnicode(false).HasColumnName("direccion");
            entity.Property(e => e.Ciudad).HasMaxLength(100).IsUnicode(false).HasColumnName("ciudad");
            entity.Property(e => e.Telefono).HasMaxLength(50).IsUnicode(false).HasColumnName("telefono");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<TipoDePago>(entity =>
        {
            entity.HasKey(e => e.IdTipoPago).HasName("PK__tipo_de___6C2C5535");
            entity.ToTable("tipo_de_pago");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<TipoProducto>(entity =>
        {
            entity.HasKey(e => e.IdTipoProducto).HasName("PK__tipo_pro__3D5E7E2C");
            entity.ToTable("tipo_producto");
            entity.Property(e => e.IdTipoProducto).HasColumnName("id_tipo_producto");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasMaxLength(255).IsUnicode(false).HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");
        });

        modelBuilder.Entity<UnidadDeMedidum>(entity =>
        {
            entity.HasKey(e => e.IdUnidad).HasName("PK__unidad_d__2D93148F");
            entity.ToTable("unidad_de_medida");
            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad");
            entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Abreviatura).HasMaxLength(10).IsUnicode(false).HasColumnName("abreviatura");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuario__4E510534");
            entity.ToTable("usuario");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Nombre).HasMaxLength(150).IsUnicode(false).HasColumnName("nombre");
            entity.Property(e => e.Username).HasMaxLength(50).IsUnicode(false).HasColumnName("username");
            entity.Property(e => e.Password).HasMaxLength(255).IsUnicode(false).HasColumnName("password");
            entity.Property(e => e.Estado).HasDefaultValue(true).HasColumnName("estado");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Sucursal");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__ventas__4E2B9635");
            entity.ToTable("ventas");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.FechaVenta).HasDefaultValueSql("(getdate())").HasColumnName("fecha_venta");
            entity.Property(e => e.NroMesa).HasColumnName("nro_mesa");
            entity.Property(e => e.TotalVenta).HasColumnType("decimal(10, 2)").HasColumnName("total_venta");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.Estado).HasMaxLength(20).IsUnicode(false).HasDefaultValue("PAGADA").HasColumnName("estado");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Venta_Cliente");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdTipoPago)
                .HasConstraintName("FK_Venta_Pago");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_Usuario");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Venta_Sucursal");
        });

        modelBuilder.Entity<VistaAlertaStock>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vista_alerta_stock");
            entity.Property(e => e.Tipo).HasMaxLength(10).IsUnicode(false).HasColumnName("tipo");
            entity.Property(e => e.Item).HasMaxLength(150).IsUnicode(false).HasColumnName("item");
            entity.Property(e => e.StockActual).HasColumnType("decimal(10, 3)").HasColumnName("stock_actual");
            entity.Property(e => e.StockMinimo).HasColumnType("decimal(10, 3)").HasColumnName("stock_minimo");
            entity.Property(e => e.Unidad).HasMaxLength(20).IsUnicode(false).HasColumnName("unidad");
            entity.Property(e => e.Estado).HasMaxLength(50).IsUnicode(false).HasColumnName("estado");
        });

        modelBuilder.Entity<VistaValoracionInventario>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vista_valoracion_inventario");
        });

        modelBuilder.Entity<VistaHistorialCompleto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vista_historial_completo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
