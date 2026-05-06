using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace la_paceña_v2.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarTablasComprasV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "detalle_compra",
                newName: "detalle_compra_insumo");

            migrationBuilder.RenameTable(
                name: "detalle_avastecimiento",
                newName: "detalle_compra_producto");

            migrationBuilder.RenameTable(
                name: "compra",
                newName: "compra_insumo");

            migrationBuilder.RenameTable(
                name: "avastecimiento",
                newName: "compra_producto");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "detalle_compra_producto",
                newName: "detalle_avastecimiento");

            migrationBuilder.RenameTable(
                name: "detalle_compra_insumo",
                newName: "detalle_compra");

            migrationBuilder.RenameTable(
                name: "compra_producto",
                newName: "avastecimiento");

            migrationBuilder.RenameTable(
                name: "compra_insumo",
                newName: "compra");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_compra_producto_id_producto",
                table: "detalle_avastecimiento",
                newName: "IX_detalle_avastecimiento_id_producto");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_compra_producto_id_avastecimiento",
                table: "detalle_avastecimiento",
                newName: "IX_detalle_avastecimiento_id_avastecimiento");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_compra_insumo_id_insumo",
                table: "detalle_compra",
                newName: "IX_detalle_compra_id_insumo");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_compra_insumo_id_compra",
                table: "detalle_compra",
                newName: "IX_detalle_compra_id_compra");

            migrationBuilder.RenameIndex(
                name: "IX_compra_producto_id_usuario",
                table: "avastecimiento",
                newName: "IX_avastecimiento_id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_compra_producto_id_proveedor",
                table: "avastecimiento",
                newName: "IX_avastecimiento_id_proveedor");

            migrationBuilder.RenameIndex(
                name: "IX_compra_insumo_id_usuario",
                table: "compra",
                newName: "IX_compra_id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_compra_insumo_id_proveedor",
                table: "compra",
                newName: "IX_compra_id_proveedor");
        }
    }
}
