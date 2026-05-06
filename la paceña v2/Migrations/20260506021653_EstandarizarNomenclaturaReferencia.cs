using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace la_paceña_v2.Migrations
{
    /// <inheritdoc />
    public partial class EstandarizarNomenclaturaReferencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nro_factura_proveedor",
                table: "compra_producto",
                newName: "nro_referencia");

            migrationBuilder.RenameColumn(
                name: "nro_factura_proveedor",
                table: "compra_insumo",
                newName: "nro_referencia");

            migrationBuilder.RenameColumn(
                name: "nro_factura_proveedor",
                table: "avastecimiento_plato",
                newName: "nro_referencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nro_referencia",
                table: "compra_producto",
                newName: "nro_factura_proveedor");

            migrationBuilder.RenameColumn(
                name: "nro_referencia",
                table: "compra_insumo",
                newName: "nro_factura_proveedor");

            migrationBuilder.RenameColumn(
                name: "nro_referencia",
                table: "avastecimiento_plato",
                newName: "nro_factura_proveedor");
        }
    }
}
