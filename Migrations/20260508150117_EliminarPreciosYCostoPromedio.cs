using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace la_paceña_v2.Migrations
{
    /// <inheritdoc />
    public partial class EliminarPreciosYCostoPromedio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string dropColumnSql = @"
                DECLARE @ConstraintName nvarchar(200);
                SELECT @ConstraintName = Name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('{0}') AND parent_column_id = (SELECT column_id FROM sys.columns WHERE name = '{1}' AND object_id = OBJECT_ID('{0}'));
                IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE {0} DROP CONSTRAINT ' + @ConstraintName);
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('{0}') AND name = '{1}') ALTER TABLE {0} DROP COLUMN {1};
            ";

            migrationBuilder.Sql(string.Format(dropColumnSql, "producto", "precio_compra_actual"));
            migrationBuilder.Sql(string.Format(dropColumnSql, "producto", "precio_venta"));
            migrationBuilder.Sql(string.Format(dropColumnSql, "plato", "precio_venta"));
            migrationBuilder.Sql(string.Format(dropColumnSql, "insumo", "costo_promedio"));

            // Asegurar que no haya nulos antes de cambiar a NOT NULL
            migrationBuilder.Sql("UPDATE ventas SET id_cliente = 1 WHERE id_cliente IS NULL");
            
            migrationBuilder.Sql("ALTER TABLE ventas ALTER COLUMN id_cliente int NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kardex_Plato",
                table: "kardex_general");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Cliente",
                table: "ventas");

            migrationBuilder.DropTable(
                name: "detalle_avastecimiento_plato");

            migrationBuilder.DropTable(
                name: "detalle_compra_producto");

            migrationBuilder.DropTable(
                name: "stock_plato");

            migrationBuilder.DropTable(
                name: "avastecimiento_plato");

            migrationBuilder.DropTable(
                name: "compra_producto");

            migrationBuilder.DropIndex(
                name: "IX_kardex_general_id_plato",
                table: "kardex_general");

            migrationBuilder.DropColumn(
                name: "id_plato",
                table: "kardex_general");

            migrationBuilder.RenameTable(
                name: "detalle_compra_insumo",
                newName: "detalle_compra");

            migrationBuilder.RenameTable(
                name: "compra_insumo",
                newName: "compra");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "tipo_producto",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "id_rol_permiso",
                table: "rol_permiso",
                newName: "IdRolPermiso");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "rol",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "proveedor",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "proveedor",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "contacto_nombre",
                table: "proveedor",
                newName: "ContactoNombre");

            migrationBuilder.RenameColumn(
                name: "codigo_barras",
                table: "producto",
                newName: "CodigoBarras");

            migrationBuilder.RenameColumn(
                name: "descripcion_carta",
                table: "plato",
                newName: "DescripcionCarta");

            migrationBuilder.RenameColumn(
                name: "costo_unitario",
                table: "kardex_general",
                newName: "CostoUnitario");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "cliente",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "cliente",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "impresora_destino",
                table: "categoria_plato",
                newName: "ImpresoraDestino");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_compra_insumo_id_insumo",
                table: "detalle_compra",
                newName: "IX_detalle_compra_id_insumo");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_compra_insumo_id_compra",
                table: "detalle_compra",
                newName: "IX_detalle_compra_id_compra");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "compra",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "nro_referencia",
                table: "compra",
                newName: "nro_factura_proveedor");

            migrationBuilder.RenameIndex(
                name: "IX_compra_insumo_id_usuario",
                table: "compra",
                newName: "IX_compra_id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_compra_insumo_id_proveedor",
                table: "compra",
                newName: "IX_compra_id_proveedor");

            migrationBuilder.AlterColumn<int>(
                name: "id_cliente",
                table: "ventas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TipoMagnitud",
                table: "unidad_de_medida",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "tipo_producto",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsBancarizado",
                table: "tipo_de_pago",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "rol",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "proveedor",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "proveedor",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactoNombre",
                table: "proveedor",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldUnicode: false,
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoBarras",
                table: "producto",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_compra_actual",
                table: "producto",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_venta",
                table: "producto",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionCarta",
                table: "plato",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_venta",
                table: "plato",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "costo_promedio",
                table: "insumo",
                type: "decimal(10,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "cliente",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "cliente",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImpresoraDestino",
                table: "categoria_plato",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "detalle_compra",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "compra",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "COMPLETADA");

            migrationBuilder.CreateTable(
                name: "avastecimiento",
                columns: table => new
                {
                    id_avastecimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_proveedor = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha_avastecimiento = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    nro_factura_proveedor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
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
                name: "receta",
                columns: table => new
                {
                    id_receta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_insumo = table.Column<int>(type: "int", nullable: false),
                    id_plato = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<decimal>(type: "decimal(10,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__receta__11E397AF", x => x.id_receta);
                    table.ForeignKey(
                        name: "FK_Receta_Insumo",
                        column: x => x.id_insumo,
                        principalTable: "insumo",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "FK_Receta_Plato",
                        column: x => x.id_plato,
                        principalTable: "plato",
                        principalColumn: "id_plato");
                });

            migrationBuilder.CreateTable(
                name: "detalle_avastecimiento",
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
                        principalTable: "avastecimiento",
                        principalColumn: "id_avastecimiento");
                    table.ForeignKey(
                        name: "FK_DetalleA_Producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto");
                });

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_ProductoIdProducto",
                table: "detalle_compra",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_avastecimiento_id_proveedor",
                table: "avastecimiento",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_avastecimiento_id_usuario",
                table: "avastecimiento",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_avastecimiento_id_avastecimiento",
                table: "detalle_avastecimiento",
                column: "id_avastecimiento");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_avastecimiento_id_producto",
                table: "detalle_avastecimiento",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_receta_id_insumo",
                table: "receta",
                column: "id_insumo");

            migrationBuilder.CreateIndex(
                name: "IX_receta_id_plato",
                table: "receta",
                column: "id_plato");

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_compra_producto_ProductoIdProducto",
                table: "detalle_compra",
                column: "ProductoIdProducto",
                principalTable: "producto",
                principalColumn: "id_producto");

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Cliente",
                table: "ventas",
                column: "id_cliente",
                principalTable: "cliente",
                principalColumn: "id_cliente");
        }
    }
}
