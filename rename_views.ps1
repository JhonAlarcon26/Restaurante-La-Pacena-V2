$ruta = [System.IO.Path]::GetFullPath("Views")
$archivos = Get-ChildItem -Path $ruta -Recurse -Include "*.cshtml"
foreach ($archivo in $archivos) {
    $contenido = [System.IO.File]::ReadAllText($archivo.FullName, [System.Text.Encoding]::UTF8)
    $nuevo = $contenido `
        -replace 'Comprobante de CompraInsumo de Producto', 'Comprobante de Compra Producto' `
        -replace 'Comprobante de CompraInsumo', 'Comprobante de Compra Insumo' `
        -replace 'Detalle de CompraInsumo de Producto', 'Detalle de Compra Producto' `
        -replace 'Detalle de CompraInsumo', 'Detalle de Compra Insumo' `
        -replace 'Reporte de CompraInsumo de Insumos', 'Reporte Compra Insumo' `
        -replace 'Reporte de CompraInsumo de Productos', 'Reporte Compra Producto' `
        -replace 'Reporte de CompraInsumo', 'Reporte Compra' `
        -replace 'Nueva CompraInsumo de Insumos', 'Nueva Compra Insumo' `
        -replace 'Nueva CompraInsumo', 'Nueva Compra' `
        -replace 'Registrar CompraInsumo', 'Registrar Compra' `
        -replace 'Total CompraInsumo', 'Total Compra' `
        -replace 'Costo de CompraInsumo', 'Costo de Compra' `
        -replace 'Precio CompraInsumo', 'Precio Compra' `
        -replace 'TOTAL CompraInsumo', 'TOTAL COMPRA' `
        -replace 'COMPROBANTE DE CompraInsumo DE INSUMOS', 'COMPROBANTE DE COMPRA INSUMO' `
        -replace 'COMPROBANTE DE CompraInsumo DE PRODUCTOS', 'COMPROBANTE DE COMPRA PRODUCTO' `
        -replace 'CompraInsumo de Productos', 'Compra Producto' `
        -replace 'CompraInsumo de Insumos', 'Compra Insumo' `
        -replace 'CompraInsumo Producto', 'Compra Producto' `
        -replace 'CompraInsumo Insumo', 'Compra Insumo' `
        -replace 'la CompraInsumo', 'la Compra' `
        -replace 'esta CompraInsumo', 'esta Compra' `
        -replace 'Kardex General', 'Historial Movimientos' `
        -replace 'ViewData\["Title"\] = "Kardex"', 'ViewData["Title"] = "Historial Movimientos"'
    if ($nuevo -ne $contenido) {
        [System.IO.File]::WriteAllText($archivo.FullName, $nuevo, [System.Text.Encoding]::UTF8)
        Write-Host "Actualizado: $($archivo.Name)"
    }
}
Write-Host "Listo!"
