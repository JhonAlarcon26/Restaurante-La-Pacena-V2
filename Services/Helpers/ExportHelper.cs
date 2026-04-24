using ClosedXML.Excel;
using la_paceña_v2.Services.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace la_paceña_v2.Services.Helpers
{
    public static class ExportHelper
    {
        public static byte[] ExportarVentasExcel(ReporteVentasDTO reporte)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Ventas");

                // Encabezado Profesional
                worksheet.Cell("A1").Value = "RESTAURANTE LA PACEÑA";
                worksheet.Cell("A1").Style.Font.FontSize = 16;
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Range("A1:G1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = "REPORTE DETALLADO DE VENTAS";
                worksheet.Cell("A2").Style.Font.FontSize = 12;
                worksheet.Range("A2:G2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A3").Value = $"Período: {reporte.FechaInicio:dd/MM/yyyy} - {reporte.FechaFin:dd/MM/yyyy}";
                worksheet.Range("A3:G3").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // KPIs en el Excel
                var kpiBox = worksheet.Range("A5:C6");
                kpiBox.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                kpiBox.Style.Fill.BackgroundColor = XLColor.GhostWhite;

                worksheet.Cell("A5").Value = "Total Ventas:";
                worksheet.Cell("A5").Style.Font.Bold = true;
                worksheet.Cell("B5").Value = reporte.TotalVentas;
                worksheet.Cell("B5").Style.NumberFormat.Format = "\"Bs.\" #,##0.00";

                worksheet.Cell("A6").Value = "Transacciones:";
                worksheet.Cell("A6").Style.Font.Bold = true;
                worksheet.Cell("B6").Value = reporte.CantidadTransacciones;

                // Tabla de datos
                var currentRow = 8;
                var columns = new[] { "Fecha", "Cliente", "Sucursal", "Usuario", "Tipo Pago", "Estado", "Total" };
                for (int i = 0; i < columns.Length; i++)
                {
                    var cell = worksheet.Cell(currentRow, i + 1);
                    cell.Value = columns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

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
                    worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                }

                // Fórmulas Matemáticas Automáticas
                currentRow++;
                worksheet.Cell(currentRow, 6).Value = "TOTAL FINAL:";
                worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                
                // Fórmula de suma para la columna G
                worksheet.Cell(currentRow, 7).FormulaA1 = $"=SUM(G9:G{currentRow - 1})";
                worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "\"Bs.\" #,##0.00";
                worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.Yellow;

                worksheet.Columns().AdjustToContents();
                worksheet.Range(8, 1, currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(8, 1, currentRow, 7).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

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

                // Encabezado Profesional
                worksheet.Cell("A1").Value = "RESTAURANTE LA PACEÑA";
                worksheet.Cell("A1").Style.Font.FontSize = 16;
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Range("A1:F1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = "REPORTE DETALLADO DE COMPRAS";
                worksheet.Cell("A2").Style.Font.FontSize = 12;
                worksheet.Range("A2:F2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A3").Value = $"Período: {reporte.FechaInicio:dd/MM/yyyy} - {reporte.FechaFin:dd/MM/yyyy}";
                worksheet.Range("A3:F3").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // KPIs
                var kpiBox = worksheet.Range("A5:C6");
                kpiBox.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                kpiBox.Style.Fill.BackgroundColor = XLColor.GhostWhite;

                worksheet.Cell("A5").Value = "Total Compras:";
                worksheet.Cell("A5").Style.Font.Bold = true;
                worksheet.Cell("B5").Value = reporte.TotalCompras;
                worksheet.Cell("B5").Style.NumberFormat.Format = "\"Bs.\" #,##0.00";

                worksheet.Cell("A6").Value = "Transacciones:";
                worksheet.Cell("A6").Style.Font.Bold = true;
                worksheet.Cell("B6").Value = reporte.CantidadTransacciones;

                // Tabla
                var currentRow = 8;
                var columns = new[] { "Fecha", "Proveedor", "Usuario", "Nro. Factura", "Estado", "Total" };
                for (int i = 0; i < columns.Length; i++)
                {
                    var cell = worksheet.Cell(currentRow, i + 1);
                    cell.Value = columns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                foreach (var item in reporte.Detalles)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Fecha;
                    worksheet.Cell(currentRow, 2).Value = item.Proveedor;
                    worksheet.Cell(currentRow, 3).Value = item.Usuario;
                    worksheet.Cell(currentRow, 4).Value = item.NroFactura;
                    worksheet.Cell(currentRow, 5).Value = item.Estado;
                    worksheet.Cell(currentRow, 6).Value = item.Total;
                    worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                }

                // Fórmula Matemática
                currentRow++;
                worksheet.Cell(currentRow, 5).Value = "TOTAL FINAL:";
                worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(currentRow, 6).FormulaA1 = $"=SUM(F9:F{currentRow - 1})";
                worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "\"Bs.\" #,##0.00";
                worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.Yellow;

                worksheet.Columns().AdjustToContents();
                worksheet.Range(8, 1, currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(8, 1, currentRow, 6).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

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
                            column.Item().Text("Restaurante La Paceña").FontSize(20).SemiBold().FontColor(Colors.Black);
                            column.Item().Text($"Reporte de Compras").FontSize(16);
                            column.Item().Text($"Período: {reporte.FechaInicio:dd/MM/yyyy} - {reporte.FechaFin:dd/MM/yyyy}");
                        });

                        row.ConstantItem(100).Column(column =>
                        {
                            column.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            column.Item().AlignRight().Text("DOCUMENTO OFICIAL").FontSize(8).FontColor(Colors.Grey.Medium);
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
                            column.Item().Text("Restaurante La Paceña").FontSize(20).SemiBold().FontColor(Colors.Black);
                            column.Item().Text($"Reporte de Ventas").FontSize(16);
                            column.Item().Text($"Período: {reporte.FechaInicio:dd/MM/yyyy} - {reporte.FechaFin:dd/MM/yyyy}");
                        });

                        row.ConstantItem(100).Column(column =>
                        {
                            column.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            column.Item().AlignRight().Text("DOCUMENTO OFICIAL").FontSize(8).FontColor(Colors.Grey.Medium);
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

        public static byte[] ExportarStockExcel(List<ReporteStockDTO> reporte)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stock Actual");

                // Encabezado Profesional
                worksheet.Cell("A1").Value = "RESTAURANTE LA PACEÑA";
                worksheet.Cell("A1").Style.Font.FontSize = 16;
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Range("A1:G1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A2").Value = "REPORTE DE INVENTARIO Y VALORACIÓN";
                worksheet.Cell("A2").Style.Font.FontSize = 12;
                worksheet.Range("A2:G2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell("A3").Value = $"Fecha de Generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Range("A3:G3").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // KPIs
                var kpiBox = worksheet.Range("A5:C6");
                kpiBox.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                kpiBox.Style.Fill.BackgroundColor = XLColor.GhostWhite;

                worksheet.Cell("A5").Value = "Valor Total:";
                worksheet.Cell("A5").Style.Font.Bold = true;
                worksheet.Cell("B5").Value = reporte.Sum(s => s.ValorTotal);
                worksheet.Cell("B5").Style.NumberFormat.Format = "\"Bs.\" #,##0.00";

                worksheet.Cell("A6").Value = "Items en Alerta:";
                worksheet.Cell("A6").Style.Font.Bold = true;
                worksheet.Cell("B6").Value = reporte.Count(s => s.EnAlerta);
                worksheet.Cell("B6").Style.Font.FontColor = XLColor.Red;

                // Tabla de datos
                var currentRow = 8;
                var columns = new[] { "Tipo", "Nombre", "Unidad", "Stock Actual", "Mínimo", "Costo Prom.", "Valor Total" };
                for (int i = 0; i < columns.Length; i++)
                {
                    var cell = worksheet.Cell(currentRow, i + 1);
                    cell.Value = columns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                foreach (var item in reporte)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Tipo;
                    worksheet.Cell(currentRow, 2).Value = item.Nombre;
                    worksheet.Cell(currentRow, 3).Value = item.Unidad;
                    worksheet.Cell(currentRow, 4).Value = item.StockActual;
                    worksheet.Cell(currentRow, 5).Value = item.StockMinimo;
                    worksheet.Cell(currentRow, 6).Value = item.CostoPromedio;
                    worksheet.Cell(currentRow, 7).Value = item.ValorTotal;
                    
                    worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";

                    if (item.EnAlerta)
                    {
                        worksheet.Cell(currentRow, 4).Style.Font.FontColor = XLColor.Red;
                        worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                    }
                }

                currentRow++;
                worksheet.Cell(currentRow, 6).Value = "VALOR TOTAL:";
                worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(currentRow, 7).FormulaA1 = $"=SUM(G9:G{currentRow - 1})";
                worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "\"Bs.\" #,##0.00";
                worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.Yellow;

                worksheet.Columns().AdjustToContents();
                worksheet.Range(8, 1, currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(8, 1, currentRow, 7).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] ExportarStockPDF(List<ReporteStockDTO> reporte)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Element(header =>
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Restaurante La Paceña").FontSize(20).SemiBold().FontColor(Colors.Black);
                                column.Item().Text("Reporte de Existencias (Stock)").FontSize(14);
                                column.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                            });
                        });
                    });

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Component(new KpiComponent("Valor Total Stock", $"Bs. {reporte.Sum(s => s.ValorTotal):N2}"));
                            row.RelativeItem().Component(new KpiComponent("Items en Alerta", $"{reporte.Count(s => s.EnAlerta)}"));
                            row.RelativeItem().Component(new KpiComponent("Total de Items", $"{reporte.Count}"));
                        });

                        column.Item().PaddingTop(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);  // Tipo
                                columns.RelativeColumn();    // Nombre
                                columns.ConstantColumn(40);  // Unidad
                                columns.ConstantColumn(60);  // Stock
                                columns.ConstantColumn(60);  // Minimo
                                columns.ConstantColumn(70);  // Valor Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Tipo");
                                header.Cell().Element(CellStyle).Text("Nombre");
                                header.Cell().Element(CellStyle).Text("Und.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Stock");
                                header.Cell().Element(CellStyle).AlignRight().Text("Min.");
                                header.Cell().Element(CellStyle).AlignRight().Text("V. Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var item in reporte)
                            {
                                table.Cell().Element(CellStyle).Text(item.Tipo);
                                table.Cell().Element(CellStyle).Text(item.Nombre);
                                table.Cell().Element(CellStyle).Text(item.Unidad);
                                table.Cell().Element(CellStyle).AlignRight().Text(item.StockActual.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.StockMinimo.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.ValorTotal.ToString("N2"));

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
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
