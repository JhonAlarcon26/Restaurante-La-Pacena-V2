using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Models;

namespace la_paceña_v2.Datos;

public partial class ContextoRestaurante : DbContext
{
    public ContextoRestaurante()
    {
    }

    public ContextoRestaurante(DbContextOptions<ContextoRestaurante> options)
        : base(options)
    {
    }

    public virtual DbSet<AbastecimientoPlato> AbastecimientoPlatos { get; set; }

    public virtual DbSet<CategoriaPlato> CategoriaPlatos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<CompraInsumo> Compras { get; set; }

    public virtual DbSet<CompraProducto> Abastecimientos { get; set; }

    public virtual DbSet<DetalleAbastecimientoPlato> DetalleAbastecimientoPlatos { get; set; }

    public virtual DbSet<DetalleCompraInsumo> DetalleCompras { get; set; }

    public virtual DbSet<DetalleCompraProducto> DetalleAbastecimientos { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVentas { get; set; }

    public virtual DbSet<HistorialDeMovimiento> HistorialDeMovimientos { get; set; }

    public virtual DbSet<Insumo> Insumos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Plato> Platos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Restaurante> Restaurantes { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolYPermiso> RolYPermisos { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockPlato> StockPlatos { get; set; }

    public virtual DbSet<TipoDePago> TipoDePagos { get; set; }

    public virtual DbSet<TipoProducto> TipoProductos { get; set; }

    public virtual DbSet<UnidadDeMedida> UnidadDeMedida { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-510MM2IC\\BD_JHON;Database=BD_la_pacena;User Id=sa;Password=75629487;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AbastecimientoPlato>(entity =>
        {
            entity.HasKey(e => e.IdAbastecimientoPlato);

            entity.ToTable("avastecimiento_plato");

            entity.HasIndex(e => e.IdUsuario, "IX_avastecimiento_plato_id_usuario");

            entity.Property(e => e.IdAbastecimientoPlato).HasColumnName("id_avastecimiento_plato");
            entity.Property(e => e.FechaAbastecimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_avastecimiento");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.TotalAbastecimiento)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_avastecimiento");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvasP_Usuario");
        });

        modelBuilder.Entity<CategoriaPlato>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__categori__CD54A221");

            entity.ToTable("categoria_plato");

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria_plato");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Ignore(e => e.ImpresoraDestino);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__cliente__677F38F5");

            entity.ToTable("cliente");

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.NitCarnet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nit_carnet");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("razon_social");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<CompraInsumo>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__compra__C4BAA604");

            entity.ToTable("compra_insumo");

            entity.HasIndex(e => e.IdProveedor, "IX_compra_insumo_id_proveedor");

            entity.HasIndex(e => e.IdUsuario, "IX_compra_insumo_id_usuario");

            entity.Property(e => e.IdCompra).HasColumnName("id_compra_insumo");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("COMPLETADA")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCompra)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_compra");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NroReferencia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nro_referencia");
            entity.Property(e => e.TotalCompra)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_compra");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Proveedor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Compra_Usuario");
        });

        modelBuilder.Entity<CompraProducto>(entity =>
        {
            entity.HasKey(e => e.IdAbastecimiento).HasName("PK__avasteci__53D4A869");

            entity.ToTable("compra_producto");

            entity.HasIndex(e => e.IdProveedor, "IX_compra_producto_id_proveedor");

            entity.HasIndex(e => e.IdUsuario, "IX_compra_producto_id_usuario");

            entity.Property(e => e.IdAbastecimiento).HasColumnName("id_compra_producto");
            entity.Property(e => e.FechaAbastecimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_compra");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NroReferencia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nro_referencia");
            entity.Property(e => e.TotalAbastecimiento)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_compra");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Abastecimientos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Avas_Proveedor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Abastecimientos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Avas_Usuario");
        });

        modelBuilder.Entity<DetalleAbastecimientoPlato>(entity =>
        {
            entity.HasKey(e => e.IdDetalleAbastecimientoPlato);

            entity.ToTable("detalle_avastecimiento_plato");

            entity.HasIndex(e => e.IdAbastecimientoPlato, "IX_detalle_avastecimiento_plato_id_avastecimiento_plato");

            entity.HasIndex(e => e.IdPlato, "IX_detalle_avastecimiento_plato_id_plato");

            entity.Property(e => e.IdDetalleAbastecimientoPlato).HasColumnName("id_detalle_avastecimiento_plato");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdAbastecimientoPlato).HasColumnName("id_avastecimiento_plato");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdAbastecimientoPlatoNavigation).WithMany(p => p.DetalleAbastecimientoPlatos)
                .HasForeignKey(d => d.IdAbastecimientoPlato)
                .HasConstraintName("FK_DetalleAP_Avaste");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.DetalleAbastecimientoPlatos)
                .HasForeignKey(d => d.IdPlato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleAP_Plato");
        });

        modelBuilder.Entity<DetalleCompraInsumo>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("PK__detalle___768CE6A5");

            entity.ToTable("detalle_compra_insumo");

            entity.HasIndex(e => e.IdCompra, "IX_detalle_compra_insumo_id_compra_insumo");

            entity.HasIndex(e => e.IdInsumo, "IX_detalle_compra_insumo_id_insumo");

            entity.Property(e => e.IdDetalleCompra).HasColumnName("id_detalle_compra_insumo");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("cantidad");
            entity.Property(e => e.IdCompra).HasColumnName("id_compra_insumo");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCI_Compra");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdInsumo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleC_Insumo");
        });

        modelBuilder.Entity<DetalleCompraProducto>(entity =>
        {
            entity.HasKey(e => e.IdDetalleAbastecimiento).HasName("PK__detalle___09384E3A");

            entity.ToTable("detalle_compra_producto");

            entity.HasIndex(e => e.IdAbastecimiento, "IX_detalle_compra_producto_id_compra_producto");

            entity.HasIndex(e => e.IdProducto, "IX_detalle_compra_producto_id_producto");

            entity.Property(e => e.IdDetalleAbastecimiento).HasColumnName("id_detalle_compra_producto");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdAbastecimiento).HasColumnName("id_compra_producto");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.PrecioVentaNuevo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_venta");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdAbastecimientoNavigation).WithMany(p => p.DetalleAbastecimientos)
                .HasForeignKey(d => d.IdAbastecimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCP_Compra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleAbastecimientos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleA_Producto");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.IdDetalleVenta).HasName("PK__detalle___B3000AF5");

            entity.ToTable("detalle_venta");

            entity.HasIndex(e => e.IdPlato, "IX_detalle_venta_id_plato");

            entity.HasIndex(e => e.IdProducto, "IX_detalle_venta_id_producto");

            entity.HasIndex(e => e.IdVenta, "IX_detalle_venta_id_ventas");

            entity.Property(e => e.IdDetalleVenta).HasColumnName("id_detalle_venta");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdVenta).HasColumnName("id_ventas");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.IdPlato)
                .HasConstraintName("FK_DetalleV_Plato");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_DetalleV_Producto");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVentas)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleV_Ventas");
        });

        modelBuilder.Entity<HistorialDeMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdHistorialMovimiento).HasName("PK__kardex_g__463A29B1");

            entity.ToTable("historial_movimientos");

            entity.HasIndex(e => e.IdInsumo, "IX_historial_movimientos_id_insumo");

            entity.HasIndex(e => e.IdPlato, "IX_historial_movimientos_id_plato");

            entity.HasIndex(e => e.IdProducto, "IX_historial_movimientos_id_producto");

            entity.Property(e => e.IdHistorialMovimiento).HasColumnName("id_historial_movimientos");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("cantidad");
            entity.Property(e => e.CostoUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo_unitario");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdReferencia).HasColumnName("id_referencia");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_movimiento");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.HistorialDeMovimientos)
                .HasForeignKey(d => d.IdInsumo)
                .HasConstraintName("FK_HistorialMov_Insumo");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.HistorialDeMovimientos)
                .HasForeignKey(d => d.IdPlato)
                .HasConstraintName("FK_HistorialMov_Plato");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.HistorialDeMovimientos)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_HistorialMov_Producto");
        });

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.IdInsumo).HasName("PK__insumo__DF9C369B");

            entity.ToTable("insumo");

            entity.HasIndex(e => e.IdUnidad, "IX_insumo_id_unidad_de_medida");

            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad_de_medida");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.Insumos)
                .HasForeignKey(d => d.IdUnidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Insumo_UnidadMedida");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__permisos__2D9638E7");

            entity.ToTable("permisos");

            entity.Property(e => e.IdPermiso).HasColumnName("id_permisos");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.IdPlato).HasName("PK__plato__6F37C223");

            entity.ToTable("plato");

            entity.HasIndex(e => e.IdCategoria, "IX_plato_id_categoria_plato");

            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.DescripcionCarta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion_carta");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria_plato");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Platos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Plato_CategoriaPlato");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__producto__1413F405");

            entity.ToTable("producto");

            entity.HasIndex(e => e.IdTipoProducto, "IX_producto_id_tipo_producto");

            entity.HasIndex(e => e.IdUnidad, "IX_producto_id_unidad_de_medida");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.CodigoBarras)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("codigo_barras");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.IdTipoProducto).HasColumnName("id_tipo_producto");
            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad_de_medida");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdTipoProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdTipoProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_TipoProducto");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdUnidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_UnidadMedida");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__proveedo__8D0CE3DF");

            entity.ToTable("proveedor");

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.ContactoNombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("contacto_nombre");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.NitCarnet)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nit_carnet");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre_empresa");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.IdRestaurante).HasName("PK__sucursal__32A559C3");

            entity.ToTable("restaurante");

            entity.Property(e => e.IdRestaurante).HasColumnName("id_restaurante");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ciudad");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__rol__6ABCB5E0");

            entity.ToTable("rol");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__permisos__2D9638E7");

            entity.ToTable("permisos");

            entity.Property(e => e.IdPermiso).HasColumnName("id_permisos");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<RolYPermiso>(entity =>
        {
            entity.HasKey(e => e.IdRolYPermiso).HasName("PK__rol_y_permiso");

            entity.ToTable("rol_y_permiso");

            entity.HasIndex(e => e.IdPermiso, "IX_rol_y_permiso_id_permisos");

            entity.HasIndex(e => e.IdRol, "IX_rol_y_permiso_id_rol");

            entity.Property(e => e.IdRolYPermiso).HasColumnName("id_rol_y_permiso");
            entity.Property(e => e.Controlador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("controlador");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.IdPermiso).HasColumnName("id_permisos");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Vista)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("vista");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.RolYPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Permisos");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolYPermisos)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Rol");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock).HasName("PK__stock__288BA44D");

            entity.ToTable("stock");

            entity.HasIndex(e => e.IdInsumo, "IX_stock_id_insumo");

            entity.HasIndex(e => e.IdProducto, "IX_stock_id_producto");

            entity.HasIndex(e => e.IdRestaurante, "IX_stock_id_restaurante");

            entity.Property(e => e.IdStock).HasColumnName("id_stock");
            entity.Property(e => e.CantidadActual)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("cantidad_actual");
            entity.Property(e => e.CantidadMinima)
                .HasDefaultValue(50m)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("cantidad_minima");
            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdRestaurante).HasColumnName("id_restaurante");
            entity.Property(e => e.UltimaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdInsumo)
                .HasConstraintName("FK_Stock_Insumo");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Stock_Producto");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany()
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Restaurante");
        });

        modelBuilder.Entity<StockPlato>(entity =>
        {
            entity.HasKey(e => e.IdStockPlato);

            entity.ToTable("stock_plato");

            entity.HasIndex(e => e.IdPlato, "IX_stock_plato_id_plato");

            entity.HasIndex(e => e.IdRestaurante, "IX_stock_plato_id_restaurante");

            entity.Property(e => e.IdStockPlato).HasColumnName("id_stock_plato");
            entity.Property(e => e.CantidadActual)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("cantidad_actual");
            entity.Property(e => e.CantidadMinima)
                .HasDefaultValue(50m)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("cantidad_minima");
            entity.Property(e => e.IdPlato).HasColumnName("id_plato");
            entity.Property(e => e.IdRestaurante).HasColumnName("id_restaurante");
            entity.Property(e => e.UltimaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdPlatoNavigation).WithMany(p => p.StockPlatos)
                .HasForeignKey(d => d.IdPlato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockP_Plato");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany()
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockP_Restaurante");
        });

        modelBuilder.Entity<TipoDePago>(entity =>
        {
            entity.HasKey(e => e.IdTipoPago).HasName("PK__tipo_de___6C2C5535");

            entity.ToTable("tipo_de_pago");

            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_de_pago");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoProducto>(entity =>
        {
            entity.HasKey(e => e.IdTipoProducto).HasName("PK__tipo_pro__3D5E7E2C");

            entity.ToTable("tipo_producto");

            entity.Property(e => e.IdTipoProducto).HasColumnName("id_tipo_producto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<UnidadDeMedida>(entity =>
        {
            entity.HasKey(e => e.IdUnidad).HasName("PK__unidad_d__2D93148F");

            entity.ToTable("unidad_de_medida");

            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad_de_medida");
            entity.Property(e => e.Abreviatura)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("abreviatura");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuario__4E510534");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.IdRestaurante, "IX_usuario_id_restaurante");

            entity.HasIndex(e => e.IdRol, "IX_usuario_id_rol");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.IdRestaurante).HasColumnName("id_restaurante");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Restaurante");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__ventas__4E2B9635");

            entity.ToTable("ventas");

            entity.HasIndex(e => e.IdCliente, "IX_ventas_id_cliente");

            entity.HasIndex(e => e.IdRestaurante, "IX_ventas_id_restaurante");

            entity.HasIndex(e => e.IdTipoPago, "IX_ventas_id_tipo_de_pago");

            entity.HasIndex(e => e.IdUsuario, "IX_ventas_id_usuario");

            entity.Property(e => e.IdVenta).HasColumnName("id_ventas");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PAGADA")
                .HasColumnName("estado");
            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_venta");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdRestaurante).HasColumnName("id_restaurante");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_de_pago");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NroMesa).HasColumnName("nro_mesa");
            entity.Property(e => e.TotalVenta)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_venta");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Ventas_Cliente");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdRestaurante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Restaurante");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdTipoPago)
                .HasConstraintName("FK_Ventas_TipoPago");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
