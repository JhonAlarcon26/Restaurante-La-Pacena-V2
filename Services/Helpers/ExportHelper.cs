using System;
using System.IO;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using la_paceña_v2.Services.DTOs;

namespace la_paceña_v2.Services.Helpers
{
    public static class ExportHelper
    {
        public static byte[] ExportarVentasExcel(ReporteVentasDTO reporte)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Ventas");

                worksheet.Cell("A1").Value = "REPORTE DE VENTAS";
                worksheet.Range("A1:G1").Merge().Style.Font.Bold = true;
                worksheet.Range("A1:G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = $"Desde: {reporte.FechaInicio:dd/MM/yyyy} Hasta: {reporte.FechaFin:dd/MM/yyyy}";
                worksheet.Cell("A3").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

                worksheet.Cell("A5").Value = "Total Ventas";
                worksheet.Cell("B5").Value = reporte.TotalVentas;
                worksheet.Cell("C5").Value = "Transacciones";
                worksheet.Cell("D5").Value = reporte.CantidadTransacciones;
                worksheet.Cell("E5").Value = "Ticket Promedio";
                worksheet.Cell("F5").Value = reporte.TicketPromedio;

                var currentRow = 7;
                worksheet.Cell(currentRow, 1).Value = "Fecha";
                worksheet.Cell(currentRow, 2).Value = "Cliente";
                worksheet.Cell(currentRow, 3).Value = "Sucursal";
                worksheet.Cell(currentRow, 4).Value = "Usuario";
                worksheet.Cell(currentRow, 5).Value = "Tipo Pago";
                worksheet.Cell(currentRow, 6).Value = "Estado";
                worksheet.Cell(currentRow, 7).Value = "Total";
                
                var headerRange = worksheet.Range(currentRow, 1, currentRow, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                foreach (var item in reporte.Detalles)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Fecha;
                    worksheet.Cell(currentRow, 2).Value = item.Cliente;
                    worksheet.Cell(currentRow, 3).Value = item.Sucursal;
                    worksheet.Cell(currentRow, 4).Value = item.Usuario;
                    worksheet.Cell(currentRow, 5).Value = item.TipoPago;
                    worksheet.Cell(currentRow, 6).Value = item.Estado;
                    worksheet.Cell(currentRow, 7).Value = item.Total;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] ExportarComprasExcel(ReporteComprasDTO reporte)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Compras");

                worksheet.Cell("A1").Value = "REPORTE DE COMPRAS";
                worksheet.Range("A1:F1").Merge().Style.Font.Bold = true;
                worksheet.Range("A1:F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = $"Desde: {reporte.FechaInicio:dd/MM/yyyy} Hasta: {reporte.FechaFin:dd/MM/yyyy}";
                worksheet.Cell("A3").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

                worksheet.Cell("A5").Value = "Total Compras";
                worksheet.Cell("B5").Value = reporte.TotalCompras;
                worksheet.Cell("C5").Value = "Transacciones";
                worksheet.Cell("D5").Value = reporte.CantidadTransacciones;

                var currentRow = 7;
                worksheet.Cell(currentRow, 1).Value = "Fecha";
                worksheet.Cell(currentRow, 2).Value = "Proveedor";
                worksheet.Cell(currentRow, 3).Value = "Usuario";
                worksheet.Cell(currentRow, 4).Value = "Nro. Factura";
                worksheet.Cell(currentRow, 5).Value = "Estado";
                worksheet.Cell(currentRow, 6).Value = "Total";
                
                var headerRange = worksheet.Range(currentRow, 1, currentRow, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                foreach (var item in reporte.Detalles)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Fecha;
                    worksheet.Cell(currentRow, 2).Value = item.Proveedor;
                    worksheet.Cell(currentRow, 3).Value = item.Usuario;
                    worksheet.Cell(currentRow, 4).Value = item.NroFactura;
                    worksheet.Cell(currentRow, 5).Value = item.Estado;
                    worksheet.Cell(currentRow, 6).Value = item.Total;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] ExportarComprasPDF(ReporteComprasDTO reporte)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });

                void ComposeHeader(IContainer container)
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Restaurante La Paceña").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            column.Item().Text($"Reporte de Compras").FontSize(16);
                            column.Item().Text($"Período: {reporte.FechaInicio:dd/MM/yyyy} - {reporte.FechaFin:dd/MM/yyyy}");
                        });

                        row.ConstantItem(100).Column(column =>
                        {
                            column.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            column.Item().AlignRight().Text("Confidencial").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(10).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Component(new KpiComponent("Total Compras", $"Bs. {reporte.TotalCompras:N2}"));
                            row.RelativeItem().Component(new KpiComponent("Transacciones", $"{reporte.CantidadTransacciones}"));
                        });

                        column.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80); // Fecha
                                columns.RelativeColumn();   // Proveedor
                                columns.RelativeColumn();   // Usuario
                                columns.RelativeColumn();   // Nro Factura
                                columns.ConstantColumn(80); // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Fecha");
                                header.Cell().Element(CellStyle).Text("Proveedor");
                                header.Cell().Element(CellStyle).Text("Usuario");
                                header.Cell().Element(CellStyle).Text("Nro. Factura");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var item in reporte.Detalles)
                            {
                                table.Cell().Element(CellStyle).Text($"{item.Fecha:dd/MM/yyyy}");
                                table.Cell().Element(CellStyle).Text(item.Proveedor);
                                table.Cell().Element(CellStyle).Text(item.Usuario);
                                table.Cell().Element(CellStyle).Text(item.NroFactura);
                                table.Cell().Element(CellStyle).AlignRight().Text($"Bs. {item.Total:N2}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                    });
                }
            });

            return document.GeneratePdf();
        }

        public static byte[] ExportarVentasPDF(ReporteVentasDTO reporte)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });

                void ComposeHeader(IContainer container)
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Restaurante La Paceña").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            column.Item().Text($"Reporte de Ventas").FontSize(16);
                            column.Item().Text($"Período: {reporte.FechaInicio:dd/MM/yyyy} - {reporte.FechaFin:dd/MM/yyyy}");
                        });

                        row.ConstantItem(100).Column(column =>
                        {
                            column.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            column.Item().AlignRight().Text("Confidencial").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(10).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Component(new KpiComponent("Total Ventas", $"Bs. {reporte.TotalVentas:N2}"));
                            row.RelativeItem().Component(new KpiComponent("Transacciones", $"{reporte.CantidadTransacciones}"));
                            row.RelativeItem().Component(new KpiComponent("Ticket Promedio", $"Bs. {reporte.TicketPromedio:N2}"));
                        });

                        column.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80); // Fecha
                                columns.RelativeColumn();   // Cliente
                                columns.RelativeColumn();   // Sucursal
                                columns.RelativeColumn();   // Usuario
                                columns.ConstantColumn(80); // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Fecha");
                                header.Cell().Element(CellStyle).Text("Cliente");
                                header.Cell().Element(CellStyle).Text("Sucursal");
                                header.Cell().Element(CellStyle).Text("Usuario");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var item in reporte.Detalles)
                            {
                                table.Cell().Element(CellStyle).Text($"{item.Fecha:dd/MM/yyyy}");
                                table.Cell().Element(CellStyle).Text(item.Cliente);
                                table.Cell().Element(CellStyle).Text(item.Sucursal);
                                table.Cell().Element(CellStyle).Text(item.Usuario);
                                table.Cell().Element(CellStyle).AlignRight().Text($"Bs. {item.Total:N2}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                    });
                }
            });

            return document.GeneratePdf();
        }

        private class KpiComponent : IComponent
        {
            private string Title { get; }
            private string Value { get; }

            public KpiComponent(string title, string value)
            {
                Title = title;
                Value = value;
            }

            public void Compose(IContainer container)
            {
                container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(column =>
                {
                    column.Item().Text(Title).FontSize(10).FontColor(Colors.Grey.Medium);
                    column.Item().Text(Value).FontSize(16).SemiBold();
                });
            }
        }
    }
}
