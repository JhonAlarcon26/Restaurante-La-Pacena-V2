using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace la_paceña_v2.Migrations
{
    /// <inheritdoc />
    public partial class MigracionLimpia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoria_plato",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    impresora_destino = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__CD54A221", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    razon_social = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    nit_carnet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cliente__677F38F5", x => x.id_cliente);
                });

            migrationBuilder.CreateTable(
                name: "proveedor",
                columns: table => new
                {
                    id_proveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_empresa = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    contacto_nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    nit_carnet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__proveedo__8D0CE3DF", x => x.id_proveedor);
                });

            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__rol__6ABCB5E0", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "sucursal",
                columns: table => new
                {
                    id_sucursal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    direccion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    ciudad = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__sucursal__32A559C3", x => x.id_sucursal);
                });

            migrationBuilder.CreateTable(
                name: "tipo_de_pago",
                columns: table => new
                {
                    id_tipo_pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tipo_de___6C2C5535", x => x.id_tipo_pago);
                });

            migrationBuilder.CreateTable(
                name: "tipo_producto",
                columns: table => new
                {
                    id_tipo_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tipo_pro__3D5E7E2C", x => x.id_tipo_producto);
                });

            migrationBuilder.CreateTable(
                name: "unidad_de_medida",
                columns: table => new
                {
                    id_unidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    abreviatura = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__unidad_d__2D93148F", x => x.id_unidad);
                });

            migrationBuilder.CreateTable(
                name: "plato",
                columns: table => new
                {
                    id_plato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    descripcion_carta = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__plato__6F37C223", x => x.id_plato);
                    table.ForeignKey(
                        name: "FK_Plato_Categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria_plato",
                        principalColumn: "id_categoria");
                });

            migrationBuilder.CreateTable(
                name: "permisos",
                columns: table => new
                {
                    id_permiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_rol = table.Column<int>(type: "int", nullable: true),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__permisos__2D9638E7", x => x.id_permiso);
                    table.ForeignKey(
                        name: "FK_Permiso_Rol",
                        column: x => x.id_rol,
                        principalTable: "rol",
                        principalColumn: "id_rol");
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    id_sucursal = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__usuario__4E510534", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol",
                        column: x => x.id_rol,
                        principalTable: "rol",
                        principalColumn: "id_rol");
                    table.ForeignKey(
                        name: "FK_Usuario_Sucursal",
                        column: x => x.id_sucursal,
                        principalTable: "sucursal",
                        principalColumn: "id_sucursal");
                });

            migrationBuilder.CreateTable(
                name: "insumo",
                columns: table => new
                {
                    id_insumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    id_unidad = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__insumo__DF9C369B", x => x.id_insumo);
                    table.ForeignKey(
                        name: "FK_Insumo_Unidad",
                        column: x => x.id_unidad,
                        principalTable: "unidad_de_medida",
                        principalColumn: "id_unidad");
                });

            migrationBuilder.CreateTable(
                name: "producto",
                columns: table => new
                {
                    id_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    codigo_barras = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    id_tipo_producto = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    id_unidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__producto__1413F405", x => x.id_producto);
                    table.ForeignKey(
                        name: "FK_Producto_TipoProducto",
                        column: x => x.id_tipo_producto,
                        principalTable: "tipo_producto",
                        principalColumn: "id_tipo_producto");
                    table.ForeignKey(
                        name: "FK_Producto_Unidad",
                        column: x => x.id_unidad,
                        principalTable: "unidad_de_medida",
                        principalColumn: "id_unidad");
                });

            migrationBuilder.CreateTable(
                name: "stock_plato",
                columns: table => new
                {
                    id_stock_plato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_plato = table.Column<int>(type: "int", nullable: false),
                    cantidad_actual = table.Column<decimal>(type: "decimal(10,3)", nullable: false, defaultValue: 0m),
                    cantidad_minima = table.Column<decimal>(type: "decimal(10,3)", nullable: false, defaultValue: 5m),
                    ultima_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_plato", x => x.id_stock_plato);
                    table.ForeignKey(
                        name: "FK_StockP_Plato",
                        column: x => x.id_plato,
                        principalTable: "plato",
                        principalColumn: "id_plato");
                });

            migrationBuilder.CreateTable(
                name: "avastecimiento_plato",
                columns: table => new
                {
                    id_avastecimiento_plato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha_avastecimiento = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    nro_referencia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    total_avastecimiento = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avastecimiento_plato", x => x.id_avastecimiento_plato);
                    table.ForeignKey(
                        name: "FK_AvasP_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "compra_insumo",
                columns: table => new
                {
                    id_compra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_proveedor = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha_compra = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    nro_referencia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    total_compra = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "COMPLETADA")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__compra__C4BAA604", x => x.id_compra);
                    table.ForeignKey(
                        name: "FK_Compra_Proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor");
                    table.ForeignKey(
                        name: "FK_Compra_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "compra_producto",
                columns: table => new
                {
                    id_avastecimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_proveedor = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha_avastecimiento = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    nro_referencia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    total_avastecimiento = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__avasteci__53D4A869", x => x.id_avastecimiento);
                    table.ForeignKey(
                        name: "FK_Avas_Proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor");
                    table.ForeignKey(
                        name: "FK_Avas_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    id_venta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    id_sucursal = table.Column<int>(type: "int", nullable: false),
                    fecha_venta = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    nro_mesa = table.Column<int>(type: "int", nullable: true),
                    total_venta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    id_tipo_pago = table.Column<int>(type: "int", nullable: true),
                    estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "PAGADA")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ventas__4E2B9635", x => x.id_venta);
                    table.ForeignKey(
                        name: "FK_Venta_Cliente",
                        column: x => x.id_cliente,
                        principalTable: "cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Venta_Pago",
                        column: x => x.id_tipo_pago,
                        principalTable: "tipo_de_pago",
                        principalColumn: "id_tipo_pago");
                    table.ForeignKey(
                        name: "FK_Venta_Sucursal",
                        column: x => x.id_sucursal,
                        principalTable: "sucursal",
                        principalColumn: "id_sucursal");
                    table.ForeignKey(
                        name: "FK_Venta_Usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "kardex_general",
                columns: table => new
                {
                    id_kardex = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    tipo_movimiento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    id_insumo = table.Column<int>(type: "int", nullable: true),
                    id_producto = table.Column<int>(type: "int", nullable: true),
                    id_plato = table.Column<int>(type: "int", nullable: true),
                    cantidad = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    costo_unitario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    id_referencia = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__kardex_g__463A29B1", x => x.id_kardex);
                    table.ForeignKey(
                        name: "FK_Kardex_Insumo",
                        column: x => x.id_insumo,
                        principalTable: "insumo",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "FK_Kardex_Plato",
                        column: x => x.id_plato,
                        principalTable: "plato",
                        principalColumn: "id_plato");
                    table.ForeignKey(
                        name: "FK_Kardex_Producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                });

            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    id_stock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_insumo = table.Column<int>(type: "int", nullable: true),
                    id_producto = table.Column<int>(type: "int", nullable: true),
                    cantidad_actual = table.Column<decimal>(type: "decimal(10,3)", nullable: false, defaultValue: 0m),
                    cantidad_minima = table.Column<decimal>(type: "decimal(10,3)", nullable: false, defaultValue: 5m),
                    ultima_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__stock__288BA44D", x => x.id_stock);
                    table.ForeignKey(
                        name: "FK_Stock_Insumo",
                        column: x => x.id_insumo,
                        principalTable: "insumo",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "FK_Stock_Producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                });

            migrationBuilder.CreateTable(
                name: "detalle_avastecimiento_plato",
                columns: table => new
                {
                    id_detalle_avastecimiento_plato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_avastecimiento_plato = table.Column<int>(type: "int", nullable: false),
                    id_plato = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_avastecimiento_plato", x => x.id_detalle_avastecimiento_plato);
                    table.ForeignKey(
                        name: "FK_DetalleAP_Avaste",
                        column: x => x.id_avastecimiento_plato,
                        principalTable: "avastecimiento_plato",
                        principalColumn: "id_avastecimiento_plato",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleAP_Plato",
                        column: x => x.id_plato,
                        principalTable: "plato",
                        principalColumn: "id_plato");
                });

            migrationBuilder.CreateTable(
                name: "detalle_compra_insumo",
                columns: table => new
                {
                    id_detalle_compra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_compra = table.Column<int>(type: "int", nullable: false),
                    id_insumo = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__detalle___768CE6A5", x => x.id_detalle_compra);
                    table.ForeignKey(
                        name: "FK_DetalleC_Compra",
                        column: x => x.id_compra,
                        principalTable: "compra_insumo",
                        principalColumn: "id_compra");
                    table.ForeignKey(
                        name: "FK_DetalleC_Insumo",
                        column: x => x.id_insumo,
                        principalTable: "insumo",
                        principalColumn: "id_insumo");
                });

            migrationBuilder.CreateTable(
                name: "detalle_compra_producto",
                columns: table => new
                {
                    id_detalle_avastecimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_avastecimiento = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__detalle___09384E3A", x => x.id_detalle_avastecimiento);
                    table.ForeignKey(
                        name: "FK_DetalleA_Avaste",
                        column: x => x.id_avastecimiento,
                        principalTable: "compra_producto",
                        principalColumn: "id_avastecimiento");
                    table.ForeignKey(
                        name: "FK_DetalleA_Producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                });

            migrationBuilder.CreateTable(
                name: "detalle_venta",
                columns: table => new
                {
                    id_detalle_venta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_venta = table.Column<int>(type: "int", nullable: false),
                    id_plato = table.Column<int>(type: "int", nullable: true),
                    id_producto = table.Column<int>(type: "int", nullable: true),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__detalle___B3000AF5", x => x.id_detalle_venta);
                    table.ForeignKey(
                        name: "FK_DetalleV_Plato",
                        column: x => x.id_plato,
                        principalTable: "plato",
                        principalColumn: "id_plato");
                    table.ForeignKey(
                        name: "FK_DetalleV_Producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                    table.ForeignKey(
                        name: "FK_DetalleV_Venta",
                        column: x => x.id_venta,
                        principalTable: "ventas",
                        principalColumn: "id_venta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_avastecimiento_plato_id_usuario",
                table: "avastecimiento_plato",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_compra_insumo_id_proveedor",
                table: "compra_insumo",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_compra_insumo_id_usuario",
                table: "compra_insumo",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_compra_producto_id_proveedor",
                table: "compra_producto",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_compra_producto_id_usuario",
                table: "compra_producto",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_avastecimiento_plato_id_avastecimiento_plato",
                table: "detalle_avastecimiento_plato",
                column: "id_avastecimiento_plato");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_avastecimiento_plato_id_plato",
                table: "detalle_avastecimiento_plato",
                column: "id_plato");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_insumo_id_compra",
                table: "detalle_compra_insumo",
                column: "id_compra");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_insumo_id_insumo",
                table: "detalle_compra_insumo",
                column: "id_insumo");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_producto_id_avastecimiento",
                table: "detalle_compra_producto",
                column: "id_avastecimiento");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_producto_id_producto",
                table: "detalle_compra_producto",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_venta_id_plato",
                table: "detalle_venta",
                column: "id_plato");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_venta_id_producto",
                table: "detalle_venta",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_venta_id_venta",
                table: "detalle_venta",
                column: "id_venta");

            migrationBuilder.CreateIndex(
                name: "IX_insumo_id_unidad",
                table: "insumo",
                column: "id_unidad");

            migrationBuilder.CreateIndex(
                name: "IX_kardex_general_id_insumo",
                table: "kardex_general",
                column: "id_insumo");

            migrationBuilder.CreateIndex(
                name: "IX_kardex_general_id_plato",
                table: "kardex_general",
                column: "id_plato");

            migrationBuilder.CreateIndex(
                name: "IX_kardex_general_id_producto",
                table: "kardex_general",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_permisos_id_rol",
                table: "permisos",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_plato_id_categoria",
                table: "plato",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_tipo_producto",
                table: "producto",
                column: "id_tipo_producto");

            migrationBuilder.CreateIndex(
                name: "IX_producto_id_unidad",
                table: "producto",
                column: "id_unidad");

            migrationBuilder.CreateIndex(
                name: "IX_stock_id_insumo",
                table: "stock",
                column: "id_insumo");

            migrationBuilder.CreateIndex(
                name: "IX_stock_id_producto",
                table: "stock",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_stock_plato_id_plato",
                table: "stock_plato",
                column: "id_plato");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_id_rol",
                table: "usuario",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_id_sucursal",
                table: "usuario",
                column: "id_sucursal");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_id_cliente",
                table: "ventas",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_id_sucursal",
                table: "ventas",
                column: "id_sucursal");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_id_tipo_pago",
                table: "ventas",
                column: "id_tipo_pago");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_id_usuario",
                table: "ventas",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_avastecimiento_plato");

            migrationBuilder.DropTable(
                name: "detalle_compra_insumo");

            migrationBuilder.DropTable(
                name: "detalle_compra_producto");

            migrationBuilder.DropTable(
                name: "detalle_venta");

            migrationBuilder.DropTable(
                name: "kardex_general");

            migrationBuilder.DropTable(
                name: "permisos");

            migrationBuilder.DropTable(
                name: "stock");

            migrationBuilder.DropTable(
                name: "stock_plato");

            migrationBuilder.DropTable(
                name: "avastecimiento_plato");

            migrationBuilder.DropTable(
                name: "compra_insumo");

            migrationBuilder.DropTable(
                name: "compra_producto");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "insumo");

            migrationBuilder.DropTable(
                name: "producto");

            migrationBuilder.DropTable(
                name: "plato");

            migrationBuilder.DropTable(
                name: "proveedor");

            migrationBuilder.DropTable(
                name: "cliente");

            migrationBuilder.DropTable(
                name: "tipo_de_pago");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "tipo_producto");

            migrationBuilder.DropTable(
                name: "unidad_de_medida");

            migrationBuilder.DropTable(
                name: "categoria_plato");

            migrationBuilder.DropTable(
                name: "rol");

            migrationBuilder.DropTable(
                name: "sucursal");
        }
    }
}
